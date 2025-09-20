using City_Bus_Management_System.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.HasKey(s => s.SchId);

            builder.HasOne(s => s.trip)
                   .WithMany(t => t.Schedules)
                   .HasForeignKey(s => s.TripId);

            builder.HasOne(s => s.bus)
                   .WithMany(b => b.Schedules)
                   .HasForeignKey(s => s.BusId);

            builder.HasOne(s => s.driver)
                     .WithMany(d => d.Schedules)
                     .HasForeignKey(ds => ds.DriverId);
        }
    }
}
