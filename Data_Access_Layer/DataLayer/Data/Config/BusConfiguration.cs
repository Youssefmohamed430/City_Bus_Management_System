namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class BusConfiguration : IEntityTypeConfiguration<Bus>
    {
        public void Configure(EntityTypeBuilder<Bus> builder)
        {
            builder.HasKey(b => b.BusId);
        }
    }
}
