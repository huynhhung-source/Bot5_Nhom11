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

        public async Task<IActionResult> Gyms()
        {
            var packages = await _dbContext.Packages
                .AsNoTracking()
                .Where(p => p.Status == "Active" && p.PackageType == "Offline")
                .OrderBy(p => p.Price)
                .ToListAsync();

            return View(packages);
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
