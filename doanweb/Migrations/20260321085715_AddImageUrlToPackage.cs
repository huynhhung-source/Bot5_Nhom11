using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doanweb.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Packages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2026, 3, 21, 15, 57, 14, 652, DateTimeKind.Local).AddTicks(5221), null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2026, 3, 21, 15, 57, 14, 652, DateTimeKind.Local).AddTicks(5224), null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2026, 3, 21, 15, 57, 14, 652, DateTimeKind.Local).AddTicks(5227), null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2026, 3, 21, 15, 57, 14, 652, DateTimeKind.Local).AddTicks(5230), null });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2026, 3, 21, 15, 57, 14, 652, DateTimeKind.Local).AddTicks(5233), null });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 21, 15, 57, 14, 652, DateTimeKind.Local).AddTicks(1001));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 21, 15, 57, 14, 652, DateTimeKind.Local).AddTicks(1003));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "UserRoleId",
                keyValue: 1,
                column: "AssignedDate",
                value: new DateTime(2026, 3, 21, 15, 57, 14, 652, DateTimeKind.Local).AddTicks(5166));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 21, 15, 57, 14, 652, DateTimeKind.Local).AddTicks(5073));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Packages");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 21, 0, 26, 21, 216, DateTimeKind.Local).AddTicks(9380));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 21, 0, 26, 21, 216, DateTimeKind.Local).AddTicks(9383));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 21, 0, 26, 21, 216, DateTimeKind.Local).AddTicks(9386));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 21, 0, 26, 21, 216, DateTimeKind.Local).AddTicks(9388));

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 21, 0, 26, 21, 216, DateTimeKind.Local).AddTicks(9390));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 21, 0, 26, 21, 216, DateTimeKind.Local).AddTicks(2960));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 21, 0, 26, 21, 216, DateTimeKind.Local).AddTicks(2965));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "UserRoleId",
                keyValue: 1,
                column: "AssignedDate",
                value: new DateTime(2026, 3, 21, 0, 26, 21, 216, DateTimeKind.Local).AddTicks(9351));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 21, 0, 26, 21, 216, DateTimeKind.Local).AddTicks(9298));
        }
    }
}
