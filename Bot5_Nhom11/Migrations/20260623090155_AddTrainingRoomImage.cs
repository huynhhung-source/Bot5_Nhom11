using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doanweb.Migrations
{
    public partial class AddTrainingRoomImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "TrainingRooms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE [TrainingRooms]
                SET [ImageUrl] = CASE [TrainingRoomId]
                    WHEN 1 THEN N'/img/gallery/gallery-1.jpg'
                    WHEN 2 THEN N'/img/gallery/gallery-3.jpg'
                    WHEN 3 THEN N'/img/gallery/gallery-2.jpg'
                    ELSE [ImageUrl]
                END
                WHERE [TrainingRoomId] IN (1, 2, 3)
                  AND ([ImageUrl] IS NULL OR [ImageUrl] = N'');
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "TrainingRooms");
        }
    }
}
