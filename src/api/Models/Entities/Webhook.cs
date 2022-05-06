using api.Models.Enums;

namespace api.Models.Entities
{
    public class Webhook
    {
        public long Id { get; set; }
        public string Url { get; set; } = null!;
        public WebhookType Type { get; set; } = WebhookType.Invalid;
    }
}