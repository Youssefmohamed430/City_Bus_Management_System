using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace City_Bus_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class initeV8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Routes",
                table: "Routes");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Routes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Routes",
                table: "Routes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_TripId",
                table: "Routes",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Routes",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_TripId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Routes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Routes",
                table: "Routes",
                columns: new[] { "TripId", "StationId" });
        }
    }
}
