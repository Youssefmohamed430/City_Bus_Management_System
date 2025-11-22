using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.IServices
{
    public interface IPayMobService
    {
        Task<string> PayWithCard(int amountCents,string passengerid);
        Task<bool> PaymobCallback([FromBody] PaymobCallback payload, string hmacHeader);
    }
}
