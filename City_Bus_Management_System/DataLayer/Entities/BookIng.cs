namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Booking
    {
        public int BookingId { get; set; }
        public required DateTime BookingDate { get; set; } = DateTime.Now;
        public required string Status { get; set; }
        public required string passengerId { get; set; }
        public int TicketId { get; set; }
        public int TripId { get; set; }
        public Passenger? passenger { get; set; }
        public Ticket? Ticket { get; set; }
        public Trip? Trip { get; set; }
    }
}
