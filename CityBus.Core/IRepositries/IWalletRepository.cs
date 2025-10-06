using City_Bus_Management_System.DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.IRepositries
{
    public interface IWalletRepository : IBaseRepository<Wallet>
    {
        TDto GetWalletByPassengerId<TDto>(string passengerId);
    }
}
