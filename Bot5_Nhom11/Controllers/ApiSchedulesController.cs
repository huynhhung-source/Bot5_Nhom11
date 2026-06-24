using doanweb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Controllers;

[ApiController]
[Route("api/schedules")]
public sealed class ApiSchedulesController : ControllerBase
{
    private readonly GymDbContext _dbContext;

    public ApiSchedulesController(GymDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? roomId,
        [FromQuery] string? trainer,
        [FromQuery] string? status)
    {
        var from = (fromDate ?? DateTime.Today).Date;
        var to = (toDate ?? from.AddDays(7)).Date;

        if (to < from)
        {
            return BadRequest(new { message = "toDate phải lớn hơn hoặc bằng fromDate." });
        }

        var query = _dbContext.Classes
            .AsNoTracking()
            .Where(classItem => classItem.ClassDate >= from && classItem.ClassDate < to.AddDays(1));

        if (roomId.HasValue)
        {
            query = query.Where(classItem => classItem.TrainingRoomId == roomId.Value);
        }

        if (!string.IsNullOrWhiteSpace(trainer))
        {
            var trainerName = trainer.Trim();
            query = query.Where(classItem => classItem.InstructorName.Contains(trainerName));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var scheduleStatus = status.Trim();
            query = query.Where(classItem => classItem.Status == scheduleStatus);
        }
        else
        {
            query = query.Where(classItem => classItem.Status != "Cancelled");
        }

        var schedules = await query
            .OrderBy(classItem => classItem.ClassDate)
            .ThenBy(classItem => classItem.StartTime)
            .Select(classItem => new
            {
                classItem.ClassId,
                classItem.ClassName,
                classItem.ClassType,
                classItem.Level,
                classItem.Description,
                classItem.InstructorName,
                classItem.ClassDate,
                classItem.StartTime,
                classItem.EndTime,
                classItem.TrainingRoomId,
                RoomName = classItem.TrainingRoom != null
                    ? classItem.TrainingRoom.RoomName
                    : classItem.Location,
                classItem.MaxCapacity,
                RegisteredCount = classItem.Enrollments != null
                    ? classItem.Enrollments.Count(enrollment => enrollment.Status != "Cancelled")
                    : classItem.CurrentEnrollment,
                AvailableSlots = Math.Max(
                    0,
                    classItem.MaxCapacity -
                    (classItem.Enrollments != null
                        ? classItem.Enrollments.Count(enrollment => enrollment.Status != "Cancelled")
                        : classItem.CurrentEnrollment)),
                classItem.Status
            })
            .ToListAsync();

        return Ok(new
        {
            fromDate = from,
            toDate = to,
            total = schedules.Count,
            data = schedules
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var schedule = await _dbContext.Classes
            .AsNoTracking()
            .Where(classItem => classItem.ClassId == id)
            .Select(classItem => new
            {
                classItem.ClassId,
                classItem.ClassName,
                classItem.ClassType,
                classItem.Level,
                classItem.Description,
                classItem.InstructorName,
                classItem.ClassDate,
                classItem.StartTime,
                classItem.EndTime,
                classItem.TrainingRoomId,
                RoomName = classItem.TrainingRoom != null
                    ? classItem.TrainingRoom.RoomName
                    : classItem.Location,
                classItem.MaxCapacity,
                RegisteredCount = classItem.Enrollments != null
                    ? classItem.Enrollments.Count(enrollment => enrollment.Status != "Cancelled")
                    : classItem.CurrentEnrollment,
                classItem.Status
            })
            .FirstOrDefaultAsync();

        return schedule == null
            ? NotFound(new { message = "Không tìm thấy lịch tập." })
            : Ok(schedule);
    }
}
