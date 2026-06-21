namespace doanweb.Models;

public sealed class GymLocation
{
    public int Id { get; init; }
    public string Area { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Hours { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string ClassName { get; init; } = string.Empty;
    public string InstructorName { get; init; } = string.Empty;
    public decimal Rating { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public IReadOnlyList<string> Amenities { get; init; } = [];
    public int AvailableSlots { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsClosingSoon { get; init; }
}
