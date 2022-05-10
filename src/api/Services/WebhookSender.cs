using System.Text;
using api.Infrastructure.Database;
using api.Models.Entities;
using api.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuickType;

namespace api.Services;

public class WebhookSender
{
    private const string MediaTypeJson = "application/json";
    private const string DefaultWorkItemUpdatedTitle = "Work item updated";
    private readonly WebhooksDbContext _dbContext;
    private readonly HttpClient _httpClient;

    public WebhookSender(WebhooksDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }

    public async Task<bool> NotifyWorkItemUpdated(AzureWorkItemNotification notification)
    {
        var urlsToNotify = await _dbContext.Webhooks
            .Where(x => x.Type == WebhookType.WorkItemUpdated)
            .Select(x => x.Url)
            .ToListAsync();

        Field? stateField = null;
        notification?.Resource?.Fields?.TryGetValue("System.State", out stateField);

        var discordModel = new DiscordWebhookModel
        {
            Username = "Azure Boards",
            Embeds = new Embed[1]{
                    new Embed
                    {
                        Title = await GetWorkItemUpdatedTitle(stateField),
                        Description = notification?.DetailedMessage?.Markdown ?? "",
                    },
                },
        };

        foreach (var url in urlsToNotify)
        {
            Console.WriteLine($"Sending message to {url}...");
            Console.WriteLine(notification?.DetailedMessage?.Text);
            Console.WriteLine();

            var request = GetRequestMessage(url, HttpMethod.Post, discordModel);
            await _httpClient.SendAsync(request, default(CancellationToken));
        }

        return true;
    }

    private HttpRequestMessage GetRequestMessage(string path, HttpMethod method, object content)
    {
        var uri = new Uri(path);
        var request = new HttpRequestMessage(method, uri);

        if (content != null)
        {
            request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, MediaTypeJson);
        }

        return request;
    }

    public async Task<string> GetWorkItemUpdatedTitle(Field? stateField)
    {
        if (stateField?.OldValue is null || stateField?.NewValue is null)
            return DefaultWorkItemUpdatedTitle;

        var customTitle = await _dbContext.CustomTitles
            .Where(x => x.OldState == stateField.OldValue)
            .Where(x => x.NewState == stateField.NewValue)
            .FirstOrDefaultAsync();

        if (customTitle is default(CustomTitle))
            return DefaultWorkItemUpdatedTitle;

        return customTitle.Title;
    }
}
