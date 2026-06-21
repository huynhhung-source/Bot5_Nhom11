using doanweb.Models;

namespace doanweb.Services;

public interface IGymService
{
    IReadOnlyList<GymLocation> GetAll(string? area = null, string? search = null);
    GymLocation? GetById(int id);
}

public sealed class GymService : IGymService
{
    private static readonly IReadOnlyList<GymLocation> Gyms =
    [
        new() { Id = 1, Area = "hcm", City = "TP. Hồ Chí Minh", Name = "Gym Center Nguyễn Huệ", Address = "68 Nguyễn Huệ, Phường Bến Nghé, Quận 1", Hours = "05:30 - 23:00", Phone = "0901 234 567", ClassName = "Yoga Flow buổi sáng", InstructorName = "Nguyễn Minh Anh", Rating = 4.9m, ImageUrl = "/img/gallery/gallery-1.jpg", Amenities = ["Gym", "Yoga", "Sauna", "PT 1:1"], AvailableSlots = 18, IsFeatured = true },
        new() { Id = 2, Area = "hcm", City = "TP. Hồ Chí Minh", Name = "Gym Center Thủ Đức", Address = "216 Võ Văn Ngân, TP. Thủ Đức", Hours = "05:30 - 22:30", Phone = "0902 345 678", ClassName = "Boxing Fitness", InstructorName = "Trần Quốc Huy", Rating = 4.8m, ImageUrl = "/img/gallery/gallery-2.jpg", Amenities = ["Gym", "Boxing", "Locker"], AvailableSlots = 7 },
        new() { Id = 3, Area = "hcm", City = "TP. Hồ Chí Minh", Name = "Gym Center Phú Nhuận", Address = "180 Phan Xích Long, Quận Phú Nhuận", Hours = "06:00 - 22:30", Phone = "0903 456 789", ClassName = "Zumba Energy", InstructorName = "Lê Thảo Vy", Rating = 4.7m, ImageUrl = "/img/gallery/gallery-3.jpg", Amenities = ["Gym", "Zumba", "PT 1:1"], AvailableSlots = 12 },
        new() { Id = 4, Area = "hanoi", City = "Hà Nội", Name = "Gym Center Cầu Giấy", Address = "88 Trần Thái Tông, Quận Cầu Giấy", Hours = "05:30 - 23:00", Phone = "0904 567 890", ClassName = "Strength Foundation", InstructorName = "Phạm Đức Long", Rating = 4.9m, ImageUrl = "/img/gallery/gallery-4.jpg", Amenities = ["Gym", "Yoga", "Sauna"], AvailableSlots = 25 },
        new() { Id = 5, Area = "hanoi", City = "Hà Nội", Name = "Gym Center Hai Bà Trưng", Address = "250 Bà Triệu, Quận Hai Bà Trưng", Hours = "06:00 - 22:00", Phone = "0905 678 901", ClassName = "Dance Cardio", InstructorName = "Vũ Ngọc Mai", Rating = 4.8m, ImageUrl = "/img/gallery/gallery-5.jpg", Amenities = ["Gym", "Dance", "Locker"], AvailableSlots = 3, IsClosingSoon = true },
        new() { Id = 6, Area = "danang", City = "Đà Nẵng", Name = "Gym Center Hải Châu", Address = "126 Nguyễn Văn Linh, Quận Hải Châu", Hours = "05:30 - 22:30", Phone = "0906 789 012", ClassName = "Aqua Fitness", InstructorName = "Đỗ Hoàng Nam", Rating = 4.9m, ImageUrl = "/img/gallery/gallery-6.jpg", Amenities = ["Gym", "Pool", "Sauna"], AvailableSlots = 16 }
    ];

    public IReadOnlyList<GymLocation> GetAll(string? area = null, string? search = null)
    {
        IEnumerable<GymLocation> query = Gyms;

        if (!string.IsNullOrWhiteSpace(area) && !area.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(gym => gym.Area.Equals(area, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(gym =>
                gym.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                gym.Address.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                gym.City.Contains(search, StringComparison.CurrentCultureIgnoreCase));
        }

        return query.ToList();
    }

    public GymLocation? GetById(int id) => Gyms.FirstOrDefault(gym => gym.Id == id);
}
