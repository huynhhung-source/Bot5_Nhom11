using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doanweb.Migrations
{
    /// <inheritdoc />
    public partial class Inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8710));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8713));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8716));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8718));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8720));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8749));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8752));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8755));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8757));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8759));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8761));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(127));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(129));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "UserRoleId",
                keyValue: 1,
                column: "AssignedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8684));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 17, 47, 36, 452, DateTimeKind.Local).AddTicks(8634));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
