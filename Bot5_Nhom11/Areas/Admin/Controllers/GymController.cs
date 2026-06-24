using doanweb.Data;
using doanweb.Models;
using doanweb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GymController : Controller
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStaffDirectoryService _staffDirectoryService;

        public GymController(
            GymDbContext dbContext,
            IWebHostEnvironment webHostEnvironment,
            IStaffDirectoryService staffDirectoryService)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _staffDirectoryService = staffDirectoryService;
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
                .Include(r => r.Classes)
                    .ThenInclude(c => c.Attendances)
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

            if (model.ImageFile == null || model.ImageFile.Length == 0)
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Vui lòng chọn ảnh cho phòng tập.");
                return View(model);
            }

            var roomName = model.RoomName.Trim();
            var nameExists = await _dbContext.TrainingRooms.AnyAsync(r => r.RoomName == roomName);
            if (nameExists)
            {
                ModelState.AddModelError(nameof(model.RoomName), "Tên phòng tập đã tồn tại.");
                return View(model);
            }

            var imageUrl = await SaveRoomImageAsync(model.ImageFile);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var room = new TrainingRoom
            {
                RoomName = roomName,
                Capacity = model.Capacity,
                Status = NormalizeRoomStatus(model.Status),
                Description = model.Description?.Trim(),
                ImageUrl = imageUrl,
                CreatedDate = DateTime.Now
            };

            _dbContext.TrainingRooms.Add(room);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã tạo phòng. Vui lòng thêm lịch học và huấn luyện viên để mở phòng cho khách đăng ký.";
            return room.Status == "Active"
                ? RedirectToAction(nameof(CreateSchedule), new { roomId = room.TrainingRoomId })
                : RedirectToAction(nameof(Index));
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
                Description = room.Description,
                ExistingImageUrl = room.ImageUrl
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

            var roomName = model.RoomName.Trim();
            var nameExists = await _dbContext.TrainingRooms.AnyAsync(r => r.TrainingRoomId != id && r.RoomName == roomName);
            if (nameExists)
            {
                ModelState.AddModelError(nameof(model.RoomName), "Tên phòng tập đã tồn tại.");
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

            var newImageUrl = await SaveRoomImageAsync(model.ImageFile);
            if (!ModelState.IsValid)
            {
                model.ExistingImageUrl = room.ImageUrl;
                return View(model);
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

            room.RoomName = roomName;
            room.Capacity = model.Capacity;
            room.Status = NormalizeRoomStatus(model.Status);
            room.Description = model.Description?.Trim();
            if (!string.IsNullOrWhiteSpace(newImageUrl))
            {
                DeleteRoomImage(room.ImageUrl);
                room.ImageUrl = newImageUrl;
            }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
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

            room.Status = "Active";
            room.UpdatedDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã kích hoạt lại phòng tập.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var room = await _dbContext.TrainingRooms
                .Include(r => r.Classes)
                    .ThenInclude(c => c.Enrollments)
                .Include(r => r.Classes)
                    .ThenInclude(c => c.Attendances)
                .FirstOrDefaultAsync(r => r.TrainingRoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            var classes = room.Classes?.ToList() ?? [];
            var hasRegistrationHistory = classes.Any(c => c.Enrollments?.Any() == true);
            var hasAttendanceHistory = classes.Any(c => c.Attendances?.Any() == true);
            if (hasRegistrationHistory || hasAttendanceHistory)
            {
                TempData["ErrorMessage"] = "Không thể xóa phòng vì đã có khách đăng ký hoặc lịch sử điểm danh. Hãy dùng chức năng Ẩn để giữ nguyên dữ liệu.";
                return RedirectToAction(nameof(Index));
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            if (classes.Count > 0)
            {
                _dbContext.Classes.RemoveRange(classes);
            }

            var roomImageUrl = room.ImageUrl;
            _dbContext.TrainingRooms.Remove(room);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            DeleteRoomImage(roomImageUrl);

            TempData["SuccessMessage"] = classes.Count > 0
                ? $"Đã xóa phòng tập và {classes.Count} lịch học chưa có khách đăng ký."
                : "Đã xóa phòng tập.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> CreateSchedule(int? roomId = null)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var suggestedStart = GetSuggestedScheduleStart();
            await LoadActiveRoomsAsync(roomId);
            await LoadScheduleOptionsAsync();
            return View(new RoomScheduleFormViewModel
            {
                TrainingRoomId = roomId ?? 0,
                ClassDate = suggestedStart.Date,
                StartTime = suggestedStart.TimeOfDay,
                EndTime = suggestedStart.AddHours(1).TimeOfDay
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

            var scheduledStart = model.ClassDate.Date.Add(model.StartTime);
            if (scheduledStart <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(model.StartTime), "Giờ bắt đầu phải lớn hơn thời điểm hiện tại.");
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
                await LoadScheduleOptionsAsync(model.InstructorName, model.ClassType, model.Level);
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

        private static DateTime GetSuggestedScheduleStart()
        {
            var now = DateTime.Now;
            var nextHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1);
            return nextHour.Hour >= 22
                ? nextHour.Date.AddDays(1).AddHours(7)
                : nextHour;
        }

        private static AdminTrainingRoomViewModel MapRoom(TrainingRoom room)
        {
            var upcomingClasses = (room.Classes ?? [])
                .Where(c => c.ClassDate.Date >= DateTime.Today && c.Status != "Cancelled")
                .OrderBy(c => c.ClassDate)
                .ThenBy(c => c.StartTime)
                .ToList();

            var scheduleStats = upcomingClasses
                .Select(c =>
                {
                    var registered = c.Enrollments?.Count(e => e.Status != "Cancelled") ?? c.CurrentEnrollment;
                    var capacity = c.MaxCapacity > 0 ? c.MaxCapacity : room.Capacity;
                    return new
                    {
                        ClassItem = c,
                        Registered = registered,
                        Capacity = capacity,
                        Available = Math.Max(0, capacity - registered)
                    };
                })
                .ToList();

            var registeredCount = scheduleStats.Sum(c => c.Registered);
            var nextSchedules = scheduleStats.Take(3).Select(item =>
            {
                var c = item.ClassItem;
                return new AdminRoomScheduleViewModel
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName,
                    ClassType = c.ClassType ?? string.Empty,
                    ClassDate = c.ClassDate,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    RegisteredCount = item.Registered,
                    Capacity = item.Capacity
                };
            }).ToList();

            var availableSlots = scheduleStats.Any()
                ? scheduleStats.Sum(c => c.Available)
                : room.Capacity;
            var fullClassCount = scheduleStats.Count(c => c.Available <= 0);

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
                IsAvailable = room.Status == "Active" && (!scheduleStats.Any() || scheduleStats.Any(c => c.Available > 0)),
                FullClassCount = fullClassCount,
                IsDeletable = !(room.Classes?.Any(c =>
                    c.Enrollments?.Any() == true ||
                    c.Attendances?.Any() == true) ?? false),
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

        private async Task<string?> SaveRoomImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }

            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(TrainingRoomFormViewModel.ImageFile), "Ảnh phải có định dạng JPG, PNG hoặc WebP.");
                return null;
            }

            if (imageFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(TrainingRoomFormViewModel.ImageFile), "Kích thước ảnh không được vượt quá 5MB.");
                return null;
            }

            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "rooms");
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadFolder, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return $"/images/rooms/{fileName}";
        }

        private void DeleteRoomImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl) || !imageUrl.StartsWith("/images/rooms/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.GetFullPath(Path.Combine(_webHostEnvironment.WebRootPath, relativePath));
            var roomImageFolder = Path.GetFullPath(Path.Combine(_webHostEnvironment.WebRootPath, "images", "rooms"));
            if (fullPath.StartsWith(roomImageFolder, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
