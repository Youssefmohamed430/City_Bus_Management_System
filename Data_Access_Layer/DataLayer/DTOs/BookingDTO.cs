using City_Bus_Management_System.DataLayer.DTOs;
using City_Bus_Management_System.DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.DataLayer.DTOs
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Booked";
        public string? passengerId { get; set; }
        public string? passengerName { get; set; }
        public int TicketId { get; set; }
        public int TripId { get; set; }
        public double? Price { get; set; } 
        public string? BusType { get; set; }
    }
}
