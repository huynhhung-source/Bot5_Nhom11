namespace doanweb.Models
{
    public class AdminPackageSubscriptionViewModel
    {
        public int SubscriptionId { get; set; }
        public int PackageId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int RemainingDays { get; set; }
        public int SessionsUsed { get; set; }
        public int MaxSessions { get; set; }
    }
}
