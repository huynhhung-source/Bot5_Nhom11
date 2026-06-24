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
        public bool HasCheckedIn { get; set; }
        public int AvailableSlots => Math.Max(0, Capacity - RegisteredCount);
        public int DurationMinutes => Math.Max(1, (int)(EndTime - StartTime).TotalMinutes);
        public bool IsFull => AvailableSlots <= 0;
        public bool IsRegistrationOpen => Status != "Cancelled" &&
            Status != "Completed" &&
            (ClassDate.Date > DateTime.Today ||
                (ClassDate.Date == DateTime.Today && StartTime > DateTime.Now.TimeOfDay));
    }

    public class MyWorkoutScheduleViewModel
    {
        public List<MyWorkoutScheduleItemViewModel> UpcomingClasses { get; set; } = new();
        public List<MyWorkoutScheduleItemViewModel> PastClasses { get; set; } = new();
    }

    public class MyWorkoutScheduleItemViewModel : WorkoutClassCardViewModel
    {
        public int EnrollmentId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string EnrollmentStatus { get; set; } = string.Empty;
        public bool CanCancel { get; set; }
        public bool CanCheckIn { get; set; }
    }

    public class WorkoutScheduleDetailsViewModel
    {
        public MyWorkoutScheduleItemViewModel Schedule { get; set; } = new();
        public string Description { get; set; } = string.Empty;
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

        [Required(ErrorMessage = "Vui lòng nhập số lượng người tối đa.")]
        [Range(1, 500, ErrorMessage = "Số lượng tối đa phải từ 1 đến 500.")]
        public int MaxCapacity { get; set; } = 20;

        [StringLength(50)]
        public string Status { get; set; } = "Scheduled";
    }

    public class AdminScheduleDetailsViewModel
    {
        public WorkoutClassCardViewModel ClassInfo { get; set; } = new();
        public string Description { get; set; } = string.Empty;
        public List<AdminScheduleEnrollmentViewModel> Enrollments { get; set; } = new();
        public int CheckedInCount => Enrollments.Count(e => e.HasCheckedIn);
    }

    public class AdminScheduleEnrollmentViewModel
    {
        public int EnrollmentId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string MemberPhone { get; set; } = string.Empty;
        public string MemberEmail { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool HasCheckedIn { get; set; }
        public DateTime? CheckInDate { get; set; }
        public TimeSpan? CheckInTime { get; set; }
    }
}
