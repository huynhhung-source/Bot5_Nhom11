namespace doanweb.Models
{
    public class TrainerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Role { get; set; } = "";
        public string Image { get; set; } = "";
        public string Status { get; set; } = "";
        public string StatusClass { get; set; } = "";
        public string Rating { get; set; } = "";
        public string Experience { get; set; } = "";
        public string Reviews { get; set; } = "";
        public string Introduction { get; set; } = "";
        public string Philosophy { get; set; } = "";
        public string Location { get; set; } = "";
        public IReadOnlyList<string> Skills { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> Certificates { get; set; } = Array.Empty<string>();
        public IReadOnlyList<TrainerScheduleViewModel> Schedule { get; set; } = Array.Empty<TrainerScheduleViewModel>();
    }

    public class TrainerScheduleViewModel
    {
        public string Day { get; set; } = "";
        public string Time { get; set; } = "";
    }

    public static class TrainerCatalog
    {
        public static IReadOnlyList<TrainerViewModel> All { get; } = new List<TrainerViewModel>
        {
            new()
            {
                Id = 1,
                Name = "Nguyễn Thị Mai",
                Role = "HLV Yoga & Pilates",
                Image = "/img/team/team-1.jpg",
                Status = "Sẵn sàng",
                StatusClass = "available",
                Rating = "4.9",
                Experience = "8 năm kinh nghiệm",
                Reviews = "124 đánh giá",
                Location = "Gym Center Nguyễn Huệ",
                Introduction = "Mai chuyên xây dựng các chương trình Yoga và Pilates giúp cải thiện độ dẻo dai, tư thế và khả năng kiểm soát cơ thể. Các buổi tập được điều chỉnh theo thể trạng để học viên tiến bộ an toàn.",
                Philosophy = "Tập luyện không chỉ để thay đổi vóc dáng, mà còn để tìm lại sự cân bằng và nguồn năng lượng tích cực mỗi ngày.",
                Skills = new[] { "Morning Yoga", "Pilates", "Yoga trị liệu", "Giãn cơ" },
                Certificates = new[] { "RYT 500 Yoga Alliance", "Pilates Mat Instructor", "Chứng nhận Yoga trị liệu" },
                Schedule = Schedule("Thứ 2 - Thứ 4", "06:00 - 11:00", "Thứ 6", "16:00 - 21:00", "Chủ nhật", "07:00 - 12:00")
            },
            new()
            {
                Id = 2,
                Name = "Trần Văn Hùng",
                Role = "HLV CrossFit & HIIT",
                Image = "/img/team/team-2.jpg",
                Status = "Sẵn sàng",
                StatusClass = "available",
                Rating = "4.8",
                Experience = "6 năm kinh nghiệm",
                Reviews = "98 đánh giá",
                Location = "Gym Center Thủ Đức",
                Introduction = "Hùng tập trung vào các giáo án cường độ cao giúp tăng sức bền, đốt mỡ và cải thiện thể lực toàn diện. Anh luôn theo sát kỹ thuật để học viên tập mạnh nhưng vẫn đúng và an toàn.",
                Philosophy = "Mỗi giới hạn đều có thể được mở rộng bằng kỷ luật, kỹ thuật đúng và một kế hoạch đủ thông minh.",
                Skills = new[] { "CrossFit", "HIIT Cardio", "Functional Training", "Giảm mỡ" },
                Certificates = new[] { "CrossFit Level 2", "HIIT Specialist", "CPR & First Aid" },
                Schedule = Schedule("Thứ 2 - Thứ 6", "17:00 - 22:00", "Thứ 7", "08:00 - 17:00")
            },
            new()
            {
                Id = 3,
                Name = "Phạm Văn Đức",
                Role = "HLV Boxing & Kickboxing",
                Image = "/img/team/team-3.jpg",
                Status = "Sẵn sàng",
                StatusClass = "available",
                Rating = "4.9",
                Experience = "10 năm kinh nghiệm",
                Reviews = "156 đánh giá",
                Location = "Gym Center Phú Nhuận",
                Introduction = "Đức có nhiều năm thi đấu và đào tạo Boxing, Kickboxing cho cả người mới lẫn học viên nâng cao. Chương trình của anh kết hợp kỹ thuật đối kháng, phản xạ và thể lực.",
                Philosophy = "Sức mạnh thật sự đến từ sự bình tĩnh, khả năng kiểm soát và tinh thần không bỏ cuộc.",
                Skills = new[] { "Boxing", "Kickboxing", "Phản xạ", "Thể lực đối kháng" },
                Certificates = new[] { "Boxing Coach Level 2", "Kickboxing Instructor", "Sports Conditioning" },
                Schedule = Schedule("Thứ 3 - Thứ 6", "15:00 - 21:00", "Thứ 7 - Chủ nhật", "09:00 - 16:00")
            },
            new()
            {
                Id = 4,
                Name = "Lê Thị Hoa",
                Role = "HLV Zumba & Cardio",
                Image = "/img/team/team-4.jpg",
                Status = "Nghỉ phép",
                StatusClass = "away",
                Rating = "4.7",
                Experience = "5 năm kinh nghiệm",
                Reviews = "87 đánh giá",
                Location = "Gym Center Hai Bà Trưng",
                Introduction = "Hoa mang đến những buổi Zumba giàu năng lượng, kết hợp âm nhạc và các bài cardio dễ tiếp cận. Lớp học phù hợp cho người muốn giảm cân và duy trì thói quen vận động vui vẻ.",
                Philosophy = "Một buổi tập hiệu quả không nhất thiết phải nặng nề; hãy vận động theo cách khiến bạn muốn quay lại vào ngày mai.",
                Skills = new[] { "Zumba Dance", "Aerobics", "Dance Cardio", "Giảm cân" },
                Certificates = new[] { "Zumba Instructor Network", "Group Fitness Instructor", "Aerobic Level 2" },
                Schedule = Schedule("Đang cập nhật", "HLV hiện nghỉ phép")
            },
            new()
            {
                Id = 5,
                Name = "Hoàng Minh",
                Role = "HLV Powerlifting & Strength",
                Image = "/img/team/team-5.jpg",
                Status = "Sẵn sàng",
                StatusClass = "available",
                Rating = "4.8",
                Experience = "7 năm kinh nghiệm",
                Reviews = "73 đánh giá",
                Location = "Gym Center Cầu Giấy",
                Introduction = "Minh chuyên tăng sức mạnh, cải thiện kỹ thuật squat, bench press và deadlift. Giáo án được thiết kế dựa trên khả năng vận động, mục tiêu và tốc độ phục hồi của từng học viên.",
                Philosophy = "Sức mạnh được xây từ những lần lặp đúng kỹ thuật, sự kiên nhẫn và tiến bộ nhỏ nhưng đều đặn.",
                Skills = new[] { "Power Lifting", "Strength Training", "Muscle Gain", "Kỹ thuật tạ" },
                Certificates = new[] { "Powerlifting Coach", "Strength & Conditioning", "Mobility Specialist" },
                Schedule = Schedule("Thứ 2 - Thứ 6", "06:00 - 10:00", "Thứ 2 - Thứ 7", "17:00 - 21:00")
            },
            new()
            {
                Id = 6,
                Name = "Vũ Thị Lan",
                Role = "HLV Dinh dưỡng & Tập luyện",
                Image = "/img/team/team-6.jpg",
                Status = "Sẵn sàng",
                StatusClass = "available",
                Rating = "4.8",
                Experience = "4 năm kinh nghiệm",
                Reviews = "52 đánh giá",
                Location = "Gym Center Hải Châu",
                Introduction = "Lan kết hợp huấn luyện cá nhân với tư vấn dinh dưỡng để tạo nên lộ trình toàn diện. Cô ưu tiên những thay đổi thực tế, dễ duy trì và phù hợp với nhịp sống của từng học viên.",
                Philosophy = "Kết quả bền vững đến từ một lối sống bạn có thể yêu thích và duy trì, không phải những giải pháp quá khắt khe.",
                Skills = new[] { "Personal Training", "Nutrition Coaching", "Body Recomposition", "Lifestyle" },
                Certificates = new[] { "Certified Personal Trainer", "Sports Nutrition Coach", "Body Transformation Specialist" },
                Schedule = Schedule("Thứ 2 - Thứ 5", "08:00 - 18:00", "Thứ 7", "08:00 - 14:00")
            }
        };

        public static TrainerViewModel? Find(int id) => All.FirstOrDefault(trainer => trainer.Id == id);

        private static IReadOnlyList<TrainerScheduleViewModel> Schedule(params string[] values)
        {
            var result = new List<TrainerScheduleViewModel>();
            for (var index = 0; index + 1 < values.Length; index += 2)
            {
                result.Add(new TrainerScheduleViewModel { Day = values[index], Time = values[index + 1] });
            }

            return result;
        }
    }
}
