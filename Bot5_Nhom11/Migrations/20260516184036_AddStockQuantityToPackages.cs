using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doanweb.Migrations
{
    /// <inheritdoc />
    public partial class AddStockQuantityToPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "StockQuantity" },
                values: new object[] { new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7219), 0 });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "StockQuantity" },
                values: new object[] { new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7222), 0 });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "StockQuantity" },
                values: new object[] { new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7225), 0 });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 4,
                columns: new[] { "CreatedDate", "StockQuantity" },
                values: new object[] { new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7227), 0 });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 5,
                columns: new[] { "CreatedDate", "StockQuantity" },
                values: new object[] { new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7230), 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7255));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7258));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7260));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7262));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7265));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7267));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 21, DateTimeKind.Local).AddTicks(6310));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 21, DateTimeKind.Local).AddTicks(6312));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "UserRoleId",
                keyValue: 1,
                column: "AssignedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7194));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7136));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Packages");

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
    }
}
