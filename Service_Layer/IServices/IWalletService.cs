

namespace Service_Layer.IServices
{
    public interface IWalletService
    {
        ResponseModel<WalletDTO> GetWalletByPassengerId(string passengerId);
        ResponseModel<WalletDTO> CreateWallet(WalletDTO walletDTO);
        ResponseModel<WalletDTO> ChargeWallet(double amount, string passengerId);
        bool DeductBalance(double amount,string passengerId);
        bool RefundBalance(double amount,string passengerId);

    }
}
