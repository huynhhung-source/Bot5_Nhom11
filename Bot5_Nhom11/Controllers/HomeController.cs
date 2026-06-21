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

        public HomeController(
            ILogger<HomeController> logger,
            GymDbContext dbContext,
            IGymService gymService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _gymService = gymService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Trainers()
        {
            return View(TrainerCatalog.All);
        }

        public IActionResult TrainerDetail(int id)
        {
            var trainer = TrainerCatalog.Find(id);
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

        public IActionResult ClassDetail(int id)
        {
            var gymClass = _gymService.GetById(id);
            return gymClass is null ? NotFound() : View(gymClass);
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
