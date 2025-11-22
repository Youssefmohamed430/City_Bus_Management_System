

using Microsoft.AspNetCore.Mvc;

namespace Service_Layer.IServices
{
    public interface IWalletService
    {
        ResponseModel<WalletDTO> GetWalletByPassengerId(string passengerId);
        ResponseModel<WalletDTO> CreateWallet(WalletDTO walletDTO);
        Task<string> ChargeWallet(double amount, string passengerid);
        ResponseModel<WalletDTO> UpdateBalance(double amount, string passengerid);
        bool DeductBalance(double amount,string passengerId);
        bool RefundBalance(double amount,string passengerId);
        Task<ResponseModel<object>> PaymobCallback([FromBody] PaymobCallback payload, string hmacHeader);

    }
}
