using Microsoft.AspNetCore.Mvc;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Services;

namespace Siemens.Internship2026.GradeBook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemReader _reader;
    private readonly IGradeService _gradeService;

    public ItemController(IItemReader reader, IGradeService gradeService)
    {
        _reader = reader;
        _gradeService = gradeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _reader.GetAllAsync();
        var itemList = items.ToList();

        var totalCount = _gradeService.GetTotalCount(itemList);
        var averageValue = _gradeService.CalculateAverage(itemList);

        return Ok(new
        {
            Data = itemList,
            Statistics = new
            {
                TotalCount = totalCount,
                AverageValue = averageValue,
                RetrievedAt = DateTime.UtcNow
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
        {
            Console.WriteLine($"[LOG] Invalid id: {id}");
            return BadRequest("Id must be a positive integer.");
        }

        var item = await _reader.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound($"Item with Id {id} was not found.");
        }

        return Ok(item);
    }
}
