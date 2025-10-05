using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.DataLayer.DTOs
{
    public class WalletDTO
    {
        public int? Id { get; set; }
        public double Balance { get; set; } = 0.0;
        public required string passengerId { get; set; }
    }
}
