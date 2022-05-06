using api.Services;
using Microsoft.AspNetCore.Mvc;
using QuickType;

namespace api.Controllers
{
    [ApiController]
    [Route("api/boards/notify")]
    public class BoardsNotificationsController : ControllerBase
    {
        private readonly WebhookSender _sender;

        public BoardsNotificationsController(WebhookSender sender)
        {
            _sender = sender;
        }

        [HttpPost, Route("work-item-updated")]
        public async Task<IActionResult> WorkItemUpdated([FromBody] AzureWorkItemNotification model)
        {
            await _sender.NotifyWorkItemUpdated(model);

            return Ok(model.DetailedMessage.Text);
        }
    }
}