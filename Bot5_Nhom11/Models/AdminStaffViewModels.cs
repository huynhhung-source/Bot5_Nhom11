using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace doanweb.Models
{
    public class AdminStaffIndexViewModel
    {
        public List<AdminStaffItemViewModel> StaffMembers { get; set; } = new();
        public StaffMemberFormViewModel NewStaff { get; set; } = new();
        public bool ShowCreateModal { get; set; }
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int TrainerCount { get; set; }
    }

    public class AdminStaffItemViewModel
    {
        public int StaffMemberId { get; set; }
        public string StaffCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Initial { get; set; } = "A";
        public string Position { get; set; } = string.Empty;
        public string PositionKind { get; set; } = "trainer";
        public string Specialty { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public decimal Rating { get; set; }
        public int MonthlyClasses { get; set; }
        public int ExperienceYears { get; set; }
        public string Introduction { get; set; } = string.Empty;
        public string Philosophy { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Certificates { get; set; } = string.Empty;
        public string ScheduleText { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusKind { get; set; } = "active";
        public string StatusLabel { get; set; } = "Đang làm";
    }

    public class StaffMemberFormViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [StringLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(120, ErrorMessage = "Chuyên môn tối đa 120 ký tự.")]
        public string? Specialty { get; set; }

        [StringLength(255, ErrorMessage = "Đường dẫn ảnh tối đa 255 ký tự.")]
        public string? ImageUrl { get; set; }

        public IFormFile? PhotoFile { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^[0-9+\s.-]{9,20}$", ErrorMessage = "Số điện thoại chưa đúng định dạng.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email chưa đúng định dạng.")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự.")]
        public string? Email { get; set; }

        [Range(0, 999999999, ErrorMessage = "Lương phải lớn hơn hoặc bằng 0.")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vị trí.")]
        public string Position { get; set; } = "Huấn Luyện Viên";

        [Range(0, 50, ErrorMessage = "Kinh nghiệm phải từ 0 đến 50 năm.")]
        public int ExperienceYears { get; set; } = 3;

        [Range(0, 5, ErrorMessage = "Đánh giá phải từ 0 đến 5.")]
        public decimal Rating { get; set; } = 4.8m;

        [Range(0, 999, ErrorMessage = "Số lớp/tháng không hợp lệ.")]
        public int MonthlyClasses { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active";

        [StringLength(150, ErrorMessage = "Địa điểm tối đa 150 ký tự.")]
        public string? Location { get; set; }

        [StringLength(700, ErrorMessage = "Giới thiệu tối đa 700 ký tự.")]
        public string? Introduction { get; set; }

        [StringLength(500, ErrorMessage = "Triết lý tối đa 500 ký tự.")]
        public string? Philosophy { get; set; }

        [StringLength(500, ErrorMessage = "Chứng chỉ tối đa 500 ký tự.")]
        public string? Certificates { get; set; }

        [StringLength(500, ErrorMessage = "Lịch làm việc tối đa 500 ký tự.")]
        public string? ScheduleText { get; set; }
    }
}
