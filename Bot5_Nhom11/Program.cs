using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using doanweb.Data;
using doanweb.Services;
using System.Text;
using QuestPDF.Infrastructure;

// UTF-8
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = Encoding.UTF8;

// QuestPDF: bắt buộc cấu hình license trước khi tạo PDF
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// MVC
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseSqlServer(connectionString));

// Session
var dataProtectionKeysPath = Path.Combine(builder.Environment.ContentRootPath, ".aspnet-data-protection-keys");
Directory.CreateDirectory(dataProtectionKeysPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
    .SetApplicationName("Bot5_Nhom11");

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// ================== AUTH FACEBOOK ==================
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/customer/account/login";
    options.LogoutPath = "/customer/account/logout";
    options.AccessDeniedPath = "/customer/account/login";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
// ===================================================

// Service
builder.Services.AddScoped<IOAuthService, OAuthService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IStaffDirectoryService, StaffDirectoryService>();
builder.Services.AddSingleton<IGymService, GymService>();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();


app.UseAuthentication();
app.UseAuthorization();

// Route Adminsa
app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Admin" });

// Route Customer
app.MapControllerRoute(
    name: "customer",
    pattern: "customer/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Customer" });

// Default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
