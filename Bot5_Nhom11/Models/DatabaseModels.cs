using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace doanweb.Models
{
    /// <summary>
    /// Model ??i di?n cho m?t thành viên gym
    /// </summary>
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string? PasswordHash { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; } // Male, Female, Other

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Inactive, Suspended

        // Navigation properties
        public virtual ICollection<Subscription>? Subscriptions { get; set; }
        public virtual ICollection<ClassEnrollment>? ClassEnrollments { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    /// <summary>
    /// Model ??i di?n cho các gói t?p luyến
    /// </summary>
    [Table("Packages")]
    public class Package
    {
        [Key]
        public int PackageId { get; set; }

        [Required]
        [StringLength(100)]
        public string PackageName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int DurationDays { get; set; } // S? ngày: 30, 90, etc.

        [StringLength(50)]
        public string? PackageType { get; set; } // "Online", "Offline", "Combo"

        [StringLength(50)]
        public string? Category { get; set; } // "FatLoss", "Muscle", "Strength", etc.

        [StringLength(255)]
        public string? Features { get; set; } // JSON hoặc danh sách tính năng

        public int MaxSessions { get; set; } // Số buổi tối đa trong gói

        [StringLength(255)]
        public string? AllowedClassTypes { get; set; } // Danh sách loại lớp được phép: Gym,Yoga,Boxing

        public int StockQuantity { get; set; } = 0; // Số lượng tồn kho gói tập

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Inactive

        // Navigation properties
        public virtual ICollection<Subscription>? Subscriptions { get; set; }
    }

    /// <summary>
    /// Model đại diện cho phòng tập trong chi nhánh.
    /// </summary>
    [Table("TrainingRooms")]
    public class TrainingRoom
    {
        [Key]
        public int TrainingRoomId { get; set; }

        [Required]
        [StringLength(100)]
        public string RoomName { get; set; }

        [Required]
        public int Capacity { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Inactive

        [StringLength(500)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<Class>? Classes { get; set; }
    }

    /// <summary>
    /// Model ??i di?n cho ??ng ký/subscription c?a user
    /// </summary>
    [Table("Subscriptions")]
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Package")]
        public int PackageId { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ActivationDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled, Suspended

        public int RemainingDays { get; set; }

        public int SessionsUsed { get; set; } = 0;

        public decimal AmountPaid { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("PackageId")]
        public virtual Package Package { get; set; }

        public virtual ICollection<Attendance>? Attendances { get; set; }
    }

    /// <summary>
    /// Model ??i di?n cho các l?p t?p luyến
    /// </summary>
    [Table("Classes")]
    public class Class
    {
        [Key]
        public int ClassId { get; set; }

        [ForeignKey("TrainingRoom")]
        public int? TrainingRoomId { get; set; }

        [Required]
        [StringLength(100)]
        public string ClassName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(100)]
        public string InstructorName { get; set; }

        [StringLength(50)]
        public string? ClassType { get; set; } // "Yoga", "Boxing", "Gym", "Pilates", etc.

        [StringLength(50)]
        public string? Level { get; set; } // "Beginner", "Intermediate", "Advanced"

        [DataType(DataType.Date)]
        public DateTime ClassDate { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [StringLength(200)]
        public string? Location { get; set; } // Tên phòng ho?c ??a ?i?m

        public int MaxCapacity { get; set; } = 20;

        public int CurrentEnrollment { get; set; } = 0;

        [StringLength(50)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("TrainingRoomId")]
        public virtual TrainingRoom? TrainingRoom { get; set; }

        public virtual ICollection<ClassEnrollment>? Enrollments { get; set; }
        public virtual ICollection<Attendance>? Attendances { get; set; }
    }

    /// <summary>
    /// Model ??i di?n cho ??ng ký lắp c?a user
    /// </summary>
    [Table("ClassEnrollments")]
    public class ClassEnrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Class")]
        public int ClassId { get; set; }

        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; } = "Enrolled"; // Enrolled, Completed, Cancelled

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }
    }

    /// <summary>
    /// Model ??i di?n cho ?i?m danh/attendance
    /// </summary>
    [Table("Attendances")]
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        [ForeignKey("Subscription")]
        public int SubscriptionId { get; set; }

        [Required]
        [ForeignKey("Class")]
        public int ClassId { get; set; }

        [DataType(DataType.Date)]
        public DateTime AttendanceDate { get; set; } = DateTime.Now;

        [DataType(DataType.Time)]
        public TimeSpan CheckInTime { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Present"; // Present, Absent, Late, Excused

        // Navigation properties
        [ForeignKey("SubscriptionId")]
        public virtual Subscription Subscription { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }
    }

    /// <summary>
    /// Model đại diện cho thanh toán
    /// </summary>
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Subscription")]
        public int? SubscriptionId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? PaymentMethod { get; set; } // "CreditCard", "BankTransfer", "Cash", "Wallet"

        [StringLength(100)]
        public string? TransactionId { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Success"; // Success, Failed, Pending, Refunded

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("SubscriptionId")]
        public virtual Subscription? Subscription { get; set; }
    }

    /// <summary>
    /// Model ??i di?n cho bài vi?t/blog
    /// </summary>
    [Table("Articles")]
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(500)]
        public string? Summary { get; set; }

        [StringLength(100)]
        public string? Author { get; set; }

        [StringLength(100)]
        public string? Category { get; set; } // "Fitness", "Nutrition", "Health", etc.

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Published"; // Published, Draft, Archived

        public int ViewCount { get; set; } = 0;
    }

    /// <summary>
    /// Model ??i di?n cho liên h?/contact
    /// </summary>
    [Table("Contacts")]
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; } = "New"; // New, Read, Replied, Closed

        [StringLength(500)]
        public string? AdminReply { get; set; }
    }

    /// <summary>
    /// Model ??i di?n cho ?ánh giá/review
    /// </summary>
    [Table("Reviews")]
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(50)]
        public string? ReviewType { get; set; } // "Package", "Class", "Service"

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

    /// <summary>
    /// Model đại diện cho sản phẩm (Whey, Creatine, Protein Bar, etc.)
    /// </summary>
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [Required]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string? Category { get; set; } // "Whey", "Creatine", "Protein Bar", "Vitamins", etc.

        [StringLength(50)]
        public string? Brand { get; set; } // Thương hiệu sản phẩm

        public int StockQuantity { get; set; } = 0; // Số lượng tồn kho

        [StringLength(100)]
        public string? Unit { get; set; } // "kg", "box", "bottle", etc.

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Inactive

        // Navigation properties
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }

    /// <summary>
    /// Model đại diện cho order sản phẩm
    /// </summary>
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Shipped, Delivered, Cancelled

        [StringLength(500)]
        public string? DeliveryAddress { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }

    /// <summary>
    /// Model đại diện cho các item trong một order
    /// </summary>
    [Table("OrderItems")]
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
