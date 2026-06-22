using doanweb.Data;
using doanweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ScheduleController : Controller
    {
        private readonly GymDbContext _dbContext;

        public ScheduleController(GymDbContext dbContext)
        {
            _dbContext = dbContext;
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
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            await LoadRoomsAsync();
            return View(new AdminScheduleFormViewModel());
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
                MaxCapacity = room.Capacity,
                CurrentEnrollment = 0,
                Status = model.Status,
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
            if (room!.Capacity < registeredCount)
            {
                ModelState.AddModelError(nameof(model.TrainingRoomId), $"Phòng mới chỉ có {room.Capacity} chỗ, nhỏ hơn {registeredCount} khách đã đăng ký.");
                await LoadRoomsAsync(model.TrainingRoomId);
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
            classItem.MaxCapacity = room.Capacity;
            classItem.Status = model.Status;

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

        private async Task<TrainingRoom?> ValidateScheduleAsync(AdminScheduleFormViewModel model, int? currentClassId = null)
        {
            var room = await _dbContext.TrainingRooms.FindAsync(model.TrainingRoomId);
            if (room == null || room.Status != "Active")
            {
                ModelState.AddModelError(nameof(model.TrainingRoomId), "Phòng tập không tồn tại hoặc đã bị ẩn.");
            }

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(nameof(model.EndTime), "Giờ kết thúc phải sau giờ bắt đầu.");
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

        private async Task LoadRoomsAsync(int? selectedRoomId = null)
        {
            var rooms = await _dbContext.TrainingRooms
                .Where(r => r.Status == "Active")
                .OrderBy(r => r.RoomName)
                .Select(r => new { r.TrainingRoomId, r.RoomName })
                .ToListAsync();

            ViewBag.Rooms = new SelectList(rooms, "TrainingRoomId", "RoomName", selectedRoomId);
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
