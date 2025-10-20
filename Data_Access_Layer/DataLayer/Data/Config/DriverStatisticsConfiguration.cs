
namespace Data_Access_Layer.DataLayer.Data.Config
{
    public class DriverStatisticsConfiguration : IEntityTypeConfiguration<DriverStatistics>
    {
        public void Configure(EntityTypeBuilder<DriverStatistics> builder)
        {
            builder.HasKey(ds => ds.Id);

            builder.HasOne(ds => ds.driver)
                   .WithOne(d => d.DriverStatistics)
                   .HasForeignKey<DriverStatistics>(ds => ds.DriverId);
        }
    }
}
