using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace City_Bus_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusUsageReports_Buses_BusId",
                table: "BusUsageReports");

            migrationBuilder.DropForeignKey(
                name: "FK_BusUsageReports_Reports_ReportId",
                table: "BusUsageReports");

            migrationBuilder.DropForeignKey(
                name: "FK_RevenueReports_Reports_ReportId",
                table: "RevenueReports");

            migrationBuilder.DropForeignKey(
                name: "FK_RevenueReports_Trips_tripId",
                table: "RevenueReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RevenueReports",
                table: "RevenueReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reports",
                table: "Reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusUsageReports",
                table: "BusUsageReports");

            migrationBuilder.RenameTable(
                name: "RevenueReports",
                newName: "RevenueReport");

            migrationBuilder.RenameTable(
                name: "Reports",
                newName: "Report");

            migrationBuilder.RenameTable(
                name: "BusUsageReports",
                newName: "BusUsageReport");

            migrationBuilder.RenameIndex(
                name: "IX_RevenueReports_tripId",
                table: "RevenueReport",
                newName: "IX_RevenueReport_tripId");

            migrationBuilder.RenameIndex(
                name: "IX_RevenueReports_ReportId",
                table: "RevenueReport",
                newName: "IX_RevenueReport_ReportId");

            migrationBuilder.RenameIndex(
                name: "IX_BusUsageReports_ReportId",
                table: "BusUsageReport",
                newName: "IX_BusUsageReport_ReportId");

            migrationBuilder.RenameIndex(
                name: "IX_BusUsageReports_BusId",
                table: "BusUsageReport",
                newName: "IX_BusUsageReport_BusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RevenueReport",
                table: "RevenueReport",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Report",
                table: "Report",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusUsageReport",
                table: "BusUsageReport",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusUsageReport_Buses_BusId",
                table: "BusUsageReport",
                column: "BusId",
                principalTable: "Buses",
                principalColumn: "BusId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusUsageReport_Report_ReportId",
                table: "BusUsageReport",
                column: "ReportId",
                principalTable: "Report",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RevenueReport_Report_ReportId",
                table: "RevenueReport",
                column: "ReportId",
                principalTable: "Report",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RevenueReport_Trips_tripId",
                table: "RevenueReport",
                column: "tripId",
                principalTable: "Trips",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusUsageReport_Buses_BusId",
                table: "BusUsageReport");

            migrationBuilder.DropForeignKey(
                name: "FK_BusUsageReport_Report_ReportId",
                table: "BusUsageReport");

            migrationBuilder.DropForeignKey(
                name: "FK_RevenueReport_Report_ReportId",
                table: "RevenueReport");

            migrationBuilder.DropForeignKey(
                name: "FK_RevenueReport_Trips_tripId",
                table: "RevenueReport");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RevenueReport",
                table: "RevenueReport");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Report",
                table: "Report");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusUsageReport",
                table: "BusUsageReport");

            migrationBuilder.RenameTable(
                name: "RevenueReport",
                newName: "RevenueReports");

            migrationBuilder.RenameTable(
                name: "Report",
                newName: "Reports");

            migrationBuilder.RenameTable(
                name: "BusUsageReport",
                newName: "BusUsageReports");

            migrationBuilder.RenameIndex(
                name: "IX_RevenueReport_tripId",
                table: "RevenueReports",
                newName: "IX_RevenueReports_tripId");

            migrationBuilder.RenameIndex(
                name: "IX_RevenueReport_ReportId",
                table: "RevenueReports",
                newName: "IX_RevenueReports_ReportId");

            migrationBuilder.RenameIndex(
                name: "IX_BusUsageReport_ReportId",
                table: "BusUsageReports",
                newName: "IX_BusUsageReports_ReportId");

            migrationBuilder.RenameIndex(
                name: "IX_BusUsageReport_BusId",
                table: "BusUsageReports",
                newName: "IX_BusUsageReports_BusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RevenueReports",
                table: "RevenueReports",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reports",
                table: "Reports",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusUsageReports",
                table: "BusUsageReports",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusUsageReports_Buses_BusId",
                table: "BusUsageReports",
                column: "BusId",
                principalTable: "Buses",
                principalColumn: "BusId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusUsageReports_Reports_ReportId",
                table: "BusUsageReports",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RevenueReports_Reports_ReportId",
                table: "RevenueReports",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RevenueReports_Trips_tripId",
                table: "RevenueReports",
                column: "tripId",
                principalTable: "Trips",
                principalColumn: "Id");
        }
    }
}
