using Microsoft.EntityFrameworkCore;
using doanweb.Models;

namespace doanweb.Data
{
    /// <summary>
    /// DbContext cho h? th?ng qu?n l� gym
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
        public DbSet<TrainingRoom> TrainingRooms { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

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

            // Relationship: TrainingRoom -> Classes (One-to-Many)
            modelBuilder.Entity<Class>()
                .HasOne(c => c.TrainingRoom)
                .WithMany(r => r.Classes)
                .HasForeignKey(c => c.TrainingRoomId)
                .OnDelete(DeleteBehavior.SetNull);

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

            // Relationship: User -> Orders (One-to-Many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Order -> OrderItems (One-to-Many)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Product -> OrderItems (One-to-Many)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .IsUnique()
                .HasFilter("[TransactionId] IS NOT NULL");

            modelBuilder.Entity<Class>()
                .HasIndex(c => c.ClassDate);

            modelBuilder.Entity<Class>()
                .HasIndex(c => c.TrainingRoomId);

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
                    Description = "Qu?n tr? vi�n h? th?ng",
                    Status = "Active",
                    CreatedDate = DateTime.Now
                },
                new Role
                {
                    RoleId = 2,
                    RoleName = "Customer",
                    Description = "Kh�ch h�ng",
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
                    Description = "Ch??ng tr?nh t?p luy?n c? nh�n v?i h??ng d?n dinh d??ng chi ti?t",
                    Price = 1990000,
                    DurationDays = 84,
                    PackageType = "Online",
                    Category = "Muscle",
                    AllowedClassTypes = "Gym,Strength",
                    Features = "C? nh?n h?a, Video HD 1080p, Email support 24/7",
                    MaxSessions = 12,
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Package
                {
                    PackageId = 2,
                    PackageName = "Fat Loss Program",
                    Description = "Ch??ng tr?nh cardio + t?p t? v?i k? ho?ch ?n u?ng th?c d?ng",
                    Price = 2490000,
                    DurationDays = 84,
                    PackageType = "Online",
                    Category = "FatLoss",
                    AllowedClassTypes = "Gym,Cardio",
                    Features = "Cardio, T?p t?, ?n u?ng, Support h?ng tu?n",
                    MaxSessions = 12,
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Package
                {
                    PackageId = 3,
                    PackageName = "Premium Gym",
                    Description = "Truy c?p t?t c? trang thi?t b? v?i hu?n luy?n vi?n ri?ng",
                    Price = 990000,
                    DurationDays = 30,
                    PackageType = "Offline",
                    Category = "Gym",
                    AllowedClassTypes = "Gym,Strength",
                    Features = "T?t c? thi?t b?, Hu?n luy?n vi?n 2x/tu?n, Steam room",
                    MaxSessions = 24,
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Package
                {
                    PackageId = 4,
                    PackageName = "Personal Training",
                    Description = "Hu?n luy?n vi?n ri?ng 4x/tu?n v?i ch??ng tr?nh c? nh?n h?a",
                    Price = 2990000,
                    DurationDays = 30,
                    PackageType = "Offline",
                    Category = "Gym",
                    AllowedClassTypes = "Gym,Personal Training",
                    Features = "Hu?n luy?n vi?n ri?ng, C? nh?n h?a, Theo d?i chi ti?t",
                    MaxSessions = 16,
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Package
                {
                    PackageId = 5,
                    PackageName = "Group Classes",
                    Description = "C?c l?p Yoga, Pilates, Zumba v?i hu?n luy?n vi?n chuy?n nghi?p",
                    Price = 790000,
                    DurationDays = 30,
                    PackageType = "Offline",
                    Category = "Yoga",
                    AllowedClassTypes = "Yoga,Pilates,Zumba",
                    Features = "Yoga h?ng ng?y, Pilates, Zumba, C?ng ??ng",
                    MaxSessions = 20,
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                }
            );

            // Seed Training Rooms
            modelBuilder.Entity<TrainingRoom>().HasData(
                new TrainingRoom
                {
                    TrainingRoomId = 1,
                    RoomName = "Phòng Gym 1",
                    Capacity = 30,
                    Status = "Active",
                    Description = "Phòng tập chính cho Gym, Strength và PT.",
                    CreatedDate = DateTime.Now
                },
                new TrainingRoom
                {
                    TrainingRoomId = 2,
                    RoomName = "Phòng Yoga",
                    Capacity = 20,
                    Status = "Active",
                    Description = "Không gian yên tĩnh cho Yoga, Pilates và Zumba.",
                    CreatedDate = DateTime.Now
                },
                new TrainingRoom
                {
                    TrainingRoomId = 3,
                    RoomName = "Phòng Boxing",
                    Capacity = 18,
                    Status = "Active",
                    Description = "Phòng chuyên dụng cho Boxing và cardio cường độ cao.",
                    CreatedDate = DateTime.Now
                }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    ProductName = "Whey Protein Isolate",
                    Description = "B?t Whey Protein ch?t l??ng cao, ???c c� l?p t? 90% ngu?n s?a t? nhi�n. Gi�u amino axit, h? tr? ph?c h?i c? b?p sau t?p luy?n.",
                    Price = 890000,
                    Category = "Whey",
                    Brand = "Optimum Nutrition",
                    StockQuantity = 50,
                    Unit = "kg",
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "Creatine Monohydrate",
                    Description = "Creatine Monohydrate tinh khi?t 100%, t?ng c??ng s?c m?nh v� kh? n?ng ph?c h?i c? b?p. H? tr? t?ng kh?i l??ng c? hi?u qu?.",
                    Price = 450000,
                    Category = "Creatine",
                    Brand = "Muscletech",
                    StockQuantity = 40,
                    Unit = "kg",
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "Protein Bar Chocolate",
                    Description = "B�nh Protein Bar v? socola ngon mi?ng, ch?a 20g protein, �t ???ng, l� t??ng cho b?a ?n nh? tr??c/sau t?p luy?n.",
                    Price = 65000,
                    Category = "Protein Bar",
                    Brand = "Quest Nutrition",
                    StockQuantity = 100,
                    Unit = "box",
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Product
                {
                    ProductId = 4,
                    ProductName = "Whey Protein Concentrate",
                    Description = "B?t Whey Protein n?ng ?? cao, h? tr? x�y d?ng kh?i c?, gi�u BCAA t? nhi�n. V? ngon, d? h�a tan.",
                    Price = 650000,
                    Category = "Whey",
                    Brand = "Gold Standard",
                    StockQuantity = 60,
                    Unit = "kg",
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Product
                {
                    ProductId = 5,
                    ProductName = "Creatine + Beta-Alanine Mix",
                    Description = "H?n h?p Creatine v� Beta-Alanine, t?ng c??ng hi?u su?t t?p luy?n, gi?m m?t m?i c?, c?i thi?n s?c b?n.",
                    Price = 520000,
                    Category = "Creatine",
                    Brand = "MuscleTech",
                    StockQuantity = 35,
                    Unit = "kg",
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                },
                new Product
                {
                    ProductId = 6,
                    ProductName = "Protein Bar Peanut Butter",
                    Description = "B�nh Protein Bar v? b? ??u phong v?, 25g protein, kh�ng ch?a ???ng, b? sung n?ng l??ng cho ng�y d�i.",
                    Price = 70000,
                    Category = "Protein Bar",
                    Brand = "Quest Nutrition",
                    StockQuantity = 80,
                    Unit = "box",
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
