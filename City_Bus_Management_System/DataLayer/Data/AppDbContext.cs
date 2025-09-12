using City_Bus_Management_System.DataLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Route = City_Bus_Management_System.DataLayer.Entities.Route;

namespace City_Bus_Management_System.DataLayer.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<DriverRequests> DriverRequests { get; set; }
        public DbSet<DriverSchedule> DriverSchedules { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripSchedule> TripSchedules { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<BusUsageReport> BusUsageReports { get; set; }
        public DbSet<RevenueReport> RevenueReports { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
