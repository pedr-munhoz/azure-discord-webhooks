using System.ComponentModel.DataAnnotations;
using api.Models.Enums;

namespace api.Models.ViewModels;

public class CustomTitleViewModel
{
    [Required]
    public string Title { get; set; } = null!;

    public string? OldState { get; set; }

    public string? NewState { get; set; }

    [Required]
    public WebhookType WebhookType { get; set; } = WebhookType.Invalid;
}
