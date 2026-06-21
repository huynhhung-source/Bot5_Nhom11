using System.Data;
using System.Data.Common;
using doanweb.Data;
using doanweb.Models;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Services
{
    public interface IStaffDirectoryService
    {
        Task<List<AdminStaffItemViewModel>> GetStaffMembersAsync();
        Task<AdminStaffItemViewModel?> GetStaffMemberAsync(int id);
        Task CreateStaffMemberAsync(StaffMemberFormViewModel model);
        Task UpdateStaffMemberAsync(int id, StaffMemberFormViewModel model);
        Task DeleteStaffMemberAsync(int id);
        Task<IReadOnlyList<TrainerViewModel>> GetTrainerViewModelsAsync();
        Task<TrainerViewModel?> GetTrainerViewModelAsync(int id);
        StaffMemberFormViewModel ToForm(AdminStaffItemViewModel staff);
    }

    public class StaffDirectoryService : IStaffDirectoryService
    {
        private readonly GymDbContext _dbContext;

        public StaffDirectoryService(GymDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AdminStaffItemViewModel>> GetStaffMembersAsync()
        {
            await EnsureStaffTableAsync();
            var result = new List<AdminStaffItemViewModel>();

            await WithOpenConnectionAsync(async connection =>
            {
                await using var command = connection.CreateCommand();
                command.CommandText =
                    """
                    SELECT [StaffMemberId], [StaffCode], [FullName], [Position], [Specialty],
                           [ImageUrl], [PhoneNumber], [Email], [Salary], [Rating], [MonthlyClasses],
                           [ExperienceYears], [Introduction], [Philosophy], [Location], [Certificates],
                           [ScheduleText], [Status]
                    FROM [StaffMembers]
                    ORDER BY [StaffMemberId];
                    """;

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(MapStaff(reader));
                }
            });

            return result;
        }

        public async Task<AdminStaffItemViewModel?> GetStaffMemberAsync(int id)
        {
            await EnsureStaffTableAsync();
            AdminStaffItemViewModel? staffMember = null;

            await WithOpenConnectionAsync(async connection =>
            {
                await using var command = connection.CreateCommand();
                command.CommandText =
                    """
                    SELECT [StaffMemberId], [StaffCode], [FullName], [Position], [Specialty],
                           [ImageUrl], [PhoneNumber], [Email], [Salary], [Rating], [MonthlyClasses],
                           [ExperienceYears], [Introduction], [Philosophy], [Location], [Certificates],
                           [ScheduleText], [Status]
                    FROM [StaffMembers]
                    WHERE [StaffMemberId] = @StaffMemberId;
                    """;
                command.Parameters.Add(CreateParameter(connection, "@StaffMemberId", id));

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    staffMember = MapStaff(reader);
                }
            });

            return staffMember;
        }

        public async Task CreateStaffMemberAsync(StaffMemberFormViewModel model)
        {
            await EnsureStaffTableAsync();
            var staffCode = await BuildNextStaffCodeAsync();

            await ExecuteNonQueryAsync(
                """
                INSERT INTO [StaffMembers]
                    ([StaffCode], [FullName], [Position], [Specialty], [ImageUrl], [PhoneNumber],
                     [Email], [Salary], [Rating], [MonthlyClasses], [ExperienceYears], [Introduction],
                     [Philosophy], [Location], [Certificates], [ScheduleText], [Status], [CreatedDate])
                VALUES
                    (@StaffCode, @FullName, @Position, @Specialty, @ImageUrl, @PhoneNumber,
                     @Email, @Salary, @Rating, @MonthlyClasses, @ExperienceYears, @Introduction,
                     @Philosophy, @Location, @Certificates, @ScheduleText, @Status, SYSDATETIME());
                """,
                ("@StaffCode", staffCode),
                ("@FullName", model.FullName.Trim()),
                ("@Position", model.Position.Trim()),
                ("@Specialty", ToDbValue(model.Specialty)),
                ("@ImageUrl", ToDbValue(model.ImageUrl)),
                ("@PhoneNumber", model.PhoneNumber.Trim()),
                ("@Email", ToDbValue(model.Email)),
                ("@Salary", model.Salary),
                ("@Rating", model.Rating),
                ("@MonthlyClasses", model.MonthlyClasses),
                ("@ExperienceYears", model.ExperienceYears),
                ("@Introduction", ToDbValue(model.Introduction)),
                ("@Philosophy", ToDbValue(model.Philosophy)),
                ("@Location", ToDbValue(model.Location)),
                ("@Certificates", ToDbValue(model.Certificates)),
                ("@ScheduleText", ToDbValue(model.ScheduleText)),
                ("@Status", model.Status));
        }

        public async Task UpdateStaffMemberAsync(int id, StaffMemberFormViewModel model)
        {
            await EnsureStaffTableAsync();

            await ExecuteNonQueryAsync(
                """
                UPDATE [StaffMembers]
                SET [FullName] = @FullName,
                    [Position] = @Position,
                    [Specialty] = @Specialty,
                    [ImageUrl] = @ImageUrl,
                    [PhoneNumber] = @PhoneNumber,
                    [Email] = @Email,
                    [Salary] = @Salary,
                    [Rating] = @Rating,
                    [MonthlyClasses] = @MonthlyClasses,
                    [ExperienceYears] = @ExperienceYears,
                    [Introduction] = @Introduction,
                    [Philosophy] = @Philosophy,
                    [Location] = @Location,
                    [Certificates] = @Certificates,
                    [ScheduleText] = @ScheduleText,
                    [Status] = @Status,
                    [UpdatedDate] = SYSDATETIME()
                WHERE [StaffMemberId] = @StaffMemberId;
                """,
                ("@StaffMemberId", id),
                ("@FullName", model.FullName.Trim()),
                ("@Position", model.Position.Trim()),
                ("@Specialty", ToDbValue(model.Specialty)),
                ("@ImageUrl", ToDbValue(model.ImageUrl)),
                ("@PhoneNumber", model.PhoneNumber.Trim()),
                ("@Email", ToDbValue(model.Email)),
                ("@Salary", model.Salary),
                ("@Rating", model.Rating),
                ("@MonthlyClasses", model.MonthlyClasses),
                ("@ExperienceYears", model.ExperienceYears),
                ("@Introduction", ToDbValue(model.Introduction)),
                ("@Philosophy", ToDbValue(model.Philosophy)),
                ("@Location", ToDbValue(model.Location)),
                ("@Certificates", ToDbValue(model.Certificates)),
                ("@ScheduleText", ToDbValue(model.ScheduleText)),
                ("@Status", model.Status));
        }

        public async Task DeleteStaffMemberAsync(int id)
        {
            await EnsureStaffTableAsync();
            await ExecuteNonQueryAsync(
                "DELETE FROM [StaffMembers] WHERE [StaffMemberId] = @StaffMemberId;",
                ("@StaffMemberId", id));
        }

        public async Task<IReadOnlyList<TrainerViewModel>> GetTrainerViewModelsAsync()
        {
            var staffMembers = await GetStaffMembersAsync();
            return staffMembers
                .Where(s => s.PositionKind == "trainer" && s.StatusKind != "inactive")
                .Select(MapTrainer)
                .ToList();
        }

        public async Task<TrainerViewModel?> GetTrainerViewModelAsync(int id)
        {
            var staff = await GetStaffMemberAsync(id);
            return staff is null || staff.PositionKind != "trainer" || staff.StatusKind == "inactive"
                ? null
                : MapTrainer(staff);
        }

        public StaffMemberFormViewModel ToForm(AdminStaffItemViewModel staff)
        {
            return new StaffMemberFormViewModel
            {
                FullName = staff.FullName,
                Specialty = staff.Specialty,
                ImageUrl = staff.ImageUrl,
                PhoneNumber = staff.PhoneNumber,
                Email = staff.Email,
                Salary = staff.Salary,
                Position = staff.Position,
                ExperienceYears = staff.ExperienceYears,
                Rating = staff.Rating,
                MonthlyClasses = staff.MonthlyClasses,
                Status = staff.Status,
                Location = staff.Location,
                Introduction = staff.Introduction,
                Philosophy = staff.Philosophy,
                Certificates = staff.Certificates,
                ScheduleText = staff.ScheduleText
            };
        }

        private async Task EnsureStaffTableAsync()
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                """
                IF OBJECT_ID(N'[dbo].[StaffMembers]', N'U') IS NOT NULL
                BEGIN
                    IF COL_LENGTH('dbo.StaffMembers', 'ImageUrl') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [ImageUrl] NVARCHAR(255) NULL;
                    IF COL_LENGTH('dbo.StaffMembers', 'ExperienceYears') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [ExperienceYears] INT NOT NULL DEFAULT 3;
                    IF COL_LENGTH('dbo.StaffMembers', 'Introduction') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [Introduction] NVARCHAR(700) NULL;
                    IF COL_LENGTH('dbo.StaffMembers', 'Philosophy') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [Philosophy] NVARCHAR(500) NULL;
                    IF COL_LENGTH('dbo.StaffMembers', 'Location') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [Location] NVARCHAR(150) NULL;
                    IF COL_LENGTH('dbo.StaffMembers', 'Certificates') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [Certificates] NVARCHAR(500) NULL;
                    IF COL_LENGTH('dbo.StaffMembers', 'ScheduleText') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [ScheduleText] NVARCHAR(500) NULL;
                    IF COL_LENGTH('dbo.StaffMembers', 'CreatedDate') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [CreatedDate] DATETIME2 NOT NULL DEFAULT SYSDATETIME();
                    IF COL_LENGTH('dbo.StaffMembers', 'UpdatedDate') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [UpdatedDate] DATETIME2 NULL;
                END;
                """);

            await _dbContext.Database.ExecuteSqlRawAsync(
                """
                IF OBJECT_ID(N'[dbo].[StaffMembers]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[StaffMembers]
                    (
                        [StaffMemberId] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_StaffMembers] PRIMARY KEY,
                        [StaffCode] NVARCHAR(20) NOT NULL,
                        [FullName] NVARCHAR(100) NOT NULL,
                        [Position] NVARCHAR(50) NOT NULL,
                        [Specialty] NVARCHAR(120) NULL,
                        [ImageUrl] NVARCHAR(255) NULL,
                        [PhoneNumber] NVARCHAR(20) NOT NULL,
                        [Email] NVARCHAR(100) NULL,
                        [Salary] DECIMAL(18,0) NOT NULL CONSTRAINT [DF_StaffMembers_Salary] DEFAULT 0,
                        [Rating] DECIMAL(3,1) NOT NULL CONSTRAINT [DF_StaffMembers_Rating] DEFAULT 4.8,
                        [MonthlyClasses] INT NOT NULL CONSTRAINT [DF_StaffMembers_MonthlyClasses] DEFAULT 0,
                        [ExperienceYears] INT NOT NULL CONSTRAINT [DF_StaffMembers_ExperienceYears] DEFAULT 3,
                        [Introduction] NVARCHAR(700) NULL,
                        [Philosophy] NVARCHAR(500) NULL,
                        [Location] NVARCHAR(150) NULL,
                        [Certificates] NVARCHAR(500) NULL,
                        [ScheduleText] NVARCHAR(500) NULL,
                        [Status] NVARCHAR(20) NOT NULL CONSTRAINT [DF_StaffMembers_Status] DEFAULT N'Active',
                        [CreatedDate] DATETIME2 NOT NULL CONSTRAINT [DF_StaffMembers_CreatedDate] DEFAULT SYSDATETIME(),
                        [UpdatedDate] DATETIME2 NULL
                    );

                    CREATE UNIQUE INDEX [IX_StaffMembers_StaffCode] ON [dbo].[StaffMembers] ([StaffCode]);
                    CREATE INDEX [IX_StaffMembers_PhoneNumber] ON [dbo].[StaffMembers] ([PhoneNumber]);
                END;

                IF COL_LENGTH('dbo.StaffMembers', 'ImageUrl') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [ImageUrl] NVARCHAR(255) NULL;
                IF COL_LENGTH('dbo.StaffMembers', 'ExperienceYears') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [ExperienceYears] INT NOT NULL CONSTRAINT [DF_StaffMembers_ExperienceYears] DEFAULT 3;
                IF COL_LENGTH('dbo.StaffMembers', 'Introduction') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [Introduction] NVARCHAR(700) NULL;
                IF COL_LENGTH('dbo.StaffMembers', 'Philosophy') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [Philosophy] NVARCHAR(500) NULL;
                IF COL_LENGTH('dbo.StaffMembers', 'Location') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [Location] NVARCHAR(150) NULL;
                IF COL_LENGTH('dbo.StaffMembers', 'Certificates') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [Certificates] NVARCHAR(500) NULL;
                IF COL_LENGTH('dbo.StaffMembers', 'ScheduleText') IS NULL ALTER TABLE [dbo].[StaffMembers] ADD [ScheduleText] NVARCHAR(500) NULL;

                IF NOT EXISTS (SELECT 1 FROM [dbo].[StaffMembers])
                BEGIN
                    INSERT INTO [dbo].[StaffMembers]
                        ([StaffCode], [FullName], [Position], [Specialty], [ImageUrl], [PhoneNumber],
                         [Email], [Salary], [Rating], [MonthlyClasses], [ExperienceYears],
                         [Introduction], [Philosophy], [Location], [Certificates], [ScheduleText],
                         [Status], [CreatedDate])
                    VALUES
                        (N'NV001', N'Nguyễn Thị Mai', N'Huấn Luyện Viên', N'Yoga & Pilates', N'/img/team/team-1.jpg', N'0901234567', N'mai@gympro.vn', 12000000, 4.9, 24, 8, N'Chuyên xây dựng chương trình Yoga và Pilates giúp cải thiện độ dẻo dai, tư thế và kiểm soát cơ thể.', N'Tập luyện là cách tìm lại cân bằng và nguồn năng lượng tích cực mỗi ngày.', N'Gym Center Nguyễn Huệ', N'RYT 500 Yoga Alliance, Pilates Mat Instructor', N'Thứ 2 - Thứ 4|06:00 - 11:00;Thứ 6|16:00 - 21:00', N'Active', SYSDATETIME()),
                        (N'NV002', N'Trần Văn Hùng', N'Huấn Luyện Viên', N'CrossFit & HIIT', N'/img/team/team-2.jpg', N'0912345678', N'hung@gympro.vn', 11000000, 4.8, 18, 6, N'Tập trung vào giáo án cường độ cao giúp tăng sức bền, đốt mỡ và cải thiện thể lực toàn diện.', N'Mỗi giới hạn đều có thể mở rộng bằng kỷ luật, kỹ thuật đúng và kế hoạch thông minh.', N'Gym Center Thủ Đức', N'CrossFit Level 2, HIIT Specialist, CPR & First Aid', N'Thứ 2 - Thứ 6|17:00 - 22:00;Thứ 7|08:00 - 17:00', N'Active', SYSDATETIME()),
                        (N'NV003', N'Phạm Văn Đức', N'Huấn Luyện Viên', N'Boxing & Kickboxing', N'/img/team/team-3.jpg', N'0923456789', N'duc@gympro.vn', 13000000, 4.9, 22, 10, N'Có nhiều năm thi đấu và đào tạo Boxing, Kickboxing cho người mới lẫn học viên nâng cao.', N'Sức mạnh thật sự đến từ sự bình tĩnh, kiểm soát và tinh thần không bỏ cuộc.', N'Gym Center Phú Nhuận', N'Boxing Coach Level 2, Kickboxing Instructor', N'Thứ 3 - Thứ 6|15:00 - 21:00;Thứ 7 - Chủ nhật|09:00 - 16:00', N'Active', SYSDATETIME()),
                        (N'NV004', N'Lê Thị Hoa', N'Huấn Luyện Viên', N'Zumba & Cardio', N'/img/team/team-4.jpg', N'0934567890', N'hoa@gympro.vn', 10000000, 4.7, 20, 5, N'Mang đến các buổi Zumba giàu năng lượng, kết hợp âm nhạc và cardio dễ tiếp cận.', N'Hãy vận động theo cách khiến bạn muốn quay lại vào ngày mai.', N'Gym Center Hai Bà Trưng', N'Zumba Instructor Network, Group Fitness Instructor', N'Đang cập nhật|HLV hiện nghỉ phép', N'OnLeave', SYSDATETIME()),
                        (N'NV005', N'Hoàng Minh', N'Huấn Luyện Viên', N'Powerlifting', N'/img/team/team-5.jpg', N'0945678901', N'minh@gympro.vn', 11500000, 4.8, 16, 7, N'Chuyên tăng sức mạnh, cải thiện kỹ thuật squat, bench press và deadlift.', N'Sức mạnh được xây từ kỹ thuật đúng, sự kiên nhẫn và tiến bộ đều đặn.', N'Gym Center Cầu Giấy', N'Powerlifting Coach, Strength & Conditioning', N'Thứ 2 - Thứ 6|06:00 - 10:00;Thứ 2 - Thứ 7|17:00 - 21:00', N'Active', SYSDATETIME()),
                        (N'NV006', N'Trần Thị Thu', N'Lễ Tân', N'Chăm sóc KH', NULL, N'0956789012', N'thu@gympro.vn', 7000000, 4.6, 0, 2, NULL, NULL, N'Quầy lễ tân', NULL, NULL, N'Active', SYSDATETIME()),
                        (N'NV007', N'Nguyễn Văn Bình', N'Kỹ Thuật', N'Bảo trì thiết bị', NULL, N'0967890123', N'binh@gympro.vn', 8500000, 4.6, 0, 4, NULL, NULL, N'Khu kỹ thuật', NULL, NULL, N'Active', SYSDATETIME());
                END;

                UPDATE [dbo].[StaffMembers]
                SET [ImageUrl] = CASE [StaffCode]
                    WHEN N'NV001' THEN N'/img/team/team-1.jpg'
                    WHEN N'NV002' THEN N'/img/team/team-2.jpg'
                    WHEN N'NV003' THEN N'/img/team/team-3.jpg'
                    WHEN N'NV004' THEN N'/img/team/team-4.jpg'
                    WHEN N'NV005' THEN N'/img/team/team-5.jpg'
                    ELSE [ImageUrl]
                END
                WHERE ([ImageUrl] IS NULL OR [ImageUrl] = N'') AND [Position] = N'Huấn Luyện Viên';
                """);
        }

        private AdminStaffItemViewModel MapStaff(DbDataReader reader)
        {
            var fullName = ReadString(reader, "FullName");
            var position = ReadString(reader, "Position");
            var status = ReadString(reader, "Status");

            return new AdminStaffItemViewModel
            {
                StaffMemberId = ReadInt(reader, "StaffMemberId"),
                StaffCode = ReadString(reader, "StaffCode"),
                FullName = fullName,
                Initial = BuildInitial(fullName),
                Position = position,
                PositionKind = GetPositionKind(position),
                Specialty = ReadString(reader, "Specialty"),
                ImageUrl = ReadString(reader, "ImageUrl"),
                PhoneNumber = ReadString(reader, "PhoneNumber"),
                Email = ReadString(reader, "Email"),
                Salary = ReadDecimal(reader, "Salary"),
                Rating = ReadDecimal(reader, "Rating"),
                MonthlyClasses = ReadInt(reader, "MonthlyClasses"),
                ExperienceYears = ReadInt(reader, "ExperienceYears"),
                Introduction = ReadString(reader, "Introduction"),
                Philosophy = ReadString(reader, "Philosophy"),
                Location = ReadString(reader, "Location"),
                Certificates = ReadString(reader, "Certificates"),
                ScheduleText = ReadString(reader, "ScheduleText"),
                Status = status,
                StatusKind = GetStatusKind(status),
                StatusLabel = GetStatusLabel(status)
            };
        }

        private static TrainerViewModel MapTrainer(AdminStaffItemViewModel staff)
        {
            var specialty = string.IsNullOrWhiteSpace(staff.Specialty) ? "Cá nhân" : staff.Specialty;

            return new TrainerViewModel
            {
                Id = staff.StaffMemberId,
                Name = staff.FullName,
                Role = $"HLV {specialty}",
                Image = string.IsNullOrWhiteSpace(staff.ImageUrl) ? "/img/team/team-1.jpg" : staff.ImageUrl,
                Status = staff.StatusKind == "leave" ? "Nghỉ phép" : "Sẵn sàng",
                StatusClass = staff.StatusKind == "leave" ? "away" : "available",
                Rating = staff.Rating.ToString("0.0"),
                Experience = $"{staff.ExperienceYears} năm kinh nghiệm",
                Reviews = staff.MonthlyClasses > 0 ? $"{staff.MonthlyClasses} lớp/tháng" : "Đang cập nhật",
                Location = string.IsNullOrWhiteSpace(staff.Location) ? "GymPro Center" : staff.Location,
                Introduction = string.IsNullOrWhiteSpace(staff.Introduction)
                    ? $"{staff.FullName} là huấn luyện viên của GymPro, chuyên đồng hành cùng học viên theo mục tiêu cá nhân."
                    : staff.Introduction,
                Philosophy = string.IsNullOrWhiteSpace(staff.Philosophy)
                    ? "Kết quả bền vững đến từ kỹ thuật đúng, sự đều đặn và một lộ trình phù hợp."
                    : staff.Philosophy,
                Skills = SplitList(staff.Specialty),
                Certificates = SplitList(staff.Certificates, "Certified Personal Trainer"),
                Schedule = ParseSchedule(staff.ScheduleText)
            };
        }

        private async Task<string> BuildNextStaffCodeAsync()
        {
            var nextNumber = 1;

            await WithOpenConnectionAsync(async connection =>
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT ISNULL(MAX([StaffMemberId]), 0) + 1 FROM [StaffMembers];";
                var scalar = await command.ExecuteScalarAsync();
                nextNumber = Convert.ToInt32(scalar);
            });

            return $"NV{nextNumber:000}";
        }

        private async Task ExecuteNonQueryAsync(string sql, params (string Name, object Value)[] parameters)
        {
            await WithOpenConnectionAsync(async connection =>
            {
                await using var command = connection.CreateCommand();
                command.CommandText = sql;
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(CreateParameter(connection, parameter.Name, parameter.Value));
                }

                await command.ExecuteNonQueryAsync();
            });
        }

        private async Task WithOpenConnectionAsync(Func<DbConnection, Task> action)
        {
            var connection = _dbContext.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;

            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                await action(connection);
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private static DbParameter CreateParameter(DbConnection connection, string name, object value)
        {
            using var command = connection.CreateCommand();
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        private static object ToDbValue(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
        }

        private static string GetPositionKind(string position)
        {
            if (position.Contains("Lễ", StringComparison.OrdinalIgnoreCase)) return "reception";
            if (position.Contains("Kỹ", StringComparison.OrdinalIgnoreCase)) return "technical";
            if (position.Contains("Quản", StringComparison.OrdinalIgnoreCase)) return "manager";
            return "trainer";
        }

        private static string GetStatusKind(string status)
        {
            return status switch
            {
                "OnLeave" => "leave",
                "Inactive" => "inactive",
                _ => "active"
            };
        }

        private static string GetStatusLabel(string status)
        {
            return status switch
            {
                "OnLeave" => "Nghỉ phép",
                "Inactive" => "Tạm nghỉ",
                _ => "Đang làm"
            };
        }

        private static string BuildInitial(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0 ? "A" : parts[^1].Substring(0, 1).ToUpperInvariant();
        }

        private static IReadOnlyList<string> SplitList(string? value, string fallback = "Huấn luyện cá nhân")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new[] { fallback };
            }

            return value
                .Split(new[] { ',', '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => item.Length > 0)
                .Take(5)
                .ToList();
        }

        private static IReadOnlyList<TrainerScheduleViewModel> ParseSchedule(string? scheduleText)
        {
            if (string.IsNullOrWhiteSpace(scheduleText))
            {
                return new[] { new TrainerScheduleViewModel { Day = "Đang cập nhật", Time = "Liên hệ tư vấn" } };
            }

            var result = scheduleText
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(item =>
                {
                    var parts = item.Split('|', 2, StringSplitOptions.TrimEntries);
                    return new TrainerScheduleViewModel
                    {
                        Day = parts.Length > 0 ? parts[0] : "Đang cập nhật",
                        Time = parts.Length > 1 ? parts[1] : "Liên hệ tư vấn"
                    };
                })
                .ToList();

            return result.Count == 0
                ? new[] { new TrainerScheduleViewModel { Day = "Đang cập nhật", Time = "Liên hệ tư vấn" } }
                : result;
        }

        private static string ReadString(DbDataReader reader, string name)
        {
            var ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
        }

        private static int ReadInt(DbDataReader reader, string name)
        {
            var ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
        }

        private static decimal ReadDecimal(DbDataReader reader, string name)
        {
            var ordinal = reader.GetOrdinal(name);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
        }
    }
}
