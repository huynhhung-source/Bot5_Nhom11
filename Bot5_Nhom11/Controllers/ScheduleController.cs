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
                .Where(c =>
                    c.Status != "Cancelled" &&
                    c.ClassDate.Date == selectedDate &&
                    c.TrainingRoom != null &&
                    c.TrainingRoom.Status == "Active");

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

        [HttpGet]
        public async Task<IActionResult> MySchedule()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/Schedule/MySchedule" });
            }

            var enrollments = await _dbContext.ClassEnrollments
                .Include(e => e.Class)
                    .ThenInclude(c => c.TrainingRoom)
                .Include(e => e.Class)
                    .ThenInclude(c => c.Enrollments)
                .Where(e => e.UserId == userId.Value && e.Status != "Cancelled")
                .OrderBy(e => e.Class.ClassDate)
                .ThenBy(e => e.Class.StartTime)
                .ToListAsync();

            var classIds = enrollments.Select(e => e.ClassId).ToList();
            var checkedInClassIds = await _dbContext.Attendances
                .Include(a => a.Subscription)
                .Where(a => a.Subscription.UserId == userId.Value && classIds.Contains(a.ClassId))
                .Select(a => a.ClassId)
                .Distinct()
                .ToListAsync();

            var items = enrollments
                .Select(e => MapMyScheduleItem(
                    e,
                    e.Status == "CheckedIn" || checkedInClassIds.Contains(e.ClassId)))
                .ToList();

            var now = DateTime.Now;
            var model = new MyWorkoutScheduleViewModel
            {
                UpcomingClasses = items
                    .Where(i => i.ClassDate.Date > now.Date || (i.ClassDate.Date == now.Date && i.EndTime >= now.TimeOfDay))
                    .OrderBy(i => i.ClassDate)
                    .ThenBy(i => i.StartTime)
                    .ToList(),
                PastClasses = items
                    .Where(i => i.ClassDate.Date < now.Date || (i.ClassDate.Date == now.Date && i.EndTime < now.TimeOfDay))
                    .OrderByDescending(i => i.ClassDate)
                    .ThenByDescending(i => i.StartTime)
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new
                {
                    area = "Customer",
                    returnUrl = Url.Action(nameof(Details), new { id })
                });
            }

            var enrollment = await _dbContext.ClassEnrollments
                .Include(e => e.Class)
                    .ThenInclude(c => c.TrainingRoom)
                .Include(e => e.Class)
                    .ThenInclude(c => c.Enrollments)
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId.Value &&
                    e.ClassId == id &&
                    e.Status != "Cancelled");

            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy giờ tập đã đăng ký.";
                return Redirect("/Schedule/MySchedule");
            }

            var hasLegacyAttendance = await _dbContext.Attendances
                .AnyAsync(a => a.ClassId == id && a.Subscription.UserId == userId.Value);
            var hasCheckedIn = enrollment.Status == "CheckedIn" || hasLegacyAttendance;

            return View(new WorkoutScheduleDetailsViewModel
            {
                Schedule = MapMyScheduleItem(enrollment, hasCheckedIn),
                Description = enrollment.Class.Description ?? "Chưa có mô tả cho giờ tập này."
            });
        }

        [HttpGet]
        public async Task<IActionResult> BookRoom(int roomId, string? returnUrl)
        {
            var safeReturnUrl = !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : $"/Home/Gyms#gym-room-{roomId}";
            var resumeUrl = Url.Action(nameof(BookRoom), "Schedule", new
            {
                area = "",
                roomId,
                returnUrl = safeReturnUrl
            }) ?? safeReturnUrl;

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new
                {
                    area = "Customer",
                    returnUrl = resumeUrl
                });
            }

            var roomExists = await _dbContext.TrainingRooms
                .AsNoTracking()
                .AnyAsync(room => room.TrainingRoomId == roomId && room.Status == "Active");
            if (!roomExists)
            {
                TempData["ErrorMessage"] = "Phòng tập không tồn tại hoặc hiện không hoạt động.";
                return Redirect(safeReturnUrl);
            }

            var now = DateTime.Now;
            var classId = await _dbContext.Classes
                .AsNoTracking()
                .Where(classItem =>
                    classItem.TrainingRoomId == roomId &&
                    classItem.Status != "Cancelled" &&
                    classItem.Status != "Completed" &&
                    (classItem.ClassDate.Date > now.Date ||
                        (classItem.ClassDate.Date == now.Date && classItem.StartTime > now.TimeOfDay)))
                .OrderBy(classItem => classItem.ClassDate)
                .ThenBy(classItem => classItem.StartTime)
                .Select(classItem => (int?)classItem.ClassId)
                .FirstOrDefaultAsync();

            if (!classId.HasValue)
            {
                TempData["ErrorMessage"] = "Phòng tập này chưa có lịch mở đăng ký. Vui lòng chọn lịch khác hoặc liên hệ quản trị viên.";
                return RedirectToAction(nameof(Index), new
                {
                    area = "",
                    roomId,
                    date = DateTime.Today.ToString("yyyy-MM-dd")
                });
            }

            return RedirectToAction("ClassCheckout", "Payment", new
            {
                area = "",
                classId = classId.Value
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(
            int classId,
            DateTime date,
            int? roomId,
            string? trainer,
            string? returnUrl)
        {
            var safeReturnUrl = GetSafeReturnUrl(returnUrl, date, roomId, trainer);
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = safeReturnUrl });
            }

            var classItem = await _dbContext.Classes
                .Include(c => c.TrainingRoom)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (classItem == null || classItem.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Lịch tập không tồn tại hoặc đã bị hủy.";
                return Redirect(safeReturnUrl);
            }

            if (classItem.TrainingRoom == null || classItem.TrainingRoom.Status != "Active")
            {
                TempData["ErrorMessage"] = "Phòng tập của giờ tập này hiện không hoạt động.";
                return Redirect(safeReturnUrl);
            }

            if (!IsClassRegistrationOpen(classItem))
            {
                TempData["ErrorMessage"] = "Giờ tập hiện không mở đăng ký.";
                return Redirect(safeReturnUrl);
            }

            var alreadyPaidOrBooked = await _dbContext.ClassEnrollments
                .AnyAsync(e => e.UserId == userId.Value && e.ClassId == classId && e.Status != "Cancelled");
            if (alreadyPaidOrBooked)
            {
                TempData["ErrorMessage"] = "Bạn đã đặt giờ tập này rồi.";
                return Redirect(safeReturnUrl);
            }

            if (await HasScheduleConflictAsync(userId.Value, classItem))
            {
                TempData["ErrorMessage"] = "Bạn đã có giờ tập khác trùng thời gian.";
                return Redirect(safeReturnUrl);
            }

            var availableCount = classItem.Enrollments?.Count(e => e.Status != "Cancelled") ?? classItem.CurrentEnrollment;
            if (availableCount >= classItem.MaxCapacity)
            {
                TempData["ErrorMessage"] = "Giờ tập đã đủ chỗ.";
                return Redirect(safeReturnUrl);
            }

            return RedirectToAction("ClassCheckout", "Payment", new { area = "", classId });

