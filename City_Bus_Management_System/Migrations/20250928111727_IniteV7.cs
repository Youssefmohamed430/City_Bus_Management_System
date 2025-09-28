using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace City_Bus_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class IniteV7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfStations",
                table: "Tickets",
                newName: "MinStations");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "MinStations",
                table: "Tickets",
                newName: "NumberOfStations");
        }
    }
}
