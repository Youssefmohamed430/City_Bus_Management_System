
namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.HasKey(un => new { un.UserId, un.NotifId });

            builder.HasOne(un => un.Notif)
                   .WithMany(n => n.UserNotifications)
                   .HasForeignKey(n => n.NotifId);

            builder.HasOne(un => un.User)
                   .WithMany(u => u.UserNotifications)
                   .HasForeignKey(n => n.UserId);
        }
    }
}
