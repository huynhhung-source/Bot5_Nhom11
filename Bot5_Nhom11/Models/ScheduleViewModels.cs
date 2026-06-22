using System.ComponentModel.DataAnnotations;

namespace doanweb.Models
{
    public class WorkoutScheduleIndexViewModel
    {
        public DateTime SelectedDate { get; set; }
        public int? RoomId { get; set; }
        public string Trainer { get; set; } = string.Empty;
        public List<WorkoutClassCardViewModel> Classes { get; set; } = new();
        public List<TrainingRoom> Rooms { get; set; } = new();
        public List<string> Trainers { get; set; } = new();
        public List<int> MyBookedClassIds { get; set; } = new();
    }

    public class WorkoutClassCardViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string ClassType { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public DateTime ClassDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Capacity { get; set; }
        public int RegisteredCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public int AvailableSlots => Math.Max(0, Capacity - RegisteredCount);
        public int DurationMinutes => Math.Max(1, (int)(EndTime - StartTime).TotalMinutes);
        public bool IsFull => AvailableSlots <= 0;
    }

    public class AdminScheduleIndexViewModel
    {
        public DateTime SelectedDate { get; set; }
        public List<WorkoutClassCardViewModel> Classes { get; set; } = new();
        public int TotalClasses { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalCapacity { get; set; }
        public int FullClasses { get; set; }
    }

    public class AdminScheduleFormViewModel
    {
        public int ClassId { get; set; }

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

        [Required(ErrorMessage = "Vui lòng chọn ngày học.")]
        [DataType(DataType.Date)]
        public DateTime ClassDate { get; set; } = DateTime.Today;

        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; } = new(6, 0, 0);

        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; } = new(7, 0, 0);

        [Required(ErrorMessage = "Vui lòng chọn phòng.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn phòng.")]
        public int TrainingRoomId { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Scheduled";
    }
}
