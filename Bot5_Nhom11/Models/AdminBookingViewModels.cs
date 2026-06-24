namespace doanweb.Models
{
    public class AdminBookingIndexViewModel
    {
        public List<AdminBookingItemViewModel> Bookings { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = "all";
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int PendingCount { get; set; }
        public int CancelledCount { get; set; }
    }

    public class AdminBookingItemViewModel
    {
        public int EnrollmentId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public string MemberPhone { get; set; } = string.Empty;
        public string MemberEmail { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string TrainerName { get; set; } = string.Empty;
        public DateTime ClassDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusKind { get; set; } = "confirmed";
        public string StatusLabel { get; set; } = "Xác nhận";
        public bool CanConfirm { get; set; }
        public bool CanCancel { get; set; }
    }
}
