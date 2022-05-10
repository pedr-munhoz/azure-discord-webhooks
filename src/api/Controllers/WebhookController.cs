using api.Models.ViewModels;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/webhook")]
public class WebhookController : ControllerBase
{
    private readonly WebhookManagement _management;

    public WebhookController(WebhookManagement management)
    {
        _management = management;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WebhookViewModel model)
    {
        var result = await _management.Create(model);
        return Ok(result.webhook);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _management.Get();
        return Ok(result.webhooks);
    }

    [HttpGet, Route("{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var result = await _management.Get(id);

        if (!result.success)
            return NotFound($"Entity not found for id: {id}");

        return Ok(result.webhook);
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
