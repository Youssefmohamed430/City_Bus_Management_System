namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.BookingId);

            builder.HasOne(b => b.Trip)
                   .WithMany(t => t.Bookings)
                   .HasForeignKey(b => b.TripId);

            builder.HasOne(b => b.passenger)
                   .WithMany(u => u.BookIngs)
                   .HasForeignKey(b => b.passengerId);

            builder.HasOne(b => b.Ticket)
                   .WithMany(ts => ts.BookIngs)
                   .HasForeignKey(b => b.TicketId);

            builder.HasOne(b => b.StationFrom)
                   .WithMany(s => s.FromStationsBookings)
                   .HasForeignKey(b => b.StationFromId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.StationTo)
                   .WithMany(s => s.ToStationsBookings)
                   .HasForeignKey(b => b.StationToId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
