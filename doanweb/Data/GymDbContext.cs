using Microsoft.EntityFrameworkCore;
using doanweb.Models;

namespace doanweb.Data
{
    /// <summary>
    /// DbContext cho h? th?ng qu?n lý gym
    /// </summary>
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationship: User -> Subscriptions (One-to-Many)
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Package -> Subscriptions (One-to-Many)
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Package)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(s => s.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: User -> ClassEnrollments (One-to-Many)
            modelBuilder.Entity<ClassEnrollment>()
                .HasOne(ce => ce.User)
                .WithMany(u => u.ClassEnrollments)
                .HasForeignKey(ce => ce.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Class -> ClassEnrollments (One-to-Many)
            modelBuilder.Entity<ClassEnrollment>()
                .HasOne(ce => ce.Class)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(ce => ce.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Subscription -> Attendances (One-to-Many)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Subscription)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Class -> Attendances (One-to-Many)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Class)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: User -> Payments (One-to-Many)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: User -> Reviews (One-to-Many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber);

            modelBuilder.Entity<Subscription>()
                .HasIndex(s => s.UserId);

            modelBuilder.Entity<Subscription>()
                .HasIndex(s => s.Status);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.TransactionId)
                .IsUnique();

            modelBuilder.Entity<Class>()
                .HasIndex(c => c.ClassDate);

            // Seed initial data
            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleId = 1,
                    RoleName = "Admin",
                    Description = "Qu?n tr? viên h? th?ng",
                    Status = "Active",
                    CreatedDate = DateTime.Now
                },
                new Role
                {
                    RoleId = 2,
                    RoleName = "Customer",
                    Description = "Khách hàng",
                    Status = "Active",
                    CreatedDate = DateTime.Now
                }
            );

            // Seed Admin User
            // Password: Admin@123
            var adminPasswordHash = HashPassword("Admin@123");
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Admin",
                    Email = "admin@gmail.com",
                    PhoneNumber = "0123456789",
                    PasswordHash = adminPasswordHash,
                    Address = "123 Admin Street",
                    Gender = "Male",
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                }
            );

            // Seed UserRole (Assign Admin role to Admin user)
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    UserRoleId = 1,
                    UserId = 1,
                    RoleId = 1,
                    AssignedDate = DateTime.Now
                }
            );

            // Seed Packages
            modelBuilder.Entity<Package>().HasData(
                new Package
                {
                    PackageId = 1,
                    PackageName = "Body Recomposition",
                    Description = "Ch??ng trình t?p luy?n cá nhân v?i h??ng d?n dinh d??ng chi ti?t",
                    Price = 1990000,
                    DurationDays = 84,
                    PackageType = "Online",
                    Category = "Muscle",
                    Features = "Cá nhân hóa, Video HD 1080p, Email support 24/7",
                    MaxSessions = 12,
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Package
                {
                    PackageId = 2,
                    PackageName = "Fat Loss Program",
                    Description = "Ch??ng trình cardio + t?p t? v?i k? ho?ch ?n u?ng th?c d?ng",
                    Price = 2490000,
                    DurationDays = 84,
                    PackageType = "Online",
                    Category = "FatLoss",
                    Features = "Cardio, T?p t?, ?n u?ng, Support hàng tu?n",
                    MaxSessions = 12,
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Package
                {
                    PackageId = 3,
                    PackageName = "Premium Gym",
                    Description = "Truy c?p t?t c? trang thi?t b? v?i hu?n luy?n viên riêng",
                    Price = 990000,
                    DurationDays = 30,
                    PackageType = "Offline",
                    Category = "Gym",
                    Features = "T?t c? thi?t b?, Hu?n luy?n viên 2x/tu?n, Steam room",
                    MaxSessions = 24,
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Package
                {
                    PackageId = 4,
                    PackageName = "Personal Training",
                    Description = "Hu?n luy?n viên riêng 4x/tu?n v?i ch??ng trình cá nhân hóa",
                    Price = 2990000,
                    DurationDays = 30,
                    PackageType = "Offline",
                    Category = "Gym",
                    Features = "Hu?n luy?n viên riêng, Cá nhân hóa, Theo dõi chi ti?t",
                    MaxSessions = 16,
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Package
                {
                    PackageId = 5,
                    PackageName = "Group Classes",
                    Description = "Các l?p Yoga, Pilates, Zumba v?i hu?n luy?n viên chuyên nghi?p",
                    Price = 790000,
                    DurationDays = 30,
                    PackageType = "Offline",
                    Category = "Yoga",
                    Features = "Yoga hàng ngày, Pilates, Zumba, C?ng ??ng",
                    MaxSessions = 20,
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                }
            );
        }

        // Helper method to hash password (copy from AccountController)
        private static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
