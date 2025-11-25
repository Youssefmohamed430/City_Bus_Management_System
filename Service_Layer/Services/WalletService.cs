

using Azure.Core;
using City_Bus_Management_System.DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Service_Layer.Services
{
    public class WalletService(ILogger<WalletService> logger,IPayMobService payMobService,IUnitOfWork _unitOfWork,INotificationService notificationService) : IWalletService
    {
        public ResponseModel<WalletDTO> CreateWallet(WalletDTO walletDTO)
        {
            var wallet = walletDTO.Adapt<Wallet>();

            _unitOfWork.Wallets.AddAsync(wallet);

            _unitOfWork.SaveAsync();

            notificationService.SendNotification(wallet.passengerId,
                "Wallet created, please charge it to book trips.");

            return ResponseModelFactory<WalletDTO>.CreateResponse("Wallet created successfully", wallet.Adapt<WalletDTO>());
        }

        public Task<string> ChargeWallet(double amount,string passengerid)
        {
            return payMobService.PayWithCard((int)amount,passengerid);
        }

        public ResponseModel<WalletDTO> UpdateBalance(double amount,string passengerid)
        {
            var wallet = _unitOfWork.Wallets.Find(w => w.passengerId == passengerid);

            wallet.Balance += amount;
            _unitOfWork.Wallets.UpdateAsync(wallet);
            _unitOfWork.SaveAsync();

            notificationService.SendNotification(wallet.passengerId,
                $"Wallet charged Successfully with {amount} pounds.");

            return ResponseModelFactory<WalletDTO>.CreateResponse("Wallet charged successfully", wallet.Adapt<WalletDTO>());
        }

        public async Task<ResponseModel<object>> PaymobCallback([FromBody] PaymobCallback payload, string hmacHeader)
        {
            try
            {
                logger.LogInformation("Paymob callback received with payload: {payload}", System.Text.Json.JsonSerializer.Serialize(payload));
                _unitOfWork.BeginTransaction();
                
                if (await payMobService.PaymobCallback(payload, hmacHeader))
                {
                    var passengerId = payload.obj.payment_key_claims.billing_data.apartment;
                    await notificationService.SendNotification(passengerId,
                    $"Your card has been successfully debited with {Convert.ToInt32(payload.obj.amount_cents) / 100} pounds.");


                    UpdateBalance((double)Convert.ToInt32(payload.obj.amount_cents) / 100,passengerId);

                    _unitOfWork.Commit();

                    return ResponseModelFactory<object>.CreateResponse(
                        "The payment process was completed successfully", null!);
                }
                else { 
                    _unitOfWork.Commit();

                return ResponseModelFactory<object>.CreateResponse(
                       "Sorry, The payment process was failed.", null!, false);
            }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ResponseModelFactory<object>.CreateResponse(
                    $"Sorry, An error occurred during the payment process. {ex.Message}", null!, false);
            }
        }

        public bool DeductBalance(double amount, string passengerId)
        {
            var wallet = _unitOfWork.Wallets.Find(w => w.passengerId == passengerId);

            if (wallet.Balance < amount)
                return false;

            wallet.Balance -= amount;
            _unitOfWork.Wallets.UpdateAsync(wallet);
            _unitOfWork.SaveAsync();

            notificationService.SendNotification(wallet.passengerId,
                $"Wallet deducted Successfully with {amount} pounds.");

            return true;
        }

        public ResponseModel<WalletDTO> GetWalletByPassengerId(string passengerId)
        {
            var wallet = _unitOfWork.Wallets.GetWalletByPassengerId<WalletDTO>(passengerId);

            return ResponseModelFactory<WalletDTO>.CreateResponse("Wallet fetched successfully", wallet.Adapt<WalletDTO>());
        }

        public bool RefundBalance(double amount, string passengerId)
        {
            var wallet = _unitOfWork.Wallets.Find(w => w.passengerId == passengerId);

            wallet.Balance += amount;
            _unitOfWork.Wallets.UpdateAsync(wallet);
            _unitOfWork.SaveAsync();

            return false;
        }

        public Task<ResponseModel<WalletDTO>> SendMoneyBetweenTwoWallets(TransformMoneyDTO transformMoneyDTO)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var walletFrom = _unitOfWork.Wallets.FindWithTracking(w => w.passengerId == transformMoneyDTO.PassengerFromId
                , new string[] { "passenger.User" });


                logger.LogInformation("Sender wallet details: {walletFrom}", JsonSerializer.Serialize(walletFrom.Adapt<WalletDTO>()));

                var walletTo = _unitOfWork.GetRepository<ApplicationUser>()
                     .FindWithTracking(u => u.UserName == transformMoneyDTO.PassengerToUserName, new string[] { "Passenger.wallet" })?.Passenger?.wallet;

                logger.LogInformation("Recipient wallet details: {walletTo}", JsonSerializer.Serialize(walletTo.Adapt<WalletDTO>()));

                if (walletTo == null)
                    throw new Exception("Recipient wallet not found.");

                if (walletFrom.Balance < transformMoneyDTO.Amount)
                    throw new Exception("Insufficient balance in sender's wallet.");

                UpdateWalletBalance(transformMoneyDTO.Amount, walletFrom, walletTo);

                NotifyUpdate(transformMoneyDTO.PassengerToUserName, transformMoneyDTO.Amount, walletFrom, walletTo);

                _unitOfWork.Commit();

                return Task.FromResult(ResponseModelFactory<WalletDTO>.CreateResponse(
                    "Money transferred successfully", walletFrom.Adapt<WalletDTO>()));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                logger.LogError("Error during money transfer: {Message}", ex.Message);

                return Task.FromResult(ResponseModelFactory<WalletDTO>.CreateResponse(
                    $"Sorry, An error occurred during the transaction process. {ex.Message}", null!, false));
            }
        }

        private void NotifyUpdate(string PassengerTwoUserName, int amount, Wallet walletFrom, Wallet walletTo)
        {
            try
            {
                notificationService.SendNotification(walletFrom.passengerId,
                                $"You have sent {amount} pounds to {PassengerTwoUserName} successfully.");

                notificationService.SendNotification(walletTo.passengerId,
                    $"You have received {amount} pounds from {walletFrom.passenger?.User?.UserName} successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending notifications: " + ex.Message);
            }
        }

        private void UpdateWalletBalance(int amount, Wallet walletFrom, Wallet walletTo)
        {
            try
            {
                walletFrom.Balance -= amount;
                walletTo.Balance += amount;

                _unitOfWork.Wallets.UpdateAsync(walletFrom);
                _unitOfWork.Wallets.UpdateAsync(walletTo);

                _unitOfWork.SaveAsync();

                logger.LogInformation("Wallet balances updated successfully: {walletFrom}, {walletTo}",
                    JsonSerializer.Serialize(walletFrom.Adapt<WalletDTO>()),
                    JsonSerializer.Serialize(walletTo.Adapt<WalletDTO>()));
            }
            catch (Exception ex)
            {
                logger.LogError("Error updating wallet balances: {Message}", ex.Message);

                throw new Exception("Error updating wallet balances: " + ex.Message);
            }
        }
    }
}
