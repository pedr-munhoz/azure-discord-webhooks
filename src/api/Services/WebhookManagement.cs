using System.Collections;
using api.Infrastructure.Database;
using api.Models.Entities;
using api.Models.Enums;
using api.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class WebhookManagement
    {
        private readonly WebhooksDbContext _dbContext;

        public WebhookManagement(WebhooksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(bool success, Webhook? webhook)> Create(WebhookViewModel model)
        {
            var entity = new Webhook
            {
                Url = model.Url,
                Type = model.Type,
            };

            await _dbContext.Webhooks.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return (true, entity);
        }

        public async Task<(bool success, IEnumerable<Webhook> webhooks)> Get()
        {
            var entities = await _dbContext.Webhooks.ToListAsync();
            return (true, entities);
        }

        public async Task<(bool success, Webhook? webhook)> Get(long id)
        {
            var entity = await _dbContext.Webhooks.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (entity is null)
            {
                return (false, null);
            }

            return (true, entity);
        }

        public async Task<bool> Remove(long id)
        {
            var entity = await _dbContext.Webhooks.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (entity is null)
            {
                return false;
            }

            _dbContext.Webhooks.Remove(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}