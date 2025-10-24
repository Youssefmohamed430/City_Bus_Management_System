namespace City_Bus_Management_System.DataLayer.Entities
{
    public class Booking
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Booked";
        public required string passengerId { get; set; }
        public int TicketId { get; set; }
        public int TripId { get; set; }
        public int StationFromId { get; set; }
        public Station StationFrom { get; set; }
        public int StationToId { get; set; }
        public Station StationTo { get; set; }
        public Passenger? passenger { get; set; }
        public Ticket? Ticket { get; set; }
        public Trip? Trip { get; set; }
    }
}
