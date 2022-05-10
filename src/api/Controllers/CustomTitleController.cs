using api.Models.ViewModels;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/custom-title")]
public class CustomTitleController : ControllerBase
{
    private readonly CustomTitleManagement _management;

    public CustomTitleController(CustomTitleManagement management)
    {
        _management = management;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomTitleViewModel model)
    {
        var result = await _management.Create(model);

        if (!result.success)
        {
            return UnprocessableEntity($"Custom title already created - '{model.Title}'");
        }

        return Ok(result.customTitle);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _management.Get();
        return Ok(result.customTitles);
    }

    [HttpGet, Route("{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var result = await _management.Get(id);

        if (!result.success)
            return NotFound($"Entity not found for id: {id}");

        return Ok(result.entity);
    }

    [HttpDelete, Route("{id}")]
    public async Task<IActionResult> Remove([FromRoute] long id)
    {
        var success = await _management.Remove(id);

        if (!success)
            return NotFound($"Entity not found for id: {id}");

        return NoContent();
    }
}
