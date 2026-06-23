using System.Diagnostics;
using doanweb.Data;
using doanweb.Models;
using doanweb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GymDbContext _dbContext;
        private readonly IGymService _gymService;
        private readonly IStaffDirectoryService _staffDirectoryService;

        public HomeController(
            ILogger<HomeController> logger,
            GymDbContext dbContext,
            IGymService gymService,
            IStaffDirectoryService staffDirectoryService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _gymService = gymService;
            _staffDirectoryService = staffDirectoryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Trainers()
        {
            var trainers = await _staffDirectoryService.GetTrainerViewModelsAsync();
            return View(trainers);
        }

        public async Task<IActionResult> TrainerDetail(int id)
        {
            var trainer = await _staffDirectoryService.GetTrainerViewModelAsync(id);
            return trainer is null ? NotFound() : View(trainer);
        }

        public IActionResult OnlinePackages()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Gyms()
        {
            var packages = await _dbContext.Packages
                .AsNoTracking()
                .Where(p => p.Status == "Active" && p.PackageType == "Offline")
                .OrderBy(p => p.Price)
                .ToListAsync();

            var rooms = await _dbContext.TrainingRooms
                .AsNoTracking()
                .Where(room => room.Status == "Active")
                .Include(room => room.Classes)
                    .ThenInclude(classItem => classItem.Enrollments)
                .OrderBy(room => room.RoomName)
                .ToListAsync();

            var roomModels = rooms.Select(room =>
            {
                var displayedClasses = (room.Classes ?? [])
                    .Where(classItem => classItem.Status != "Cancelled" &&
                        classItem.ClassDate.Date >= DateTime.Today)
                    .OrderBy(classItem => classItem.ClassDate)
                    .ThenBy(classItem => classItem.StartTime)
                    .ToList();

                var bookableClass = displayedClasses.FirstOrDefault(classItem =>
                    classItem.Status == "Scheduled" &&
                    (classItem.ClassDate.Date > DateTime.Today ||
                        (classItem.ClassDate.Date == DateTime.Today && classItem.EndTime > DateTime.Now.TimeOfDay)));
                var displayedClass = bookableClass ?? displayedClasses.FirstOrDefault();

                var availableSlots = displayedClasses.Count > 0
                    ? displayedClasses.Sum(classItem =>
                    {
                        var registered = classItem.Enrollments?.Count(enrollment => enrollment.Status != "Cancelled")
                            ?? classItem.CurrentEnrollment;
                        var capacity = classItem.MaxCapacity > 0 ? classItem.MaxCapacity : room.Capacity;
                        return Math.Max(0, capacity - registered);
                    })
                    : room.Capacity;

                var classTypes = displayedClasses
                    .Select(classItem => classItem.ClassType)
                    .Where(classType => !string.IsNullOrWhiteSpace(classType))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(4)
                    .Cast<string>()
                    .ToList();

                if (classTypes.Count == 0)
                {
                    classTypes.Add("Gym");
                }

                return new PublicGymRoomViewModel
                {
                    TrainingRoomId = room.TrainingRoomId,
                    NextClassId = bookableClass?.ClassId,
                    RoomName = room.RoomName,
                    Description = string.IsNullOrWhiteSpace(room.Description)
                        ? "Không gian luyện tập đang được cập nhật."
                        : room.Description,
                    Capacity = room.Capacity,
                    AvailableSlots = availableSlots,
                    UpcomingClassCount = displayedClasses.Count,
                    NextClassName = displayedClass?.ClassName ?? "Chưa có lịch tập",
                    InstructorName = displayedClass?.InstructorName ?? "Đang cập nhật",
                    ScheduleText = displayedClass == null
                        ? "Chưa có lịch sắp tới"
                        : $"{displayedClass.ClassDate:dd/MM/yyyy} {displayedClass.StartTime:hh\\:mm} - {displayedClass.EndTime:hh\\:mm}",
                    ImageUrl = string.IsNullOrWhiteSpace(room.ImageUrl)
                        ? GetRoomImage(room.RoomName, room.TrainingRoomId)
                        : room.ImageUrl,
                    ClassTypes = classTypes
                };
            }).ToList();

            return View(new PublicGymsViewModel
            {
                Rooms = roomModels,
                Packages = packages
            });
        }

        private static string GetRoomImage(string roomName, int roomId)
        {
            if (roomName.Contains("Boxing", StringComparison.OrdinalIgnoreCase))
            {
                return "/img/gallery/gallery-2.jpg";
            }

            if (roomName.Contains("Yoga", StringComparison.OrdinalIgnoreCase))
            {
                return "/img/gallery/gallery-3.jpg";
            }

            if (roomName.Contains("Gym", StringComparison.OrdinalIgnoreCase))
            {
                return "/img/gallery/gallery-1.jpg";
            }

            var imageNumber = ((roomId - 1) % 6) + 1;
            return $"/img/gallery/gallery-{imageNumber}.jpg";
        }

        public async Task<IActionResult> ClassDetail(int id)
        {
            var gymClass = _gymService.GetById(id);
            if (gymClass is null)
            {
                return NotFound();
            }

            try
            {
                ViewBag.JoinPackage = await _dbContext.Packages
                    .AsNoTracking()
                    .Where(p => p.Status == "Active" && p.PackageType == "Offline")
                    .OrderBy(p => p.StockQuantity > 0 ? 0 : 1)
                    .ThenBy(p => p.Price)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load join package for class detail. Falling back to default group class package.");
                ViewBag.JoinPackage = new Package
                {
                    PackageId = 5,
                    PackageName = "Group Classes",
                    Price = 790000,
                    DurationDays = 30
                };
            }

            return View(gymClass);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
