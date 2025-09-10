using City_Bus_Management_System.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class TripScheduleConfiguration : IEntityTypeConfiguration<TripSchedule>
    {
        public void Configure(EntityTypeBuilder<TripSchedule> builder)
        {
            builder.HasKey(ts => ts.Id);

            builder.HasOne(ts => ts.trip)
                   .WithMany(t => t.TripSchedules)
                   .HasForeignKey(ts => ts.TripId);

            builder.HasOne(ts => ts.bus)
                   .WithMany(b => b.TripSchedules)
                   .HasForeignKey(ts => ts.BusId);  
        }
    }
}
