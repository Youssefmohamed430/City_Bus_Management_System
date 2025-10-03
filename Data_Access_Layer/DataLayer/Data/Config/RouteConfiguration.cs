using City_Bus_Management_System.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Route = City_Bus_Management_System.DataLayer.Entities.Route;

namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.trip)
                .WithMany(t => t.Routes)
                .HasForeignKey(r => r.TripId);

            builder.HasOne(r => r.station)
                .WithMany(s => s.Routes)
                .HasForeignKey(r => r.StationId);
        }
    }
}
