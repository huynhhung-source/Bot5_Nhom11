namespace doanweb.Models;

public sealed class PublicGymsViewModel
{
    public IReadOnlyList<PublicGymRoomViewModel> Rooms { get; init; } = [];
    public IReadOnlyList<Package> Packages { get; init; } = [];
}

public sealed class PublicGymRoomViewModel
{
    public int TrainingRoomId { get; init; }
    public int? NextClassId { get; init; }
    public string RoomName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Capacity { get; init; }
    public int AvailableSlots { get; init; }
    public int UpcomingClassCount { get; init; }
    public string NextClassName { get; init; } = "Chưa có lịch tập";
    public string InstructorName { get; init; } = "Đang cập nhật";
    public string ScheduleText { get; init; } = "Chưa có lịch sắp tới";
    public string ImageUrl { get; init; } = "/img/gallery/gallery-1.jpg";
    public IReadOnlyList<string> ClassTypes { get; init; } = [];
}
