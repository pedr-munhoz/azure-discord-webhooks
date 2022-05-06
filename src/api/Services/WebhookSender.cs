using System.Text;
using api.Infrastructure.Database;
using api.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuickType;

namespace api.Services
{
    public class WebhookSender
    {
        private const string MediaTypeJson = "application/json";
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

            var discordModel = new DiscordWebhookModel
            {
                Username = "Azure Boards",
                Embeds = new Embed[1]{
                    new Embed
                    {
                        Title = GetWorkItemUpdatedTitle(notification.Resource.Fields["System.State"]),
                        Description = notification.Message.Markdown,
                    },
                },
            };

            foreach (var url in urlsToNotify)
            {
                Console.WriteLine($"Sending message to {url}...");
                Console.WriteLine(notification.DetailedMessage.Text);
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

        public string GetWorkItemUpdatedTitle(Field field)
        {
            //* Tabela - TitulosMensagem {valorAntigo, valorNovo, titulo}

            switch (field.OldValue, field.NewValue)
            {
                case ("New", "Approved"):
                    return "Tarefa liberada para desenvolvimento";
                case ("New", "Commited"):
                    return "Tarefa aguardando revisão de código";

                default:
                    return "Board Update";
            }
        }
    }
}