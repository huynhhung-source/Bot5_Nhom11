using System.Text;
using doanweb.Data;
using doanweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportController : Controller
    {
        private readonly GymDbContext _dbContext;

        public ReportController(GymDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return !string.IsNullOrEmpty(userRole) && userRole == "Admin";
        }

        [HttpGet]
        public async Task<IActionResult> Index(string periodType = "month", DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/admin/report/index" });
            }

            var range = ResolveRange(periodType, startDate, endDate);
            var model = await BuildReportAsync(range.PeriodType, range.StartDate, range.EndDate);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Export(string periodType = "month", DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/admin/report/index" });
            }

            var range = ResolveRange(periodType, startDate, endDate);
            var model = await BuildReportAsync(range.PeriodType, range.StartDate, range.EndDate);
            var csv = new StringBuilder();
            csv.AppendLine("Bao cao thong ke");
            csv.AppendLine($"Tu ngay,{model.StartDate:dd/MM/yyyy}");
            csv.AppendLine($"Den ngay,{model.EndDate:dd/MM/yyyy}");
            csv.AppendLine();
            csv.AppendLine("Chi so,Gia tri");
            csv.AppendLine($"Tong khach hang,{model.TotalCustomers}");
            csv.AppendLine($"Hoi vien dang hoat dong,{model.ActiveMembers}");
            csv.AppendLine($"Doanh thu,{model.Revenue}");
            csv.AppendLine($"Luot dat lich,{model.BookingCount}");
            csv.AppendLine($"Luot huy lich,{model.CancelledBookingCount}");
            csv.AppendLine($"Lop dang ky nhieu nhat,{EscapeCsv(model.TopClassName)} ({model.TopClassBookings})");
            csv.AppendLine($"Phong su dung nhieu nhat,{EscapeCsv(model.TopRoomName)} ({model.TopRoomBookings})");
            csv.AppendLine();
            csv.AppendLine("Huan luyen vien,So lop,Luot dat,Luot huy,Suc chua,Hieu suat");
            foreach (var item in model.TrainerPerformance)
            {
                csv.AppendLine($"{EscapeCsv(item.TrainerName)},{item.ClassCount},{item.BookingCount},{item.CancelledCount},{item.Capacity},{item.FillRate:0.##}%");
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            var fileName = $"bao-cao-{model.StartDate:yyyyMMdd}-{model.EndDate:yyyyMMdd}.csv";
            return File(bytes, "text/csv", fileName);
        }

        private async Task<AdminReportViewModel> BuildReportAsync(string periodType, DateTime startDate, DateTime endDate)
        {
            var endExclusive = endDate.Date.AddDays(1);
            var totalCustomers = await _dbContext.Users.CountAsync(u => u.CreatedDate < endExclusive);
            var activeMembers = await _dbContext.Subscriptions
                .Where(s => s.Status == "Active" && s.StartDate < endExclusive && s.EndDate.Date >= startDate.Date)
                .Select(s => s.UserId)
                .Distinct()
                .CountAsync();
            var revenue = await _dbContext.Payments
                .Where(p => p.Status == "Success" && p.PaymentDate >= startDate && p.PaymentDate < endExclusive)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            var enrollments = await _dbContext.ClassEnrollments
                .Include(e => e.Class)
                    .ThenInclude(c => c.TrainingRoom)
                .Where(e => e.EnrollmentDate >= startDate && e.EnrollmentDate < endExclusive)
                .ToListAsync();

            var classes = await _dbContext.Classes
                .Include(c => c.TrainingRoom)
                .Include(c => c.Enrollments)
                .Where(c => c.ClassDate >= startDate && c.ClassDate < endExclusive && c.Status != "Cancelled")
                .ToListAsync();

            var bookingCount = enrollments.Count(e => e.Status != "Cancelled");
            var cancelledCount = enrollments.Count(e => e.Status == "Cancelled");

            var topClasses = enrollments
                .GroupBy(e => e.Class?.ClassName ?? "Chưa có lớp")
                .Select(g => new ReportRankItem
                {
                    Name = g.Key,
                    Count = g.Count(e => e.Status != "Cancelled"),
                    Rate = bookingCount > 0 ? g.Count(e => e.Status != "Cancelled") * 100m / bookingCount : 0m
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Name)
                .Take(5)
                .ToList();

            var topRooms = enrollments
                .GroupBy(e => e.Class?.TrainingRoom?.RoomName ?? e.Class?.Location ?? "Chưa có phòng")
                .Select(g => new ReportRankItem
                {
                    Name = g.Key,
                    Count = g.Count(e => e.Status != "Cancelled"),
                    Rate = bookingCount > 0 ? g.Count(e => e.Status != "Cancelled") * 100m / bookingCount : 0m
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Name)
                .Take(5)
                .ToList();

            var trainerPerformance = classes
                .GroupBy(c => c.InstructorName)
                .Select(g =>
                {
                    var capacity = g.Sum(c => c.MaxCapacity);
                    var trainerBookings = g.Sum(c => c.Enrollments?.Count(e => e.Status != "Cancelled") ?? c.CurrentEnrollment);
                    var trainerCancelled = g.Sum(c => c.Enrollments?.Count(e => e.Status == "Cancelled") ?? 0);
                    return new TrainerPerformanceItem
                    {
                        TrainerName = g.Key,
                        ClassCount = g.Count(),
                        BookingCount = trainerBookings,
                        CancelledCount = trainerCancelled,
                        Capacity = capacity,
                        FillRate = capacity > 0 ? trainerBookings * 100m / capacity : 0m
                    };
                })
                .OrderByDescending(x => x.FillRate)
                .ThenByDescending(x => x.BookingCount)
                .Take(8)
                .ToList();

            return new AdminReportViewModel
            {
                PeriodType = periodType,
                StartDate = startDate,
                EndDate = endDate,
                TotalCustomers = totalCustomers,
                ActiveMembers = activeMembers,
                Revenue = revenue,
                BookingCount = bookingCount,
                CancelledBookingCount = cancelledCount,
                TopClassName = topClasses.FirstOrDefault()?.Name ?? "Chưa có dữ liệu",
                TopClassBookings = topClasses.FirstOrDefault()?.Count ?? 0,
                TopRoomName = topRooms.FirstOrDefault()?.Name ?? "Chưa có dữ liệu",
                TopRoomBookings = topRooms.FirstOrDefault()?.Count ?? 0,
                RevenueSeries = await BuildSeriesAsync(periodType, startDate, endDate),
                TopClasses = topClasses,
                TopRooms = topRooms,
                TrainerPerformance = trainerPerformance
            };
        }

        private async Task<List<ReportSeriesItem>> BuildSeriesAsync(string periodType, DateTime startDate, DateTime endDate)
        {
            var items = new List<ReportSeriesItem>();
            if (periodType == "year")
            {
                for (var month = 1; month <= 12; month++)
                {
                    var current = new DateTime(startDate.Year, month, 1);
                    items.Add(await BuildSeriesItemAsync($"T{month}", current, current.AddMonths(1)));
                }
                return items;
            }

            var step = periodType == "day" ? TimeSpan.FromHours(4) : TimeSpan.FromDays(1);
            var cursor = startDate.Date;
            while (cursor <= endDate.Date)
            {
                var next = periodType == "day" ? cursor.AddDays(1) : cursor.AddDays(1);
                items.Add(await BuildSeriesItemAsync(periodType == "day" ? cursor.ToString("dd/MM") : cursor.ToString("dd/MM"), cursor, next));
                cursor = cursor.Add(step).Date;
            }

            return items;
        }

        private async Task<ReportSeriesItem> BuildSeriesItemAsync(string label, DateTime start, DateTime endExclusive)
        {
            var revenue = await _dbContext.Payments
                .Where(p => p.Status == "Success" && p.PaymentDate >= start && p.PaymentDate < endExclusive)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;
            var bookingCount = await _dbContext.ClassEnrollments
                .CountAsync(e => e.EnrollmentDate >= start && e.EnrollmentDate < endExclusive && e.Status != "Cancelled");
            var cancellationCount = await _dbContext.ClassEnrollments
                .CountAsync(e => e.EnrollmentDate >= start && e.EnrollmentDate < endExclusive && e.Status == "Cancelled");

            return new ReportSeriesItem
            {
                Label = label,
                Revenue = revenue,
                Bookings = bookingCount,
                Cancellations = cancellationCount
            };
        }

        private static (string PeriodType, DateTime StartDate, DateTime EndDate) ResolveRange(string periodType, DateTime? startDate, DateTime? endDate)
        {
            periodType = periodType?.ToLowerInvariant() switch
            {
                "day" => "day",
                "year" => "year",
                _ => "month"
            };

            var today = DateTime.Today;
            var start = startDate?.Date;
            var end = endDate?.Date;

            if (!start.HasValue || !end.HasValue)
            {
                if (periodType == "day")
                {
                    start = today;
                    end = today;
                }
                else if (periodType == "year")
                {
                    start = new DateTime(today.Year, 1, 1);
                    end = new DateTime(today.Year, 12, 31);
                }
                else
                {
                    start = new DateTime(today.Year, today.Month, 1);
                    end = start.Value.AddMonths(1).AddDays(-1);
                }
            }

            if (end.Value < start.Value)
            {
                (start, end) = (end, start);
            }

            return (periodType, start.Value, end.Value);
        }

        private static string EscapeCsv(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}
