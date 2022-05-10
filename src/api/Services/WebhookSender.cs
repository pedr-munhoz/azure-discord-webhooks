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
    private readonly HttpClient _httpClient;
    private readonly WebhookManagement _webhooksManagement;
    private readonly CustomTitleManagement _titleManagement;

    public WebhookSender(HttpClient httpClient, WebhookManagement webhooksManagement, CustomTitleManagement titleManagement)
    {
        _httpClient = httpClient;
        _webhooksManagement = webhooksManagement;
        _titleManagement = titleManagement;
    }

    public async Task<bool> NotifyWorkItemUpdated(AzureWorkItemNotification notification)
    {
        var urlsToNotify = await _webhooksManagement.GetUrlsByType(WebhookType.WorkItemUpdated);

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
        if (stateField is null)
            return DefaultWorkItemUpdatedTitle;

        var customTitle = await _titleManagement.GetClosestMatch(oldState: stateField?.OldValue, newState: stateField?.NewValue);

        if (!customTitle.success || customTitle.entity is null)
            return DefaultWorkItemUpdatedTitle;

        return customTitle.entity.Title;
    }
}
