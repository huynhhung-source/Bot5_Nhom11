namespace doanweb.Models
{
    public class AdminReportViewModel
    {
        public string PeriodType { get; set; } = "month";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalCustomers { get; set; }
        public int ActiveMembers { get; set; }
        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }
        public int CancelledBookingCount { get; set; }
        public string TopClassName { get; set; } = "Chưa có dữ liệu";
        public int TopClassBookings { get; set; }
        public string TopRoomName { get; set; } = "Chưa có dữ liệu";
        public int TopRoomBookings { get; set; }
        public List<ReportSeriesItem> RevenueSeries { get; set; } = new();
        public List<ReportRankItem> TopClasses { get; set; } = new();
        public List<ReportRankItem> TopRooms { get; set; } = new();
        public List<TrainerPerformanceItem> TrainerPerformance { get; set; } = new();
    }

    public class ReportSeriesItem
    {
        public string Label { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int Bookings { get; set; }
        public int Cancellations { get; set; }
    }

    public class ReportRankItem
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Rate { get; set; }
    }

    public class TrainerPerformanceItem
    {
        public string TrainerName { get; set; } = string.Empty;
        public int ClassCount { get; set; }
        public int BookingCount { get; set; }
        public int CancelledCount { get; set; }
        public int Capacity { get; set; }
        public decimal FillRate { get; set; }
    }
}
