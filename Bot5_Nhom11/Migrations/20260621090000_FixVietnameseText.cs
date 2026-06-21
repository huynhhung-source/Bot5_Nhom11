using doanweb.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doanweb.Migrations
{
    [DbContext(typeof(GymDbContext))]
    [Migration("20260621090000_FixVietnameseText")]
    public partial class FixVietnameseText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [Roles] SET [Description] = N'Quản trị viên hệ thống' WHERE [RoleId] = 1;

                UPDATE [Packages] SET
                    [Description] = N'Chương trình tập luyện cá nhân với hướng dẫn dinh dưỡng chi tiết',
                    [Features] = N'Cá nhân hóa, Video HD 1080p, Hỗ trợ email 24/7'
                WHERE [PackageId] = 1;

                UPDATE [Packages] SET
                    [Description] = N'Chương trình cardio và tập tạ với kế hoạch ăn uống thực dụng',
                    [Features] = N'Cardio, Tập tạ, Ăn uống, Hỗ trợ hàng tuần'
                WHERE [PackageId] = 2;

                UPDATE [Packages] SET
                    [Description] = N'Truy cập tất cả trang thiết bị với huấn luyện viên riêng',
                    [Features] = N'Tất cả thiết bị, Huấn luyện viên 2 buổi/tuần, Phòng xông hơi'
                WHERE [PackageId] = 3;

                UPDATE [Packages] SET
                    [Description] = N'Huấn luyện viên riêng 4 buổi/tuần với chương trình cá nhân hóa',
                    [Features] = N'Huấn luyện viên riêng, Cá nhân hóa, Theo dõi chi tiết'
                WHERE [PackageId] = 4;

                UPDATE [Packages] SET
                    [Description] = N'Các lớp Yoga, Pilates và Zumba với huấn luyện viên chuyên nghiệp',
                    [Features] = N'Yoga hằng ngày, Pilates, Zumba, Cộng đồng'
                WHERE [PackageId] = 5;

                UPDATE [Products] SET [Description] = N'Bột Whey Protein chất lượng cao, giàu amino axit, hỗ trợ phục hồi cơ bắp sau tập luyện.' WHERE [ProductId] = 1;
                UPDATE [Products] SET [Description] = N'Creatine Monohydrate tinh khiết 100%, tăng cường sức mạnh và khả năng phục hồi cơ bắp.' WHERE [ProductId] = 2;
                UPDATE [Products] SET [Description] = N'Bánh Protein Bar vị sô-cô-la, chứa 20g protein, ít đường, phù hợp dùng trước hoặc sau tập.' WHERE [ProductId] = 3;
                UPDATE [Products] SET [Description] = N'Bột Whey Protein nồng độ cao, hỗ trợ xây dựng khối cơ, giàu BCAA tự nhiên và dễ hòa tan.' WHERE [ProductId] = 4;
                UPDATE [Products] SET [Description] = N'Hỗn hợp Creatine và Beta-Alanine, tăng hiệu suất tập luyện, giảm mệt mỏi và cải thiện sức bền.' WHERE [ProductId] = 5;
                UPDATE [Products] SET [Description] = N'Bánh Protein Bar vị bơ đậu phộng, 25g protein, không chứa đường, bổ sung năng lượng cho ngày dài.' WHERE [ProductId] = 6;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
