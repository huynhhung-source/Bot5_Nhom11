using System.ComponentModel.DataAnnotations;

namespace doanweb.Models;

public sealed class AdminGymIndexViewModel
{
    public IReadOnlyList<AdminTrainingRoomViewModel> Rooms { get; init; } = [];
    public string StatusFilter { get; init; } = "all";
    public string SearchTerm { get; init; } = string.Empty;
    public int TotalCount { get; init; }
    public int ActiveCount { get; init; }
    public int HiddenCount { get; init; }
    public int TotalCapacity { get; init; }
    public int TotalRegistered { get; init; }
    public int TotalAvailableSlots { get; init; }
}

public sealed class AdminTrainingRoomViewModel
{
    public int TrainingRoomId { get; init; }
    public string RoomName { get; init; } = string.Empty;
    public int Capacity { get; init; }
    public string Status { get; init; } = "Active";
    public string? Description { get; init; }
    public int ClassCount { get; init; }
    public int RegisteredCount { get; init; }
    public int AvailableSlots { get; init; }
    public bool IsAvailable { get; init; }
    public IReadOnlyList<AdminRoomScheduleViewModel> UpcomingClasses { get; init; } = [];

    public string StatusLabel => Status == "Active" ? "Đang sử dụng" : "Đã ẩn";
    public string StatusKind => Status == "Active" ? "success" : "secondary";
    public string AvailabilityLabel => IsAvailable ? "Còn trống" : "Hết chỗ";
    public string AvailabilityKind => IsAvailable ? "success" : "danger";
}

public sealed class AdminRoomScheduleViewModel
{
    public int ClassId { get; init; }
    public string ClassName { get; init; } = string.Empty;
    public string ClassType { get; init; } = string.Empty;
    public DateTime ClassDate { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public int RegisteredCount { get; init; }
    public int Capacity { get; init; }
    public int AvailableSlots => Math.Max(0, Capacity - RegisteredCount);
}

public sealed class TrainingRoomFormViewModel
{
    public int TrainingRoomId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên phòng.")]
    [StringLength(100)]
    public string RoomName { get; set; } = string.Empty;

    [Range(1, 500, ErrorMessage = "Sức chứa phải từ 1 đến 500.")]
    public int Capacity { get; set; } = 20;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Active";

    [StringLength(500)]
    public string? Description { get; set; }
}

public sealed class RoomScheduleFormViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn phòng tập.")]
    public int TrainingRoomId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên lớp.")]
    [StringLength(100)]
    public string ClassName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập huấn luyện viên.")]
    [StringLength(100)]
    public string InstructorName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? ClassType { get; set; }

    [StringLength(50)]
    public string? Level { get; set; }

    [DataType(DataType.Date)]
    public DateTime ClassDate { get; set; } = DateTime.Today;

    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; } = new(7, 0, 0);

    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; } = new(8, 0, 0);
}
