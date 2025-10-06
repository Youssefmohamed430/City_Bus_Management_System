using City_Bus_Management_System.DataLayer.Entities;
using Core_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.DataLayer.Data.Config
{
    public class DriverStatisticsConfiguration : IEntityTypeConfiguration<DriverStatistics>
    {
        public void Configure(EntityTypeBuilder<DriverStatistics> builder)
        {
            builder.HasKey(ds => ds.Id);

            builder.HasOne(ds => ds.driver)
                   .WithOne(d => d.DriverStatistics)
                   .HasForeignKey<DriverStatistics>(ds => ds.DriverId);
        }
    }
}
