using doanweb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ApiReportsController : ControllerBase
{
    private readonly GymDbContext _dbContext;

    public ApiReportsController(GymDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetReport(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        if (!string.Equals(HttpContext.Session.GetString("UserRole"), "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = "Bạn không có quyền xem báo cáo."
            });
        }

        var today = DateTime.Today;
        var start = (startDate ?? new DateTime(today.Year, today.Month, 1)).Date;
        var end = (endDate ?? today).Date;

        if (end < start)
        {
            return BadRequest(new { message = "endDate phải lớn hơn hoặc bằng startDate." });
        }

        var endExclusive = end.AddDays(1);

        var totalCustomers = await _dbContext.Users
            .AsNoTracking()
            .CountAsync(u => u.CreatedDate < endExclusive);

        var activeMembers = await _dbContext.Subscriptions
            .AsNoTracking()
            .Where(s =>
                s.Status == "Active" &&
                s.StartDate < endExclusive &&
                s.EndDate >= start)
            .Select(s => s.UserId)
            .Distinct()
            .CountAsync();

        var revenue = await _dbContext.Payments
            .AsNoTracking()
            .Where(p =>
                p.Status == "Success" &&
                p.PaymentDate >= start &&
                p.PaymentDate < endExclusive)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

        var enrollments = await _dbContext.ClassEnrollments
            .AsNoTracking()
            .Where(e => e.EnrollmentDate >= start && e.EnrollmentDate < endExclusive)
            .Select(e => new
            {
                e.Status,
                ClassName = e.Class.ClassName,
                RoomName = e.Class.TrainingRoom != null
                    ? e.Class.TrainingRoom.RoomName
                    : e.Class.Location
            })
            .ToListAsync();

        var bookingCount = enrollments.Count(e => e.Status != "Cancelled");
        var cancelledBookingCount = enrollments.Count(e => e.Status == "Cancelled");

        var topClasses = enrollments
            .Where(e => e.Status != "Cancelled")
            .GroupBy(e => e.ClassName)
            .Select(group => new
            {
                name = group.Key,
                bookings = group.Count()
            })
            .OrderByDescending(item => item.bookings)
            .ThenBy(item => item.name)
            .Take(5)
            .ToList();

        var topRooms = enrollments
            .Where(e => e.Status != "Cancelled")
            .GroupBy(e => string.IsNullOrWhiteSpace(e.RoomName) ? "Chưa có phòng" : e.RoomName)
            .Select(group => new
            {
                name = group.Key,
                bookings = group.Count()
            })
            .OrderByDescending(item => item.bookings)
            .ThenBy(item => item.name)
            .Take(5)
            .ToList();

        return Ok(new
        {
            startDate = start,
            endDate = end,
            summary = new
            {
                totalCustomers,
                activeMembers,
                revenue,
                bookingCount,
                cancelledBookingCount
            },
            topClasses,
            topRooms
        });
    }
}
