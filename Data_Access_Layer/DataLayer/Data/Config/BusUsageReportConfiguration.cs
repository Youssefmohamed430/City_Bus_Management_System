using City_Bus_Management_System.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class BusUsageReportConfiguration : IEntityTypeConfiguration<BusUsageReport>
    {
        public void Configure(EntityTypeBuilder<BusUsageReport> builder)
        {
            builder.HasKey(bur => bur.Id);

            builder.HasOne(bur => bur.report)
                   .WithOne(r => r.BusUsagereports)
                   .HasForeignKey<BusUsageReport>(bur => bur.ReportId);
        }
    }
}
