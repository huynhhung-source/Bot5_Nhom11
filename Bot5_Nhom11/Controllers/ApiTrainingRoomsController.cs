using doanweb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Controllers;

[ApiController]
[Route("api/training-rooms")]
public sealed class ApiTrainingRoomsController : ControllerBase
{
    private readonly GymDbContext _dbContext;

    public ApiTrainingRoomsController(GymDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? status = "Active")
    {
        var query = _dbContext.TrainingRooms.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status) &&
            !status.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            var roomStatus = status.Trim();
            query = query.Where(room => room.Status == roomStatus);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(room =>
                room.RoomName.Contains(keyword) ||
                (room.Description != null && room.Description.Contains(keyword)));
        }

        var rooms = await query
            .OrderBy(room => room.RoomName)
            .Select(room => new
            {
                room.TrainingRoomId,
                room.RoomName,
                room.Capacity,
                room.Status,
                room.Description,
                room.ImageUrl,
                UpcomingClassCount = room.Classes != null
                    ? room.Classes.Count(classItem =>
                        classItem.Status != "Cancelled" &&
                        classItem.ClassDate >= DateTime.Today)
                    : 0,
                NextClassDate = room.Classes != null
                    ? room.Classes
                        .Where(classItem =>
                            classItem.Status != "Cancelled" &&
                            classItem.ClassDate >= DateTime.Today)
                        .OrderBy(classItem => classItem.ClassDate)
                        .ThenBy(classItem => classItem.StartTime)
                        .Select(classItem => (DateTime?)classItem.ClassDate)
                        .FirstOrDefault()
                    : null
            })
            .ToListAsync();

        return Ok(new
        {
            total = rooms.Count,
            data = rooms
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await _dbContext.TrainingRooms
            .AsNoTracking()
            .Where(item => item.TrainingRoomId == id)
            .Select(item => new
            {
                item.TrainingRoomId,
                item.RoomName,
                item.Capacity,
                item.Status,
                item.Description,
                item.ImageUrl,
                item.CreatedDate,
                item.UpdatedDate
            })
            .FirstOrDefaultAsync();

        return room == null
            ? NotFound(new { message = "Không tìm thấy phòng tập." })
            : Ok(room);
    }
}
