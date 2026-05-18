using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleInventoryManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPurchaseItemRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "VehicleParts",
                keyColumn: "Part_ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "VehicleParts",
                keyColumn: "Part_ID",
                keyValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "Invoice_Number",
                table: "PurchaseInvoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "PurchaseInvoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Sub_Total",
                table: "PurchaseInvoices",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax_Amount",
                table: "PurchaseInvoices",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Invoice_Number",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "Sub_Total",
                table: "PurchaseInvoices");

            migrationBuilder.DropColumn(
                name: "Tax_Amount",
                table: "PurchaseInvoices");

            migrationBuilder.InsertData(
                table: "VehicleParts",
                columns: new[] { "Part_ID", "Brand", "Created_At", "IsAvailable", "Minimum_Stock_Level", "Part_Category", "Part_Name", "Purchase_Price", "Stock_Quantity", "Unit_Price", "Updated_At" },
                values: new object[,]
                {
                    { 1, "Bosch", new DateTime(2026, 4, 28, 0, 0, 0, 0, DateTimeKind.Utc), true, 10, "Brakes", "Brake Pads", 1000m, 100, 1500m, new DateTime(2026, 4, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "Castrol", new DateTime(2026, 4, 28, 0, 0, 0, 0, DateTimeKind.Utc), true, 20, "Fluids", "Engine Oil", 1800m, 100, 2500m, new DateTime(2026, 4, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }
    }
}
