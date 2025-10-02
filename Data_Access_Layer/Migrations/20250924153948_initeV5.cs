﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace City_Bus_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class initeV5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stations");
        }
    }
}
