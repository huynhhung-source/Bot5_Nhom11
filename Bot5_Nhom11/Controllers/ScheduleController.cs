using doanweb.Data;
using doanweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly GymDbContext _dbContext;

        public ScheduleController(GymDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date, int? roomId, string? trainer)
        {
            await EnsureDefaultSchedulesAsync();

            var selectedDate = (date ?? DateTime.Today).Date;
            trainer = trainer?.Trim() ?? string.Empty;

            var query = _dbContext.Classes
                .Include(c => c.TrainingRoom)
                .Include(c => c.Enrollments)
                .AsNoTracking()
                .Where(c => c.Status != "Cancelled" && c.ClassDate.Date == selectedDate);

            if (roomId.HasValue)
            {
                query = query.Where(c => c.TrainingRoomId == roomId.Value);
            }

            if (!string.IsNullOrWhiteSpace(trainer))
            {
                query = query.Where(c => c.InstructorName == trainer);
            }

            var classes = await query
                .OrderBy(c => c.StartTime)
                .ToListAsync();

            var userId = HttpContext.Session.GetInt32("UserId");
            var myBookedClassIds = userId.HasValue
                ? await _dbContext.ClassEnrollments
                    .Where(e => e.UserId == userId.Value && e.Status != "Cancelled")
                    .Select(e => e.ClassId)
                    .ToListAsync()
                : new List<int>();

            var model = new WorkoutScheduleIndexViewModel
            {
                SelectedDate = selectedDate,
                RoomId = roomId,
                Trainer = trainer,
                Classes = classes.Select(MapClass).ToList(),
                Rooms = await _dbContext.TrainingRooms.AsNoTracking().Where(r => r.Status == "Active").OrderBy(r => r.RoomName).ToListAsync(),
                Trainers = await _dbContext.Classes.AsNoTracking().Where(c => c.Status != "Cancelled").Select(c => c.InstructorName).Distinct().OrderBy(x => x).ToListAsync(),
                MyBookedClassIds = myBookedClassIds
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int classId, DateTime date, int? roomId, string? trainer)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = Url.Action(nameof(Index), new { date, roomId, trainer }) });
            }

            var classItem = await _dbContext.Classes
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (classItem == null || classItem.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Lịch tập không tồn tại hoặc đã bị hủy.";
                return RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            var subscription = await GetValidSubscriptionAsync(userId.Value, classItem.ClassType);
            if (subscription == null)
            {
                TempData["ErrorMessage"] = "Bạn cần có gói tập còn hiệu lực và phù hợp loại lớp để đặt lịch.";
                return RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            var existingBooking = await _dbContext.ClassEnrollments
                .AnyAsync(e => e.UserId == userId.Value && e.ClassId == classId && e.Status != "Cancelled");
            if (existingBooking)
            {
                TempData["ErrorMessage"] = "Bạn đã đặt lịch tập này rồi.";
                return RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            var registeredCount = classItem.Enrollments?.Count(e => e.Status != "Cancelled") ?? classItem.CurrentEnrollment;
            if (registeredCount >= classItem.MaxCapacity)
            {
                TempData["ErrorMessage"] = "Lớp học đã đủ chỗ.";
                return RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            _dbContext.ClassEnrollments.Add(new ClassEnrollment
            {
                UserId = userId.Value,
                ClassId = classId,
                EnrollmentDate = DateTime.Now,
                Status = "Enrolled"
            });

            classItem.CurrentEnrollment = registeredCount + 1;
            subscription.SessionsUsed += 1;
            subscription.UpdatedDate = DateTime.Now;

            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đặt lịch {classItem.ClassName} thành công.";
            return RedirectToAction(nameof(Index), new { date, roomId, trainer });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int classId, DateTime date, int? roomId, string? trainer)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var booking = await _dbContext.ClassEnrollments
                .Include(e => e.Class)
                .FirstOrDefaultAsync(e => e.UserId == userId.Value && e.ClassId == classId && e.Status != "Cancelled");

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch đã đặt.";
                return RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            var subscription = await GetValidSubscriptionAsync(userId.Value, booking.Class.ClassType, requireRemainingSession: false);
            booking.Status = "Cancelled";
            if (booking.Class.CurrentEnrollment > 0)
            {
                booking.Class.CurrentEnrollment -= 1;
            }

            if (subscription != null && subscription.SessionsUsed > 0)
            {
                subscription.SessionsUsed -= 1;
                subscription.UpdatedDate = DateTime.Now;
            }

            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã hủy lịch tập.";
            return RedirectToAction(nameof(Index), new { date, roomId, trainer });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(int oldClassId, int newClassId, DateTime date, int? roomId, string? trainer)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            if (oldClassId == newClassId)
            {
                TempData["ErrorMessage"] = "Bạn đã đặt lịch tập này rồi.";
                return RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            var oldBooking = await _dbContext.ClassEnrollments
                .Include(e => e.Class)
                .FirstOrDefaultAsync(e => e.UserId == userId.Value && e.ClassId == oldClassId && e.Status != "Cancelled");

            if (oldBooking == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch cũ để đổi.";
                return RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            var newClass = await _dbContext.Classes
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.ClassId == newClassId);

            if (newClass == null || newClass.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Lịch tập mới không tồn tại hoặc đã bị hủy.";
                return RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            var subscription = await GetValidSubscriptionAsync(userId.Value, newClass.ClassType, requireRemainingSession: false);
            if (subscription == null)
            {
                TempData["ErrorMessage"] = "Gói tập của bạn không phù hợp loại lớp mới.";
                return RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            var alreadyBooked = await _dbContext.ClassEnrollments
                .AnyAsync(e => e.UserId == userId.Value && e.ClassId == newClassId && e.Status != "Cancelled");
            if (alreadyBooked)
            {
                TempData["ErrorMessage"] = "Bạn đã đặt lịch tập này rồi.";
                return RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            var registeredCount = newClass.Enrollments?.Count(e => e.Status != "Cancelled") ?? newClass.CurrentEnrollment;
            if (registeredCount >= newClass.MaxCapacity)
            {
                TempData["ErrorMessage"] = "Lớp học mới đã đủ chỗ.";
                return RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            oldBooking.Status = "Cancelled";
            if (oldBooking.Class.CurrentEnrollment > 0)
            {
                oldBooking.Class.CurrentEnrollment -= 1;
            }

            _dbContext.ClassEnrollments.Add(new ClassEnrollment
            {
                UserId = userId.Value,
                ClassId = newClassId,
                EnrollmentDate = DateTime.Now,
                Status = "Enrolled"
            });

            newClass.CurrentEnrollment = registeredCount + 1;

            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã đổi lịch sang {newClass.ClassName}.";
            return RedirectToAction(nameof(Index), new { date, roomId, trainer });
        }

        private async Task<Subscription?> GetValidSubscriptionAsync(int userId, string? classType, bool requireRemainingSession = true)
        {
            var subscriptions = await _dbContext.Subscriptions
                .Include(s => s.Package)
                .Where(s => s.UserId == userId && s.Status == "Active" && s.EndDate.Date >= DateTime.Today)
                .OrderByDescending(s => s.EndDate)
                .ToListAsync();

            return subscriptions.FirstOrDefault(s =>
                (!requireRemainingSession || s.Package.MaxSessions <= 0 || s.SessionsUsed < s.Package.MaxSessions) &&
                IsClassTypeAllowed(s.Package.AllowedClassTypes, classType));
        }

        private static bool IsClassTypeAllowed(string? allowedClassTypes, string? classType)
        {
            if (string.IsNullOrWhiteSpace(allowedClassTypes) || string.IsNullOrWhiteSpace(classType))
            {
                return true;
            }

            return allowedClassTypes
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Any(type => type.Equals(classType, StringComparison.OrdinalIgnoreCase));
        }

        private static WorkoutClassCardViewModel MapClass(Class classItem)
        {
            var registeredCount = classItem.Enrollments?.Count(e => e.Status != "Cancelled") ?? classItem.CurrentEnrollment;
            return new WorkoutClassCardViewModel
            {
                ClassId = classItem.ClassId,
                ClassName = classItem.ClassName,
                ClassType = classItem.ClassType ?? string.Empty,
                InstructorName = classItem.InstructorName,
                RoomName = classItem.TrainingRoom?.RoomName ?? classItem.Location ?? "Chưa có phòng",
                ClassDate = classItem.ClassDate,
                StartTime = classItem.StartTime,
                EndTime = classItem.EndTime,
                Capacity = classItem.MaxCapacity,
                RegisteredCount = registeredCount,
                Status = classItem.Status
            };
        }

        private async Task EnsureDefaultSchedulesAsync()
        {
            if (await _dbContext.Classes.AnyAsync())
            {
                return;
            }

            var rooms = await _dbContext.TrainingRooms.Where(r => r.Status == "Active").OrderBy(r => r.TrainingRoomId).ToListAsync();
            if (!rooms.Any())
            {
                return;
            }

            TrainingRoom PickRoom(string keyword, int fallbackIndex)
            {
                return rooms.FirstOrDefault(r => r.RoomName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ??
                    rooms[Math.Min(fallbackIndex, rooms.Count - 1)];
            }

            var yogaRoom = PickRoom("Yoga", 0);
            var gymRoom = PickRoom("Gym", 0);
            var boxingRoom = PickRoom("Boxing", 0);
            var today = DateTime.Today;

            var defaults = new[]
            {
                new { Name = "Morning Yoga", Type = "Yoga", Trainer = "Nguyễn Thị Mai", Room = yogaRoom, Start = new TimeSpan(6, 0, 0), End = new TimeSpan(7, 0, 0) },
                new { Name = "CrossFit", Type = "Gym", Trainer = "Trần Văn Hùng", Room = gymRoom, Start = new TimeSpan(7, 30, 0), End = new TimeSpan(8, 15, 0) },
                new { Name = "Zumba Dance", Type = "Zumba", Trainer = "Lê Thị Hoa", Room = yogaRoom, Start = new TimeSpan(9, 0, 0), End = new TimeSpan(10, 0, 0) },
                new { Name = "Boxing", Type = "Boxing", Trainer = "Phạm Văn Đức", Room = boxingRoom, Start = new TimeSpan(17, 0, 0), End = new TimeSpan(18, 0, 0) },
                new { Name = "Power Lifting", Type = "Gym", Trainer = "Hoàng Minh", Room = gymRoom, Start = new TimeSpan(18, 30, 0), End = new TimeSpan(20, 0, 0) },
                new { Name = "Yoga Relax", Type = "Yoga", Trainer = "Nguyễn Thị Mai", Room = yogaRoom, Start = new TimeSpan(20, 0, 0), End = new TimeSpan(21, 0, 0) }
            };

            foreach (var item in defaults)
            {
                _dbContext.Classes.Add(new Class
                {
                    TrainingRoomId = item.Room.TrainingRoomId,
                    ClassName = item.Name,
                    ClassType = item.Type,
                    Description = $"Lịch tập mẫu cho lớp {item.Name}.",
                    InstructorName = item.Trainer,
                    Level = "All levels",
                    ClassDate = today,
                    StartTime = item.Start,
                    EndTime = item.End,
                    Location = item.Room.RoomName,
                    MaxCapacity = item.Room.Capacity,
                    CurrentEnrollment = 0,
                    Status = "Scheduled",
                    CreatedDate = DateTime.Now
                });
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
