using doanweb.Models;
using doanweb.Services;
using Microsoft.AspNetCore.Mvc;

namespace doanweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StaffController : Controller
    {
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private readonly IStaffDirectoryService _staffDirectoryService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<StaffController> _logger;

        public StaffController(
            IStaffDirectoryService staffDirectoryService,
            IWebHostEnvironment environment,
            ILogger<StaffController> logger)
        {
            _staffDirectoryService = staffDirectoryService;
            _environment = environment;
            _logger = logger;
        }

        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return !string.IsNullOrEmpty(userRole) && userRole == "Admin";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/admin/staff/index" });
            }

            return View(await BuildIndexViewModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "NewStaff")] StaffMemberFormViewModel newStaff)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            await ApplyUploadedPhotoAsync(newStaff);

            if (!ModelState.IsValid)
            {
                return View("Index", await BuildIndexViewModelAsync(newStaff, showCreateModal: true));
            }

            try
            {
                await _staffDirectoryService.CreateStaffMemberAsync(newStaff);
                TempData["SuccessMessage"] = $"Đã thêm nhân viên {newStaff.FullName.Trim()}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff member");
                ModelState.AddModelError("", "Không thể thêm nhân viên. Vui lòng thử lại.");
                return View("Index", await BuildIndexViewModelAsync(newStaff, showCreateModal: true));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var staff = await _staffDirectoryService.GetStaffMemberAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            ViewBag.StaffMemberId = id;
            ViewBag.StaffCode = staff.StaffCode;
            ViewBag.CurrentImageUrl = staff.ImageUrl;
            return View(_staffDirectoryService.ToForm(staff));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = $"/admin/staff/details/{id}" });
            }

            var staff = await _staffDirectoryService.GetStaffMemberAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StaffMemberFormViewModel model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var staff = await _staffDirectoryService.GetStaffMemberAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            await ApplyUploadedPhotoAsync(model);

            if (!ModelState.IsValid)
            {
                ViewBag.StaffMemberId = id;
                ViewBag.StaffCode = staff.StaffCode;
                ViewBag.CurrentImageUrl = model.ImageUrl;
                return View(model);
            }

            await _staffDirectoryService.UpdateStaffMemberAsync(id, model);
            TempData["SuccessMessage"] = $"Đã cập nhật {model.FullName.Trim()}.";
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

            await _staffDirectoryService.DeleteStaffMemberAsync(id);
            TempData["SuccessMessage"] = "Đã xóa nhân viên.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<AdminStaffIndexViewModel> BuildIndexViewModelAsync(
            StaffMemberFormViewModel? form = null,
            bool showCreateModal = false)
        {
            var staffMembers = await _staffDirectoryService.GetStaffMembersAsync();

            return new AdminStaffIndexViewModel
            {
                StaffMembers = staffMembers,
                NewStaff = form ?? new StaffMemberFormViewModel(),
                ShowCreateModal = showCreateModal,
                TotalCount = staffMembers.Count,
                ActiveCount = staffMembers.Count(s => s.StatusKind == "active"),
                TrainerCount = staffMembers.Count(s => s.PositionKind == "trainer")
            };
        }

        private async Task ApplyUploadedPhotoAsync(StaffMemberFormViewModel model)
        {
            if (model.PhotoFile == null || model.PhotoFile.Length == 0)
            {
                model.ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim();
                return;
            }

            var extension = Path.GetExtension(model.PhotoFile.FileName);
            if (!AllowedImageExtensions.Contains(extension))
            {
                ModelState.AddModelError("PhotoFile", "Ảnh phải là JPG, PNG hoặc WEBP.");
                ModelState.AddModelError("NewStaff.PhotoFile", "Ảnh phải là JPG, PNG hoặc WEBP.");
                return;
            }

            var folder = Path.Combine(_environment.WebRootPath, "img", "team", "staff");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var filePath = Path.Combine(folder, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await model.PhotoFile.CopyToAsync(stream);

            model.ImageUrl = $"/img/team/staff/{fileName}";
        }
    }
}
