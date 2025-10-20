
namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class RevenueReportConfiguration : IEntityTypeConfiguration<RevenueReport>
    {
        public void Configure(EntityTypeBuilder<RevenueReport> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.report)
                   .WithOne(rep => rep.Revenuereport)
                   .HasForeignKey<RevenueReport>(r => r.ReportId);
        }
    }
}
