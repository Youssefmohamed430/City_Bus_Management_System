using City_Bus_Management_System.DataLayer;
using Data_Access_Layer.DataLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.IServices
{
    public interface IWalletService
    {
        ResponseModel<WalletDTO> GetWalletByPassengerId(string passengerId);
        ResponseModel<WalletDTO> CreateWallet(WalletDTO walletDTO);
        ResponseModel<WalletDTO> ChargeWallet(double amount, string passengerId);
        ResponseModel<WalletDTO> DeductBalance(double amount,string passengerId);
    }
}
