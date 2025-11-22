

using Azure.Core;
using City_Bus_Management_System.DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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

        public async Task<ResponseModel<object>> PaymobCallback([FromBody] PaymobCallback payload, string hmacHeader, string passengerid)
        {
            try
            {
                logger.LogInformation("Paymob callback received with payload: {payload}", System.Text.Json.JsonSerializer.Serialize(payload));
                _unitOfWork.BeginTransaction();
                
                if (await payMobService.PaymobCallback(payload, hmacHeader))
                {
                    await notificationService.SendNotification(passengerid,
                    $"Your card has been successfully debited with {Convert.ToInt32(payload.obj.amount_cents) / 100} pounds.");

                    var passengerId = payload.obj.payment_key_claims.billing_data.apartment;

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
    }
}
