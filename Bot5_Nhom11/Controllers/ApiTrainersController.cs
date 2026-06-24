using doanweb.Services;
using Microsoft.AspNetCore.Mvc;

namespace doanweb.Controllers;

[ApiController]
[Route("api/trainers")]
public sealed class ApiTrainersController : ControllerBase
{
    private readonly IStaffDirectoryService _staffDirectoryService;

    public ApiTrainersController(IStaffDirectoryService staffDirectoryService)
    {
        _staffDirectoryService = staffDirectoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var trainers = await _staffDirectoryService.GetTrainerViewModelsAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            trainers = trainers
                .Where(trainer =>
                    trainer.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    trainer.Role.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    trainer.Location.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    trainer.Skills.Any(skill =>
                        skill.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        return Ok(new
        {
            total = trainers.Count,
            data = trainers
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var trainer = await _staffDirectoryService.GetTrainerViewModelAsync(id);

        return trainer == null
            ? NotFound(new { message = "Không tìm thấy huấn luyện viên." })
            : Ok(trainer);
    }
}
