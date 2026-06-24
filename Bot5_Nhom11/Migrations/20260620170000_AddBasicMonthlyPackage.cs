using System;
using doanweb.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doanweb.Migrations
{
    [DbContext(typeof(GymDbContext))]
    [Migration("20260620170000_AddBasicMonthlyPackage")]
    public partial class AddBasicMonthlyPackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF NOT EXISTS (SELECT 1 FROM [Packages] WHERE [PackageName] = N'Gói Tháng Cơ Bản')
                BEGIN
                    SET IDENTITY_INSERT [Packages] ON;
                    INSERT INTO [Packages]
                        ([PackageId], [PackageName], [Description], [ImageUrl], [Price],
                         [DurationDays], [PackageType], [Category], [Features],
                         [MaxSessions], [StockQuantity], [CreatedDate], [Status])
                    VALUES
                        (6, N'Gói Tháng Cơ Bản',
                         N'Gói tập gym 1 tháng dành cho hội viên có nhu cầu luyện tập thông thường tại phòng tập.',
                         NULL, 399000, 30, N'Offline', N'Gym',
                         N'Tập gym không giới hạn, Sử dụng đầy đủ máy tập, Tủ đồ cá nhân, Hỗ trợ bài tập cơ bản',
                         30, 100, '2026-06-20', N'Active');
                    SET IDENTITY_INSERT [Packages] OFF;
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM [Packages] WHERE [PackageId] = 6 AND [PackageName] = N'Gói Tháng Cơ Bản';");
        }
    }
}
