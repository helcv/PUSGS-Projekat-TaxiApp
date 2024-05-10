using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taxi_App.Data.Migrations
{
    /// <inheritdoc />
    public partial class AvgRateAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AvgRate",
                table: "AspNetUsers",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvgRate",
                table: "AspNetUsers");
        }
    }
}
