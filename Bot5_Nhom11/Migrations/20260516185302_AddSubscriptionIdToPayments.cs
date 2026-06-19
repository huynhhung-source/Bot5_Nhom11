using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doanweb.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionIdToPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Subscriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Subscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8657));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8660));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8662));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8664));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8666));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8692));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8694));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8697));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8699));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8701));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8703));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(4238));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(4240));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "UserRoleId",
                keyValue: 1,
                column: "AssignedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8635));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 53, 2, 286, DateTimeKind.Local).AddTicks(8585));

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Subscriptions_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "SubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Subscriptions_SubscriptionId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_SubscriptionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7219));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7222));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7225));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7227));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 5, 17, 1, 40, 36, 22, DateTimeKind.Local).AddTicks(7230));

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
    }
}