#if false
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
#endif
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int classId, DateTime date, int? roomId, string? trainer, string? returnTo)
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
                return returnTo == nameof(MySchedule)
                    ? Redirect("/Schedule/MySchedule")
                    : RedirectToAction(nameof(Index), new { date, roomId, trainer });
            }

            if (booking.Status == "CheckedIn")
            {
                TempData["ErrorMessage"] = "Giờ tập đã check-in nên không thể hủy.";
                return Redirect("/Schedule/MySchedule");
            }

            var classStart = booking.Class.ClassDate.Date.Add(booking.Class.StartTime);
            if (classStart <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Đã đến giờ tập nên không thể hủy lịch.";
                return Redirect("/Schedule/MySchedule");
            }

            booking.Status = "Cancelled";
            if (booking.Class.CurrentEnrollment > 0)
            {
                booking.Class.CurrentEnrollment -= 1;
            }

            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã hủy lịch tập.";
            if (returnTo == nameof(MySchedule))
            {
                return Redirect("/Schedule/MySchedule");
            }

            return RedirectToAction(nameof(Index), new { date, roomId, trainer });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int classId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/Schedule/MySchedule" });
            }

            var booking = await _dbContext.ClassEnrollments
                .Include(e => e.Class)
                .FirstOrDefaultAsync(e => e.UserId == userId.Value && e.ClassId == classId && e.Status != "Cancelled");

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa đăng ký giờ tập này.";
                return Redirect("/Schedule/MySchedule");
            }

            if (booking.Class.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Giờ tập đã bị hủy, không thể check-in.";
                return Redirect("/Schedule/MySchedule");
            }

            if (booking.Class.ClassDate.Date > DateTime.Today)
            {
                booking.Status = "CheckedIn";
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Check-in {booking.Class.ClassName} thành công.";
                return Redirect("/Schedule/MySchedule");
            }

            if (booking.Class.ClassDate.Date != DateTime.Today)
            {
                TempData["ErrorMessage"] = "Bạn chỉ có thể check-in trong đúng ngày tập.";
                return Redirect("/Schedule/MySchedule");
            }

            if (booking.Status == "CheckedIn")
            {
                TempData["ErrorMessage"] = "Bạn đã check-in giờ tập này rồi.";
                return Redirect("/Schedule/MySchedule");
            }

            var classStart = booking.Class.ClassDate.Date.Add(booking.Class.StartTime);
            var classEnd = booking.Class.ClassDate.Date.Add(booking.Class.EndTime);
            var checkInOpens = classStart.AddMinutes(-30);
            if (DateTime.Now < checkInOpens || DateTime.Now > classEnd)
            {
                TempData["ErrorMessage"] = $"Bạn chỉ có thể check-in từ {checkInOpens:HH:mm} đến {classEnd:HH:mm}.";
                return Redirect("/Schedule/MySchedule");
            }

            booking.Status = "CheckedIn";
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Check-in {booking.Class.ClassName} thành công.";
            return Redirect("/Schedule/MySchedule");
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

        private static MyWorkoutScheduleItemViewModel MapMyScheduleItem(ClassEnrollment enrollment, bool hasCheckedIn)
        {
            var classItem = enrollment.Class;
            var registeredCount = classItem.Enrollments?.Count(e => e.Status != "Cancelled") ?? classItem.CurrentEnrollment;
            var now = DateTime.Now;

            return new MyWorkoutScheduleItemViewModel
            {
                EnrollmentId = enrollment.EnrollmentId,
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
                Status = classItem.Status,
                HasCheckedIn = hasCheckedIn,
                EnrollmentDate = enrollment.EnrollmentDate,
                EnrollmentStatus = enrollment.Status,
                CanCancel = !hasCheckedIn &&
                    classItem.ClassDate.Date.Add(classItem.StartTime) > now &&
                    classItem.Status != "Cancelled",
                CanCheckIn = !hasCheckedIn &&
                    now <= classItem.ClassDate.Date.Add(classItem.EndTime) &&
                    classItem.Status != "Cancelled"
            };
        }

        internal async Task<bool> HasScheduleConflictAsync(int userId, Class classItem)
        {
            return await _dbContext.ClassEnrollments
                .Include(e => e.Class)
                .AnyAsync(e =>
                    e.UserId == userId &&
                    e.Status != "Cancelled" &&
                    e.ClassId != classItem.ClassId &&
                    e.Class.ClassDate.Date == classItem.ClassDate.Date &&
                    classItem.StartTime < e.Class.EndTime &&
                    classItem.EndTime > e.Class.StartTime);
        }

        internal static bool IsClassRegistrationOpen(Class classItem)
        {
            return classItem.Status != "Cancelled" &&
                classItem.Status != "Completed" &&
                (classItem.ClassDate.Date > DateTime.Today ||
                    (classItem.ClassDate.Date == DateTime.Today && classItem.StartTime > DateTime.Now.TimeOfDay));
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

        private static string BuildScheduleReturnUrl(DateTime date, int? roomId, string? trainer)
        {
            var selectedDate = date == default ? DateTime.Today : date.Date;
            var returnUrl = $"/Schedule?date={selectedDate:yyyy-MM-dd}";

            if (roomId.HasValue)
            {
                returnUrl += $"&roomId={roomId.Value}";
            }

            if (!string.IsNullOrWhiteSpace(trainer))
            {
                returnUrl += $"&trainer={Uri.EscapeDataString(trainer)}";
            }

            return returnUrl;
        }

        private string GetSafeReturnUrl(string? returnUrl, DateTime date, int? roomId, string? trainer)
        {
            return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : BuildScheduleReturnUrl(date, roomId, trainer);
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
