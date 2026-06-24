using doanweb.Data;
using doanweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookingController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<BookingController> _logger;

        public BookingController(GymDbContext dbContext, ILogger<BookingController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return !string.IsNullOrEmpty(userRole) && userRole == "Admin";
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search = "", string status = "all")
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/admin/booking/index" });
            }

            status = NormalizeFilter(status);
            search = search?.Trim() ?? string.Empty;

            var bookings = await _dbContext.ClassEnrollments
                .Include(e => e.User)
                .Include(e => e.Class)
                .OrderBy(e => e.Class.ClassDate)
                .ThenBy(e => e.Class.StartTime)
                .ThenByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            var mappedBookings = bookings.Select(MapBooking).ToList();
            var filteredBookings = mappedBookings.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                filteredBookings = filteredBookings.Where(b =>
                    ContainsIgnoreCase(b.BookingCode, search) ||
                    ContainsIgnoreCase(b.MemberName, search) ||
                    ContainsIgnoreCase(b.MemberPhone, search) ||
                    ContainsIgnoreCase(b.MemberEmail, search) ||
                    ContainsIgnoreCase(b.ClassName, search) ||
                    ContainsIgnoreCase(b.Room, search) ||
                    ContainsIgnoreCase(b.TrainerName, search));
            }

            if (status != "all")
            {
                filteredBookings = filteredBookings.Where(b => b.StatusKind == status);
            }

            var filteredList = filteredBookings.ToList();

            var viewModel = new AdminBookingIndexViewModel
            {
                Bookings = filteredList,
                SearchTerm = search,
                StatusFilter = status,
                TotalCount = mappedBookings.Count,
                FilteredCount = filteredList.Count,
                ConfirmedCount = mappedBookings.Count(b => b.StatusKind == "confirmed"),
                PendingCount = mappedBookings.Count(b => b.StatusKind == "pending"),
                CancelledCount = mappedBookings.Count(b => b.StatusKind == "cancelled")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id, string search = "", string status = "all")
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var booking = await _dbContext.ClassEnrollments
                .Include(e => e.Class)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (booking == null)
            {
                return NotFound();
            }

            var wasConfirmed = NormalizeStatusKind(booking.Status) == "confirmed";
            if (!wasConfirmed && booking.Class.CurrentEnrollment >= booking.Class.MaxCapacity)
            {
                TempData["ErrorMessage"] = "Lớp học đã đủ chỗ, không thể xác nhận thêm lịch.";
                return RedirectToIndex(search, status);
            }

            booking.Status = "Enrolled";
            if (!wasConfirmed)
            {
                booking.Class.CurrentEnrollment += 1;
            }

            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xác nhận lịch {BuildBookingCode(booking.EnrollmentId)}.";
            return RedirectToIndex(search, status);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string search = "", string status = "all")
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var booking = await _dbContext.ClassEnrollments
                .Include(e => e.Class)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (booking == null)
            {
                return NotFound();
            }

            var wasConfirmed = NormalizeStatusKind(booking.Status) == "confirmed";
            booking.Status = "Cancelled";

            if (wasConfirmed && booking.Class.CurrentEnrollment > 0)
            {
                booking.Class.CurrentEnrollment -= 1;
            }

            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã hủy lịch {BuildBookingCode(booking.EnrollmentId)}.";
            return RedirectToIndex(search, status);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string search = "", string status = "all")
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var booking = await _dbContext.ClassEnrollments
                .Include(e => e.Class)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (booking == null)
            {
                return NotFound();
            }

            var wasConfirmed = NormalizeStatusKind(booking.Status) == "confirmed";
            if (wasConfirmed && booking.Class.CurrentEnrollment > 0)
            {
                booking.Class.CurrentEnrollment -= 1;
            }

            _dbContext.ClassEnrollments.Remove(booking);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã xóa lịch {BuildBookingCode(id)}.";
            return RedirectToIndex(search, status);
        }

        private RedirectToActionResult RedirectToIndex(string search, string status)
        {
            return RedirectToAction(nameof(Index), new { search, status = NormalizeFilter(status) });
        }

        private static AdminBookingItemViewModel MapBooking(ClassEnrollment booking)
        {
            var statusKind = NormalizeStatusKind(booking.Status);

            return new AdminBookingItemViewModel
            {
                EnrollmentId = booking.EnrollmentId,
                BookingCode = BuildBookingCode(booking.EnrollmentId),
                MemberName = booking.User?.FullName ?? "Chưa có hội viên",
                MemberPhone = booking.User?.PhoneNumber ?? "Chưa có SĐT",
                MemberEmail = booking.User?.Email ?? string.Empty,
                ClassName = booking.Class?.ClassName ?? "Chưa có lớp",
                Room = booking.Class?.Location ?? "Chưa có phòng",
                TrainerName = booking.Class?.InstructorName ?? "Chưa phân công",
                ClassDate = booking.Class?.ClassDate ?? booking.EnrollmentDate,
                StartTime = booking.Class?.StartTime ?? TimeSpan.Zero,
                EndTime = booking.Class?.EndTime ?? TimeSpan.Zero,
                EnrollmentDate = booking.EnrollmentDate,
                Status = booking.Status,
                StatusKind = statusKind,
                StatusLabel = GetStatusLabel(statusKind),
                CanConfirm = statusKind != "confirmed",
                CanCancel = statusKind != "cancelled"
            };
        }

        private static string NormalizeFilter(string? status)
        {
            var normalized = (status ?? "all").Trim().ToLowerInvariant();
            return normalized is "confirmed" or "pending" or "cancelled" ? normalized : "all";
        }

        private static string NormalizeStatusKind(string? status)
        {
            var normalized = (status ?? string.Empty).Trim().ToLowerInvariant();

            if (normalized is "pending" or "waiting" or "cho duyet" or "chờ duyệt")
            {
                return "pending";
            }

            if (normalized is "cancelled" or "canceled" or "da huy" or "đã hủy" or "huy" or "hủy")
            {
                return "cancelled";
            }

            return "confirmed";
        }

        private static string GetStatusLabel(string statusKind)
        {
            return statusKind switch
            {
                "pending" => "Chờ duyệt",
                "cancelled" => "Đã hủy",
                _ => "Xác nhận"
            };
        }

        private static bool ContainsIgnoreCase(string? value, string term)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                value.Contains(term, StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildBookingCode(int id)
        {
            return $"BK{id:0000}";
        }
    }
}
