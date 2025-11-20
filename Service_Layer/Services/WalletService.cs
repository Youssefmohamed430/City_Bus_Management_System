

namespace Service_Layer.Services
{
    public class WalletService(IUnitOfWork _unitOfWork,INotificationService notificationService) : IWalletService
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

        public ResponseModel<WalletDTO> ChargeWallet(double amount, string passengerId)
        {
            var wallet = _unitOfWork.Wallets.Find(w => w.passengerId == passengerId);

            wallet.Balance += amount;
            _unitOfWork.Wallets.UpdateAsync(wallet);
            _unitOfWork.SaveAsync();

            notificationService.SendNotification(wallet.passengerId,
                $"Wallet charged Successfully with {amount} pounds.");

            return ResponseModelFactory<WalletDTO>.CreateResponse("Wallet charged successfully", wallet.Adapt<WalletDTO>());
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
