using System.ComponentModel.DataAnnotations;

namespace doanweb.Models
{
    /// <summary>
    /// Model cho trang thanh toán.
    /// </summary>
    public class PaymentViewModel
    {
        [Required(ErrorMessage = "PackageId là bắt buộc")]
        public int PackageId { get; set; }

        [Required(ErrorMessage = "Tên gói là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên gói không được vượt quá 200 ký tự")]
        public string PackageName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Thời hạn là bắt buộc")]
        [Range(1, 365, ErrorMessage = "Thời hạn phải từ 1 đến 365 ngày")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [StringLength(50, ErrorMessage = "Phương thức thanh toán không được vượt quá 50 ký tự")]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Mã giao dịch không được vượt quá 100 ký tự")]
        public string TransactionId { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string Notes { get; set; } = string.Empty;

        public int? GymId { get; set; }

        [StringLength(150)]
        public string? GymName { get; set; }

        [StringLength(150)]
        public string? ClassName { get; set; }

        [StringLength(150)]
        public string? InstructorName { get; set; }

        [StringLength(250)]
        public string? GymAddress { get; set; }

        [StringLength(80)]
        public string? GymHours { get; set; }

        public bool IsGymCheckout => GymId.HasValue;
    }

    public class ClassPaymentViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string ClassType { get; set; } = string.Empty;
        public DateTime ClassDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int RegisteredCount { get; set; }
        public int AvailableSlots => Math.Max(0, Capacity - RegisteredCount);
        public string Status { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public int UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(100)]
        public string? TransactionId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
