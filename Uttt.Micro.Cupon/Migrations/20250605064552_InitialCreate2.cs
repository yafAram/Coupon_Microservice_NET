using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uttt.Micro.Cupon.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 1,
                columns: new[] { "DateEnd", "DateInt" },
                values: new object[] { new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 2,
                columns: new[] { "DateEnd", "DateInt" },
                values: new object[] { new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 1,
                columns: new[] { "DateEnd", "DateInt" },
                values: new object[] { new DateTime(2025, 12, 5, 0, 44, 4, 928, DateTimeKind.Local).AddTicks(4062), new DateTime(2025, 6, 5, 0, 44, 4, 914, DateTimeKind.Local).AddTicks(6195) });

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "CouponId",
                keyValue: 2,
                columns: new[] { "DateEnd", "DateInt" },
                values: new object[] { new DateTime(2025, 9, 5, 0, 44, 4, 928, DateTimeKind.Local).AddTicks(9773), new DateTime(2025, 6, 5, 0, 44, 4, 928, DateTimeKind.Local).AddTicks(9770) });
        }
    }
}
