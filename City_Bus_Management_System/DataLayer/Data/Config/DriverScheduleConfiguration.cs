using City_Bus_Management_System.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class DriverScheduleConfiguration : IEntityTypeConfiguration<DriverSchedule>
    {
        public void Configure(EntityTypeBuilder<DriverSchedule> builder)
        {
            builder.HasKey(ds => new {ds.DriverId , ds.BusId});

            builder.HasOne(ds => ds.bus)
                   .WithMany(b => b.DriverSchedules)
                   .HasForeignKey(ds => ds.BusId);

            builder.HasOne(ds => ds.driver)
                     .WithMany(d => d.DriverSchedules)
                     .HasForeignKey(ds => ds.DriverId);
        }
    }
}
