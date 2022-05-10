using api.Models.Enums;

namespace api.Models.Entities;

public class CustomTitle
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string? OldState { get; set; }
    public string? NewState { get; set; }
    public WebhookType WebhookType { get; set; } = WebhookType.Invalid;
}
