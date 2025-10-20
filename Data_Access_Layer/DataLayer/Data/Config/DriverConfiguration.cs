namespace City_Bus_Management_System.DataLayer.Data.Config
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.Id);

            builder.HasOne(d => d.User)
                   .WithOne(d => d.Driver)
                   .HasForeignKey<Driver>(d => d.Id);

            builder.ToTable("Drivers", 
                d => d.HasCheckConstraint("CK_Users_SSN_Format", "LEN(SSN) = 14 AND SSN NOT LIKE '%[^0-9]%'"));
        }
    }
}
