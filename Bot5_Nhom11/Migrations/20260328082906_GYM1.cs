using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace doanweb.Migrations
{
    /// <inheritdoc />
    public partial class GYM1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Description", "Features" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8061), "Ch??ng tr?nh t?p luy?n c? nhân v?i h??ng d?n dinh d??ng chi ti?t", "C? nh?n h?a, Video HD 1080p, Email support 24/7" });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Description", "Features" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8064), "Ch??ng tr?nh cardio + t?p t? v?i k? ho?ch ?n u?ng th?c d?ng", "Cardio, T?p t?, ?n u?ng, Support h?ng tu?n" });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "Description", "Features" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8067), "Truy c?p t?t c? trang thi?t b? v?i hu?n luy?n vi?n ri?ng", "T?t c? thi?t b?, Hu?n luy?n vi?n 2x/tu?n, Steam room" });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 4,
                columns: new[] { "CreatedDate", "Description", "Features" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8069), "Hu?n luy?n vi?n ri?ng 4x/tu?n v?i ch??ng tr?nh c? nh?n h?a", "Hu?n luy?n vi?n ri?ng, C? nh?n h?a, Theo d?i chi ti?t" });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 5,
                columns: new[] { "CreatedDate", "Description", "Features" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8071), "C?c l?p Yoga, Pilates, Zumba v?i hu?n luy?n vi?n chuy?n nghi?p", "Yoga h?ng ng?y, Pilates, Zumba, C?ng ??ng" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Brand", "Category", "CreatedDate", "Description", "ImageUrl", "Price", "ProductName", "Status", "StockQuantity", "Unit", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "Optimum Nutrition", "Whey", new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8132), "Bột Whey Protein chất lượng cao, được cô lập từ 90% nguồn sữa tự nhiên. Giàu amino axit, hỗ trợ phục hồi cơ bắp sau tập luyện.", "https://images.unsplash.com/photo-1594381898348-846ce32e5eb9?w=500&h=500&fit=crop", 890000m, "Whey Protein Isolate", "Active", 50, "kg", null },
                    { 2, "Muscletech", "Creatine", new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8135), "Creatine Monohydrate tinh khiết 100%, tăng cường sức mạnh và khả năng phục hồi cơ bắp. Hỗ trợ tăng khối lượng cơ hiệu quả.", "https://images.unsplash.com/photo-1607623814075-e51df1bdc82f?w=500&h=500&fit=crop", 450000m, "Creatine Monohydrate", "Active", 40, "kg", null },
                    { 3, "Quest Nutrition", "Protein Bar", new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8137), "Bánh Protein Bar vị socola ngon miệng, chứa 20g protein, ít đường, lý tưởng cho bữa ăn nhẹ trước/sau tập luyện.", "https://images.unsplash.com/photo-1516996122174-8d440a00a6ff?w=500&h=500&fit=crop", 65000m, "Protein Bar Chocolate", "Active", 100, "box", null },
                    { 4, "Gold Standard", "Whey", new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8139), "Bột Whey Protein nồng độ cao, hỗ trợ xây dựng khối cơ, giàu BCAA tự nhiên. Vị ngon, dễ hòa tan.", "https://images.unsplash.com/photo-1590080875515-8a3a8dc5dc86?w=500&h=500&fit=crop", 650000m, "Whey Protein Concentrate", "Active", 60, "kg", null },
                    { 5, "MuscleTech", "Creatine", new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8141), "Hỗn hợp Creatine và Beta-Alanine, tăng cường hiệu suất tập luyện, giảm mệt mỏi cơ, cải thiện sức bền.", "https://images.unsplash.com/photo-1599599810694-b5ac4dd64b11?w=500&h=500&fit=crop", 520000m, "Creatine + Beta-Alanine Mix", "Active", 35, "kg", null },
                    { 6, "Quest Nutrition", "Protein Bar", new DateTime(2026, 3, 28, 15, 29, 6, 190, DateTimeKind.Local).AddTicks(8143), "Bánh Protein Bar vị bơ đậu phong hợp, 25g protein, không chứa đường, bổ sung năng lượng cho ngày dài.", "https://images.unsplash.com/photo-1553530889-e6cf89d45abf?w=500&h=500&fit=crop", 70000m, "Protein Bar Peanut Butter", "Active", 80, "box", null }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Description", "Features" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 12, 2, 187, DateTimeKind.Local).AddTicks(6092), "Ch??ng trình t?p luy?n cá nhân v?i h??ng d?n dinh d??ng chi ti?t", "Cá nhân hóa, Video HD 1080p, Email support 24/7" });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Description", "Features" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 12, 2, 187, DateTimeKind.Local).AddTicks(6095), "Ch??ng trình cardio + t?p t? v?i k? ho?ch ?n u?ng th?c d?ng", "Cardio, T?p t?, ?n u?ng, Support hàng tu?n" });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 3,
                columns: new[] { "CreatedDate", "Description", "Features" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 12, 2, 187, DateTimeKind.Local).AddTicks(6098), "Truy c?p t?t c? trang thi?t b? v?i hu?n luy?n viên riêng", "T?t c? thi?t b?, Hu?n luy?n viên 2x/tu?n, Steam room" });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 4,
                columns: new[] { "CreatedDate", "Description", "Features" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 12, 2, 187, DateTimeKind.Local).AddTicks(6100), "Hu?n luy?n viên riêng 4x/tu?n v?i ch??ng trình cá nhân hóa", "Hu?n luy?n viên riêng, Cá nhân hóa, Theo dõi chi ti?t" });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageId",
                keyValue: 5,
                columns: new[] { "CreatedDate", "Description", "Features" },
                values: new object[] { new DateTime(2026, 3, 28, 15, 12, 2, 187, DateTimeKind.Local).AddTicks(6102), "Các l?p Yoga, Pilates, Zumba v?i hu?n luy?n viên chuyên nghi?p", "Yoga hàng ngày, Pilates, Zumba, C?ng ??ng" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 12, 2, 187, DateTimeKind.Local).AddTicks(5188));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 12, 2, 187, DateTimeKind.Local).AddTicks(5190));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "UserRoleId",
                keyValue: 1,
                column: "AssignedDate",
                value: new DateTime(2026, 3, 28, 15, 12, 2, 187, DateTimeKind.Local).AddTicks(6070));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 3, 28, 15, 12, 2, 187, DateTimeKind.Local).AddTicks(6048));
        }
    }
}
