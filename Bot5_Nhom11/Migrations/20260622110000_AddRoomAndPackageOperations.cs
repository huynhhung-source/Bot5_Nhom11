using System;
using doanweb.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doanweb.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(GymDbContext))]
    [Migration("20260622110000_AddRoomAndPackageOperations")]
    public partial class AddRoomAndPackageOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedClassTypes",
                table: "Packages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainingRoomId",
                table: "Classes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrainingRooms",
                columns: table => new
                {
                    TrainingRoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingRooms", x => x.TrainingRoomId);
                });

            migrationBuilder.Sql("""
                SET IDENTITY_INSERT [TrainingRooms] ON;

                IF NOT EXISTS (SELECT 1 FROM [TrainingRooms] WHERE [TrainingRoomId] = 1)
                    INSERT INTO [TrainingRooms] ([TrainingRoomId], [RoomName], [Capacity], [Status], [Description], [CreatedDate], [UpdatedDate])
                    VALUES (1, N'Phòng Gym 1', 30, N'Active', N'Phòng tập chính cho Gym, Strength và PT.', '2026-06-22T11:00:00', NULL);

                IF NOT EXISTS (SELECT 1 FROM [TrainingRooms] WHERE [TrainingRoomId] = 2)
                    INSERT INTO [TrainingRooms] ([TrainingRoomId], [RoomName], [Capacity], [Status], [Description], [CreatedDate], [UpdatedDate])
                    VALUES (2, N'Phòng Yoga', 20, N'Active', N'Không gian yên tĩnh cho Yoga, Pilates và Zumba.', '2026-06-22T11:00:00', NULL);

                IF NOT EXISTS (SELECT 1 FROM [TrainingRooms] WHERE [TrainingRoomId] = 3)
                    INSERT INTO [TrainingRooms] ([TrainingRoomId], [RoomName], [Capacity], [Status], [Description], [CreatedDate], [UpdatedDate])
                    VALUES (3, N'Phòng Boxing', 18, N'Active', N'Phòng chuyên dụng cho Boxing và cardio cường độ cao.', '2026-06-22T11:00:00', NULL);

                SET IDENTITY_INSERT [TrainingRooms] OFF;
                """);

            migrationBuilder.Sql("""
                UPDATE [Packages] SET [AllowedClassTypes] = N'Gym,Strength' WHERE [PackageId] = 1;
                UPDATE [Packages] SET [AllowedClassTypes] = N'Gym,Cardio' WHERE [PackageId] = 2;
                UPDATE [Packages] SET [AllowedClassTypes] = N'Gym,Strength' WHERE [PackageId] = 3;
                UPDATE [Packages] SET [AllowedClassTypes] = N'Gym,Personal Training' WHERE [PackageId] = 4;
                UPDATE [Packages] SET [AllowedClassTypes] = N'Yoga,Pilates,Zumba' WHERE [PackageId] = 5;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TrainingRoomId",
                table: "Classes",
                column: "TrainingRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_TrainingRooms_TrainingRoomId",
                table: "Classes",
                column: "TrainingRoomId",
                principalTable: "TrainingRooms",
                principalColumn: "TrainingRoomId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_TrainingRooms_TrainingRoomId",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "TrainingRooms");

            migrationBuilder.DropIndex(
                name: "IX_Classes_TrainingRoomId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "AllowedClassTypes",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "TrainingRoomId",
                table: "Classes");
        }
    }
}
