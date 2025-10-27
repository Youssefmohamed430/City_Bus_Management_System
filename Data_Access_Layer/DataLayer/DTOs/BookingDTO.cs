﻿
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
        public int StationFromId { get; set; }
        public int StationToId { get; set; }
    }
}
