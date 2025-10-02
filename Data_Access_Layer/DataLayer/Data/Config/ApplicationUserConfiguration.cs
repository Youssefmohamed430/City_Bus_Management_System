using City_Bus_Management_System.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(u => u.UserName)
                .IsUnique();

            builder.ToTable("Users", t =>
            {
                t.HasCheckConstraint("CK_Users_Name_Length", "LEN(Name) >= 3");
                t.HasCheckConstraint("CK_Users_Email_Format", "Email LIKE '%@%'");
                t.HasCheckConstraint("CK_Users_UserName_Format", "LEN(UserName) >= 3");
                t.HasCheckConstraint("CK_Users_PhoneNumber_Format", "PhoneNumber NOT LIKE '%[^0-9]%'");
            });
        }
    }
}
