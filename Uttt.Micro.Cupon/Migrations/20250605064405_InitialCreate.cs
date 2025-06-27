using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Uttt.Micro.Cupon.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    CouponId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinAmount = table.Column<int>(type: "int", nullable: false),
                    AmountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LimitUse = table.Column<int>(type: "int", nullable: false),
                    DateInt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateCoupon = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.CouponId);
                });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CouponId", "AmountType", "Category", "CouponCode", "DateEnd", "DateInt", "DiscountAmount", "LimitUse", "MinAmount", "StateCoupon" },
                values: new object[,]
                {
                    { 1, "PERCENTAGE", "GENERAL", "100FF", new DateTime(2025, 12, 5, 0, 44, 4, 928, DateTimeKind.Local).AddTicks(4062), new DateTime(2025, 6, 5, 0, 44, 4, 914, DateTimeKind.Local).AddTicks(6195), "10", 100, 20, true },
                    { 2, "PERCENTAGE", "PREMIUM", "200FF", new DateTime(2025, 9, 5, 0, 44, 4, 928, DateTimeKind.Local).AddTicks(9773), new DateTime(2025, 6, 5, 0, 44, 4, 928, DateTimeKind.Local).AddTicks(9770), "20", 50, 40, true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
