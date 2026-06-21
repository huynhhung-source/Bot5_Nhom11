using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doanweb.Migrations
{
    /// <inheritdoc />
    public partial class GYM12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8061));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8064));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8067));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8069));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8071));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8132));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8135));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8137));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8139));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8141));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8143));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(7135));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(7137));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "UserRoleId",
                keyValue: 1,
                column: "AssignedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8039));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8017));
        }
    }
}
