using doanweb.Data;
using doanweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GymController : Controller
    {
        private readonly GymDbContext _dbContext;

        public GymController(GymDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return !string.IsNullOrEmpty(userRole) && userRole == "Admin";
        }

        [HttpGet]
        public async Task<IActionResult> Index(string status = "all", string? search = null)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/admin/gym/index" });
            }

            status = NormalizeStatusFilter(status);
            search = search?.Trim() ?? string.Empty;

            var rooms = await _dbContext.TrainingRooms
                .Include(r => r.Classes)
                    .ThenInclude(c => c.Enrollments)
                .OrderBy(r => r.RoomName)
                .ToListAsync();

            var mappedRooms = rooms.Select(MapRoom).ToList();
            var filteredRooms = mappedRooms.AsEnumerable();

            if (status != "all")
            {
                filteredRooms = filteredRooms.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                filteredRooms = filteredRooms.Where(r =>
                    ContainsIgnoreCase(r.RoomName, search) ||
                    ContainsIgnoreCase(r.Description, search) ||
                    r.UpcomingClasses.Any(c => ContainsIgnoreCase(c.ClassName, search) || ContainsIgnoreCase(c.ClassType, search)));
            }

            var filteredList = filteredRooms.ToList();
            var model = new AdminGymIndexViewModel
            {
                Rooms = filteredList,
                StatusFilter = status,
                SearchTerm = search,
                TotalCount = mappedRooms.Count,
                ActiveCount = mappedRooms.Count(r => r.Status == "Active"),
                HiddenCount = mappedRooms.Count(r => r.Status == "Inactive"),
                TotalCapacity = mappedRooms.Sum(r => r.Capacity),
                TotalRegistered = mappedRooms.Sum(r => r.RegisteredCount),
                TotalAvailableSlots = mappedRooms.Sum(r => r.AvailableSlots)
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            return View(new TrainingRoomFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingRoomFormViewModel model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var room = new TrainingRoom
            {
                RoomName = model.RoomName.Trim(),
                Capacity = model.Capacity,
                Status = NormalizeRoomStatus(model.Status),
                Description = model.Description?.Trim(),
                CreatedDate = DateTime.Now
            };

            _dbContext.TrainingRooms.Add(room);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã thêm phòng tập mới.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var room = await _dbContext.TrainingRooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            return View(new TrainingRoomFormViewModel
            {
                TrainingRoomId = room.TrainingRoomId,
                RoomName = room.RoomName,
                Capacity = room.Capacity,
                Status = room.Status,
                Description = room.Description
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingRoomFormViewModel model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            if (id != model.TrainingRoomId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var room = await _dbContext.TrainingRooms
                .Include(r => r.Classes)
                    .ThenInclude(c => c.Enrollments)
                .FirstOrDefaultAsync(r => r.TrainingRoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            var highestUpcomingRegistration = room.Classes?
                .Where(c => c.ClassDate.Date >= DateTime.Today && c.Status != "Cancelled")
                .Select(c => c.Enrollments?.Count(e => e.Status != "Cancelled") ?? c.CurrentEnrollment)
                .DefaultIfEmpty(0)
                .Max() ?? 0;

            if (model.Capacity < highestUpcomingRegistration)
            {
                ModelState.AddModelError(nameof(model.Capacity), $"Sức chứa không được nhỏ hơn {highestUpcomingRegistration} khách đã đăng ký trong lịch sắp tới.");
                return View(model);
            }

            room.RoomName = model.RoomName.Trim();
            room.Capacity = model.Capacity;
            room.Status = NormalizeRoomStatus(model.Status);
            room.Description = model.Description?.Trim();
            room.UpdatedDate = DateTime.Now;

            if (room.Classes != null)
            {
                foreach (var classItem in room.Classes.Where(c => c.ClassDate.Date >= DateTime.Today))
                {
                    classItem.Location = room.RoomName;
                    classItem.MaxCapacity = room.Capacity;
                }
            }

            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã cập nhật phòng tập.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Hide(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var room = await _dbContext.TrainingRooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            room.Status = "Inactive";
            room.UpdatedDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã ẩn phòng không còn sử dụng.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> CreateSchedule(int? roomId = null)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            await LoadActiveRoomsAsync(roomId);
            return View(new RoomScheduleFormViewModel
            {
                TrainingRoomId = roomId ?? 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSchedule(RoomScheduleFormViewModel model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var room = await _dbContext.TrainingRooms.FindAsync(model.TrainingRoomId);
            if (room == null || room.Status != "Active")
            {
                ModelState.AddModelError(nameof(model.TrainingRoomId), "Phòng không tồn tại hoặc đã bị ẩn.");
            }

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(nameof(model.EndTime), "Giờ kết thúc phải sau giờ bắt đầu.");
            }

            var isBusy = await _dbContext.Classes.AnyAsync(c =>
                c.TrainingRoomId == model.TrainingRoomId &&
                c.ClassDate.Date == model.ClassDate.Date &&
                c.Status != "Cancelled" &&
                model.StartTime < c.EndTime &&
                model.EndTime > c.StartTime);

            if (isBusy)
            {
                ModelState.AddModelError("", "Phòng đã có lịch trong khung giờ này.");
            }

            if (!ModelState.IsValid)
            {
                await LoadActiveRoomsAsync(model.TrainingRoomId);
                return View(model);
            }

            var classItem = new Class
            {
                TrainingRoomId = room!.TrainingRoomId,
                ClassName = model.ClassName.Trim(),
                Description = model.Description?.Trim(),
                InstructorName = model.InstructorName.Trim(),
                ClassType = model.ClassType?.Trim(),
                Level = model.Level?.Trim(),
                ClassDate = model.ClassDate.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Location = room.RoomName,
                MaxCapacity = room.Capacity,
                CurrentEnrollment = 0,
                Status = "Scheduled",
                CreatedDate = DateTime.Now
            };

            _dbContext.Classes.Add(classItem);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã thêm lịch học vào phòng.";
            return RedirectToAction(nameof(Index));
        }

        private static AdminTrainingRoomViewModel MapRoom(TrainingRoom room)
        {
            var upcomingClasses = (room.Classes ?? [])
                .Where(c => c.ClassDate.Date >= DateTime.Today && c.Status != "Cancelled")
                .OrderBy(c => c.ClassDate)
                .ThenBy(c => c.StartTime)
                .ToList();

            var registeredCount = upcomingClasses.Sum(c => c.Enrollments?.Count(e => e.Status != "Cancelled") ?? c.CurrentEnrollment);
            var nextSchedules = upcomingClasses.Take(3).Select(c =>
            {
                var currentRegistration = c.Enrollments?.Count(e => e.Status != "Cancelled") ?? c.CurrentEnrollment;
                return new AdminRoomScheduleViewModel
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName,
                    ClassType = c.ClassType ?? string.Empty,
                    ClassDate = c.ClassDate,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    RegisteredCount = currentRegistration,
                    Capacity = c.MaxCapacity > 0 ? c.MaxCapacity : room.Capacity
                };
            }).ToList();

            var availableSlots = Math.Max(0, room.Capacity - registeredCount);

            return new AdminTrainingRoomViewModel
            {
                TrainingRoomId = room.TrainingRoomId,
                RoomName = room.RoomName,
                Capacity = room.Capacity,
                Status = room.Status,
                Description = room.Description,
                ClassCount = upcomingClasses.Count,
                RegisteredCount = registeredCount,
                AvailableSlots = availableSlots,
                IsAvailable = room.Status == "Active" && availableSlots > 0,
                UpcomingClasses = nextSchedules
            };
        }

        private static string NormalizeStatusFilter(string? status)
        {
            var normalized = (status ?? "all").Trim();
            return normalized.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
                normalized.Equals("Inactive", StringComparison.OrdinalIgnoreCase)
                ? normalized
                : "all";
        }

        private static string NormalizeRoomStatus(string? status)
        {
            return string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase) ? "Inactive" : "Active";
        }

        private static bool ContainsIgnoreCase(string? value, string term)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                value.Contains(term, StringComparison.OrdinalIgnoreCase);
        }

        private async Task LoadActiveRoomsAsync(int? selectedRoomId = null)
        {
            var rooms = await _dbContext.TrainingRooms
                .Where(r => r.Status == "Active")
                .OrderBy(r => r.RoomName)
                .Select(r => new { r.TrainingRoomId, r.RoomName })
                .ToListAsync();

            ViewBag.Rooms = new SelectList(rooms, "TrainingRoomId", "RoomName", selectedRoomId);
        }
    }
}
