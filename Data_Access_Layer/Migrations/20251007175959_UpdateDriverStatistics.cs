using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace City_Bus_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDriverStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateTime",
                table: "DriverStatistics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateTime",
                table: "DriverStatistics");
        }
    }
}
