using doanweb.Services;
using Microsoft.AspNetCore.Mvc;

namespace doanweb.Controllers;

[ApiController]
[Route("api/gyms")]
public sealed class GymsController : ControllerBase
{
    private readonly IGymService _gymService;

    public GymsController(IGymService gymService)
    {
        _gymService = gymService;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? area, [FromQuery] string? search)
    {
        var gyms = _gymService.GetAll(area, search);
        return Ok(new { total = gyms.Count, data = gyms });
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var gym = _gymService.GetById(id);
        return gym is null
            ? NotFound(new { message = "Không tìm thấy phòng tập." })
            : Ok(gym);
    }
}
