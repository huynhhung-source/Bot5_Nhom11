using doanweb.Data;
using doanweb.Models;
using doanweb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ScheduleController : Controller
    {
        private static readonly string[] DefaultTrainerNames =
        [
            "Nguyễn Thị Mai",
            "Trần Văn Hùng",
            "Phạm Văn Đức",
            "Lê Thị Hoa",
            "Hoang Minh"
        ];

        private static readonly string[] ClassTypes =
        [
            "Gym",
            "Yoga",
            "Boxing",
            "Pilates",
            "Zumba",
            "Cardio",
            "Strength",
            "HIIT",
            "Personal Training"
        ];

        private static readonly string[] ClassLevels =
        [
            "Beginner",
            "Intermediate",
            "Advanced",
            "All levels"
        ];

        private readonly GymDbContext _dbContext;
        private readonly IStaffDirectoryService _staffDirectoryService;

        public ScheduleController(GymDbContext dbContext, IStaffDirectoryService staffDirectoryService)
        {
            _dbContext = dbContext;
            _staffDirectoryService = staffDirectoryService;
        }

        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return !string.IsNullOrEmpty(userRole) && userRole == "Admin";
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/admin/schedule/index" });
            }

            await EnsureDefaultSchedulesAsync();

            var selectedDate = (date ?? DateTime.Today).Date;
            var classes = await _dbContext.Classes
                .Include(c => c.TrainingRoom)
                .Include(c => c.Enrollments)
                .Where(c => c.ClassDate.Date == selectedDate && c.Status != "Cancelled")
                .OrderBy(c => c.StartTime)
                .ToListAsync();

            var mappedClasses = classes.Select(MapClass).ToList();
            var model = new AdminScheduleIndexViewModel
            {
                SelectedDate = selectedDate,
                Classes = mappedClasses,
                TotalClasses = mappedClasses.Count,
                TotalRegistrations = mappedClasses.Sum(c => c.RegisteredCount),
                TotalCapacity = mappedClasses.Sum(c => c.Capacity),
                FullClasses = mappedClasses.Count(c => c.IsFull)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var classItem = await _dbContext.Classes
                .Include(c => c.TrainingRoom)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(c => c.ClassId == id);

            if (classItem == null)
            {
                return NotFound();
            }

            var attendances = await _dbContext.Attendances
                .Include(a => a.Subscription)
                    .ThenInclude(s => s.User)
                .Where(a => a.ClassId == id)
                .ToListAsync();

            var model = new AdminScheduleDetailsViewModel
            {
                ClassInfo = MapClass(classItem),
                Description = classItem.Description ?? string.Empty,
                Enrollments = (classItem.Enrollments ?? new List<ClassEnrollment>())
                    .OrderBy(e => e.EnrollmentDate)
                    .Select(e =>
                    {
                        var attendance = attendances.FirstOrDefault(a => a.Subscription.UserId == e.UserId);
                        return new AdminScheduleEnrollmentViewModel
                        {
                            EnrollmentId = e.EnrollmentId,
                            MemberName = e.User?.FullName ?? "Chưa có hội viên",
                            MemberPhone = e.User?.PhoneNumber ?? "Chưa có SĐT",
                            MemberEmail = e.User?.Email ?? string.Empty,
                            EnrollmentDate = e.EnrollmentDate,
                            Status = e.Status,
                            HasCheckedIn = attendance != null,
                            CheckInDate = attendance?.AttendanceDate,
                            CheckInTime = attendance?.CheckInTime
                        };
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Book(int classId, DateTime date, int? roomId, string? trainer)
        {
            var returnUrl = BuildPublicScheduleReturnUrl(date, roomId, trainer);

            if (!HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl });
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var suggestedStart = GetSuggestedScheduleStart();
            await LoadRoomsAsync();
            await LoadScheduleOptionsAsync();
            return View(new AdminScheduleFormViewModel
            {
                ClassDate = suggestedStart.Date,
                StartTime = suggestedStart.TimeOfDay,
                EndTime = suggestedStart.AddHours(1).TimeOfDay
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminScheduleFormViewModel model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var room = await ValidateScheduleAsync(model);
            if (!ModelState.IsValid)
            {
                await LoadRoomsAsync(model.TrainingRoomId);
                await LoadScheduleOptionsAsync(model.InstructorName, model.ClassType, model.Level);
                return View(model);
            }

            _dbContext.Classes.Add(new Class
            {
                TrainingRoomId = model.TrainingRoomId,
                ClassName = model.ClassName.Trim(),
                Description = model.Description?.Trim(),
                InstructorName = model.InstructorName.Trim(),
                ClassType = model.ClassType?.Trim(),
                Level = model.Level?.Trim(),
                ClassDate = model.ClassDate.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Location = room!.RoomName,
                MaxCapacity = model.MaxCapacity,
                CurrentEnrollment = 0,
                Status = "Scheduled",
                CreatedDate = DateTime.Now
            });

            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã thêm giờ tập mới.";
            return RedirectToAction(nameof(Index), new { date = model.ClassDate.ToString("yyyy-MM-dd") });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var classItem = await _dbContext.Classes.FindAsync(id);
            if (classItem == null)
            {
                return NotFound();
            }

            await LoadRoomsAsync(classItem.TrainingRoomId);
            await LoadScheduleOptionsAsync(classItem.InstructorName, classItem.ClassType, classItem.Level);
            return View(new AdminScheduleFormViewModel
            {
                ClassId = classItem.ClassId,
                ClassName = classItem.ClassName,
                Description = classItem.Description,
                InstructorName = classItem.InstructorName,
                ClassType = classItem.ClassType,
                Level = classItem.Level,
                ClassDate = classItem.ClassDate,
                StartTime = classItem.StartTime,
                EndTime = classItem.EndTime,
                TrainingRoomId = classItem.TrainingRoomId ?? 0,
                MaxCapacity = classItem.MaxCapacity,
                Status = classItem.Status
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminScheduleFormViewModel model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            if (id != model.ClassId)
            {
                return BadRequest();
            }

            var room = await ValidateScheduleAsync(model, id);
            if (!ModelState.IsValid)
            {
                await LoadRoomsAsync(model.TrainingRoomId);
                await LoadScheduleOptionsAsync(model.InstructorName, model.ClassType, model.Level);
                return View(model);
            }

            var classItem = await _dbContext.Classes
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.ClassId == id);
            if (classItem == null)
            {
                return NotFound();
            }

            var registeredCount = classItem.Enrollments?.Count(e => e.Status != "Cancelled") ?? classItem.CurrentEnrollment;
            if (model.MaxCapacity < registeredCount)
            {
                ModelState.AddModelError(nameof(model.MaxCapacity), $"Số lượng tối đa không được nhỏ hơn {registeredCount} khách đã đăng ký.");
                await LoadRoomsAsync(model.TrainingRoomId);
                await LoadScheduleOptionsAsync(model.InstructorName, model.ClassType, model.Level);
                return View(model);
            }

            classItem.TrainingRoomId = model.TrainingRoomId;
            classItem.ClassName = model.ClassName.Trim();
            classItem.Description = model.Description?.Trim();
            classItem.InstructorName = model.InstructorName.Trim();
            classItem.ClassType = model.ClassType?.Trim();
            classItem.Level = model.Level?.Trim();
            classItem.ClassDate = model.ClassDate.Date;
            classItem.StartTime = model.StartTime;
            classItem.EndTime = model.EndTime;
            classItem.Location = room.RoomName;
            classItem.MaxCapacity = model.MaxCapacity;
            var scheduledStart = model.ClassDate.Date.Add(model.StartTime);
            classItem.Status = model.Status == "Cancelled"
                ? "Cancelled"
                : scheduledStart > DateTime.Now
                    ? "Scheduled"
                    : model.Status;

            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã cập nhật giờ tập.";
            return RedirectToAction(nameof(Index), new { date = model.ClassDate.ToString("yyyy-MM-dd") });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, DateTime date)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var classItem = await _dbContext.Classes.FindAsync(id);
            if (classItem == null)
            {
                return NotFound();
            }

            classItem.Status = "Cancelled";
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã hủy giờ tập.";
            return RedirectToAction(nameof(Index), new { date = date.ToString("yyyy-MM-dd") });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, DateTime date)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var classItem = await _dbContext.Classes
                .Include(c => c.Enrollments)
                .Include(c => c.Attendances)
                .FirstOrDefaultAsync(c => c.ClassId == id);
            if (classItem == null)
            {
                return NotFound();
            }

            if ((classItem.Enrollments?.Any() ?? false) || (classItem.Attendances?.Any() ?? false))
            {
                TempData["ErrorMessage"] = "Giờ tập đã có dữ liệu đăng ký/check-in, chỉ có thể hủy để giữ lịch sử.";
                return RedirectToAction(nameof(Index), new { date = date.ToString("yyyy-MM-dd") });
            }

            _dbContext.Classes.Remove(classItem);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa giờ tập.";
            return RedirectToAction(nameof(Index), new { date = date.ToString("yyyy-MM-dd") });
        }

        private async Task<TrainingRoom?> ValidateScheduleAsync(AdminScheduleFormViewModel model, int? currentClassId = null)
        {
            var room = await _dbContext.TrainingRooms.FindAsync(model.TrainingRoomId);
            if (room == null || room.Status != "Active")
            {
                ModelState.AddModelError(nameof(model.TrainingRoomId), "Phòng tập không tồn tại hoặc đã bị ẩn.");
            }

            if (room != null && room.Status == "Active" && model.MaxCapacity > room.Capacity)
            {
                ModelState.AddModelError(nameof(model.MaxCapacity), $"Số lượng tối đa không được vượt sức chứa phòng ({room.Capacity}).");
            }

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(nameof(model.EndTime), "Giờ kết thúc phải sau giờ bắt đầu.");
            }

            var scheduledStart = model.ClassDate.Date.Add(model.StartTime);
            if (scheduledStart <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(model.StartTime), "Giờ bắt đầu phải lớn hơn thời điểm hiện tại.");
            }

            var isBusy = await _dbContext.Classes.AnyAsync(c =>
                c.ClassId != currentClassId &&
                c.TrainingRoomId == model.TrainingRoomId &&
                c.ClassDate.Date == model.ClassDate.Date &&
                c.Status != "Cancelled" &&
                model.StartTime < c.EndTime &&
                model.EndTime > c.StartTime);

            if (isBusy)
            {
                ModelState.AddModelError("", "Phòng đã có lịch trong khung giờ này.");
            }

            return room;
        }

        private static DateTime GetSuggestedScheduleStart()
        {
            var now = DateTime.Now;
            var nextHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1);
            return nextHour.Hour >= 22
                ? nextHour.Date.AddDays(1).AddHours(7)
                : nextHour;
        }

        private async Task LoadRoomsAsync(int? selectedRoomId = null)
        {
            var rooms = await _dbContext.TrainingRooms
                .Where(r => r.Status == "Active")
                .OrderBy(r => r.RoomName)
                .Select(r => new { r.TrainingRoomId, r.RoomName })
                .ToListAsync();

            ViewBag.Rooms = new SelectList(rooms, "TrainingRoomId", "RoomName", selectedRoomId);
        }

        private async Task LoadScheduleOptionsAsync(string? selectedTrainer = null, string? selectedClassType = null, string? selectedLevel = null)
        {
            var trainerNames = (await _staffDirectoryService.GetStaffMembersAsync())
                .Where(s => s.PositionKind == "trainer" && s.StatusKind == "active")
                .Select(s => s.FullName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .ToList();

            if (!trainerNames.Any())
            {
                trainerNames.AddRange(DefaultTrainerNames);
            }

            AddSelectedOption(trainerNames, selectedTrainer);

            ViewBag.Trainers = new SelectList(trainerNames, selectedTrainer);
            ViewBag.ClassTypes = new SelectList(WithSelectedOption(ClassTypes, selectedClassType), selectedClassType);
            ViewBag.Levels = new SelectList(WithSelectedOption(ClassLevels, selectedLevel), selectedLevel);
        }

        private static List<string> WithSelectedOption(IEnumerable<string> options, string? selectedValue)
        {
            var list = options.ToList();
            AddSelectedOption(list, selectedValue);
            return list;
        }

        private static void AddSelectedOption(ICollection<string> options, string? selectedValue)
        {
            if (!string.IsNullOrWhiteSpace(selectedValue) &&
                !options.Any(option => option.Equals(selectedValue, StringComparison.OrdinalIgnoreCase)))
            {
                options.Add(selectedValue);
            }
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

        private static string BuildPublicScheduleReturnUrl(DateTime date, int? roomId, string? trainer)
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

        private async Task EnsureDefaultSchedulesAsync()
        {
            if (await _dbContext.Classes.AnyAsync())
            {
                return;
            }

            var rooms = await _dbContext.TrainingRooms
                .Where(r => r.Status == "Active")
                .OrderBy(r => r.TrainingRoomId)
                .ToListAsync();
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
