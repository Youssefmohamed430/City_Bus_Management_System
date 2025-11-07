using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace City_Bus_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Passengers_passengerId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Stations_StationFromId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Stations_StationToId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Tickets_TicketId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Trips_TripId",
                table: "Bookings");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DepartureDate",
                table: "Schedules",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Passengers_passengerId",
                table: "Bookings",
                column: "passengerId",
                principalTable: "Passengers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Stations_StationFromId",
                table: "Bookings",
                column: "StationFromId",
                principalTable: "Stations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Stations_StationToId",
                table: "Bookings",
                column: "StationToId",
                principalTable: "Stations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Tickets_TicketId",
                table: "Bookings",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Trips_TripId",
                table: "Bookings",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Passengers_passengerId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Stations_StationFromId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Stations_StationToId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Tickets_TicketId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Trips_TripId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DepartureDate",
                table: "Schedules");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Passengers_passengerId",
                table: "Bookings",
                column: "passengerId",
                principalTable: "Passengers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Stations_StationFromId",
                table: "Bookings",
                column: "StationFromId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Stations_StationToId",
                table: "Bookings",
                column: "StationToId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Tickets_TicketId",
                table: "Bookings",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Trips_TripId",
                table: "Bookings",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
