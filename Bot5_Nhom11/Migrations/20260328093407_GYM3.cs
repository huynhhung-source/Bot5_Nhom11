using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doanweb.Migrations
{
    /// <inheritdoc />
    public partial class GYM3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(775));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(779));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(847));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(850));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(853));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(887));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(890));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(893));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(896));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(898));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(901));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 611, DateTimeKind.Local).AddTicks(9664));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 611, DateTimeKind.Local).AddTicks(9667));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "UserRoleId",
                keyValue: 1,
                column: "AssignedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(750));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 16, 34, 6, 612, DateTimeKind.Local).AddTicks(718));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1570));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1574));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1577));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1579));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1582));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1605));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1608));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1611));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1613));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1615));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1618));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(606));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(609));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "UserRoleId",
                keyValue: 1,
                column: "AssignedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1546));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 36, 7, 60, DateTimeKind.Local).AddTicks(1516));
        }
    }
}
