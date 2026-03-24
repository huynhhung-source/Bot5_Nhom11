using System.ComponentModel.DataAnnotations;

namespace doanweb.Models
{
    /// <summary>
    /// Model cho trang thanh toán
    /// </summary>
    public class PaymentViewModel
    {
        [Required(ErrorMessage = "PackageId là b?t bu?c")]
        public int PackageId { get; set; }

        [Required(ErrorMessage = "Tên gói là b?t bu?c")]
        [StringLength(200, ErrorMessage = "Tên gói không ???c v??t quá 200 ký t?")]
        public string PackageName { get; set; }

        [Required(ErrorMessage = "Giá là b?t bu?c")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá ph?i l?n h?n 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Th?i h?n là b?t bu?c")]
        [Range(1, 365, ErrorMessage = "Th?i h?n ph?i t? 1 ??n 365 ngày")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Vui lòng ch?n ph??ng th?c thanh toán")]
        [StringLength(50, ErrorMessage = "Ph??ng th?c thanh toán không ???c v??t quá 50 ký t?")]
        public string PaymentMethod { get; set; }

        [StringLength(100, ErrorMessage = "Mã giao d?ch không ???c v??t quá 100 ký t?")]
        public string TransactionId { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không ???c v??t quá 500 ký t?")]
        public string Notes { get; set; }
    }
}
