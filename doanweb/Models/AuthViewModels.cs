using System.ComponentModel.DataAnnotations;

namespace doanweb.Models
{
    /// <summary>
    /// Model cho trang ??ng ký
    /// </summary>
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nh?p h? tên")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "H? tên ph?i t? 3 ??n 100 ký t?")]
        [Display(Name = "H? tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nh?p email")]
        [EmailAddress(ErrorMessage = "Email không h?p l?")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nh?p s? ?i?n tho?i")]
        [Phone(ErrorMessage = "S? ?i?n tho?i không h?p l?")]
        [StringLength(20)]
        [Display(Name = "S? ?i?n tho?i")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nh?p m?t kh?u")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "M?t kh?u ph?i có ít nh?t 6 ký t?")]
        [DataType(DataType.Password)]
        [Display(Name = "M?t kh?u")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nh?n m?t kh?u")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nh?n m?t kh?u")]
        [Compare("Password", ErrorMessage = "M?t kh?u không kh?p")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Model cho trang ??ng nh?p
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nh?p email")]
        [EmailAddress(ErrorMessage = "Email không h?p l?")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nh?p m?t kh?u")]
        [DataType(DataType.Password)]
        [Display(Name = "M?t kh?u")]
        public string Password { get; set; }

        [Display(Name = "Ghi nh? tôi")]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// Model cho trang h? s? cá nhân
    /// </summary>
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Vui lòng nh?p h? tên")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "H? tên ph?i t? 3 ??n 100 ký t?")]
        [Display(Name = "H? tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nh?p email")]
        [EmailAddress(ErrorMessage = "Email không h?p l?")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nh?p s? ?i?n tho?i")]
        [Phone(ErrorMessage = "S? ?i?n tho?i không h?p l?")]
        [StringLength(20)]
        [Display(Name = "S? ?i?n tho?i")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(500)]
        [Display(Name = "??a ch?")]
        public string Address { get; set; }

        [StringLength(20)]
        [Display(Name = "Gi?i tính")]
        public string Gender { get; set; }
    }
}
