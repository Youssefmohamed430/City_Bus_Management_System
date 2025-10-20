
namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.User)
                   .WithOne(p => p.Passenger)
                   .HasForeignKey<Passenger>(p => p.Id);
        }
    }
}
