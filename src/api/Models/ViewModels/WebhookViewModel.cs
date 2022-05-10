using System.ComponentModel.DataAnnotations;
using api.Models.Enums;

namespace api.Models.ViewModels;

public class WebhookViewModel
{
    [Required]
    public string Url { get; set; } = null!;

    [Required]
    public WebhookType Type { get; set; } = WebhookType.Invalid;
}
