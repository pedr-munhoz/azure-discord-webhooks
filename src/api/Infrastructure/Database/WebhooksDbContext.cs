using api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Database
{
    public class WebhooksDbContext : DbContext
    {
        public WebhooksDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Webhook> Webhooks { get; set; } = null!;
    }
}