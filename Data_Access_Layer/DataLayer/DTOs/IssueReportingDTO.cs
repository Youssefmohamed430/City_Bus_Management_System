using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.DataLayer.DTOs
{
    public class IssueReportingDTO
    {
        public int? TripId { get; set; }
        public string? DriverId { get; set; }
        public string? DriverName { get; set; }
        public string? IssueDescription { get; set; }
    }
}
