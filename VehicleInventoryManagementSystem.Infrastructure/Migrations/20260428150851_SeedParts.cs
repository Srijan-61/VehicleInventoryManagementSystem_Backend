using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleInventoryManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedParts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "VehicleParts",
                columns: new[] { "Part_ID", "Brand", "Created_At", "IsAvailable", "Minimum_Stock_Level", "Part_Category", "Part_Name", "Purchase_Price", "Stock_Quantity", "Unit_Price", "Updated_At" },
                values: new object[,]
                {
                    { 1, "Bosch", new DateTime(2026, 4, 28, 0, 0, 0, 0, DateTimeKind.Utc), true, 10, "Brakes", "Brake Pads", 1000m, 100, 1500m, new DateTime(2026, 4, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "Castrol", new DateTime(2026, 4, 28, 0, 0, 0, 0, DateTimeKind.Utc), true, 20, "Fluids", "Engine Oil", 1800m, 100, 2500m, new DateTime(2026, 4, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "VehicleParts",
                keyColumn: "Part_ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "VehicleParts",
                keyColumn: "Part_ID",
                keyValue: 2);
        }
    }
}
