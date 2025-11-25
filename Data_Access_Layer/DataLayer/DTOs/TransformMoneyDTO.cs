using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.DataLayer.DTOs
{
    public class TransformMoneyDTO
    {
        public string PassengerFromId { get; set; } = string.Empty;
        public string PassengerToUserName { get; set; } = string.Empty;
        public int Amount { get; set; }
    }
}
