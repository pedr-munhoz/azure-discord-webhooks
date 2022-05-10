using api.Infrastructure.Database;
using api.Models.Entities;
using api.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class CustomTitleManagement
{
    private readonly WebhooksDbContext _dbContext;

    public CustomTitleManagement(WebhooksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(bool success, CustomTitle? customTitle)> Create(CustomTitleViewModel model)
    {
        var existingTitle = await Get(oldState: model.OldState, newState: model.NewState);

        if (existingTitle.success)
        {
            return (false, existingTitle.entity);
        }

        var entity = new CustomTitle
        {
            Title = model.Title,
            OldState = model.OldState,
            NewState = model.NewState,
            WebhookType = model.WebhookType,
        };

        await _dbContext.CustomTitles.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return (true, entity);
    }

    public async Task<(bool success, IEnumerable<CustomTitle> customTitles)> Get()
    {
        var entities = await _dbContext.CustomTitles.ToListAsync();
        return (true, entities);
    }

    public async Task<(bool success, CustomTitle? entity)> Get(long id)
    {
        var entity = await _dbContext.CustomTitles.Where(x => x.Id == id).FirstOrDefaultAsync();

        if (entity is null)
        {
            return (false, null);
        }

        return (true, entity);
    }

    public async Task<(bool success, CustomTitle? entity)> Get(string? oldState, string? newState)
    {
        var entity = await _dbContext.CustomTitles
            .Where(x => x.OldState == oldState)
            .Where(x => x.NewState == newState)
            .FirstOrDefaultAsync();

        if (entity is null)
        {
            return (false, null);
        }

        return (true, entity);
    }

    public async Task<(bool success, CustomTitle? entity)> GetClosestMatch(string? oldState, string? newState)
    {
        // retrieves every viable match from the DB
        var entities = await _dbContext.CustomTitles
            .Where(x => x.OldState == oldState || x.OldState == null)
            .Where(x => x.NewState == newState || x.NewState == null)
            .ToListAsync();

        // checks for the exact match
        var entity = entities
            .Where(x => x.OldState == oldState)
            .Where(x => x.NewState == newState)
            .FirstOrDefault();

        if (entity is not null)
        {
            return (true, entity);
        }

        // checks for a new state match
        entity = entities
            .Where(x => x.OldState == null)
            .Where(x => x.NewState == newState)
            .FirstOrDefault();

        if (entity is not null)
        {
            return (true, entity);
        }

        // checks for a old state match
        entity = entities
            .Where(x => x.OldState == oldState)
            .Where(x => x.NewState == null)
            .FirstOrDefault();

        // checks for a all null (default) match
        entity = entities
            .Where(x => x.OldState == null)
            .Where(x => x.NewState == null)
            .FirstOrDefault();

        if (entity is not null)
        {
            return (true, entity);
        }

        return (false, null);
    }

    public async Task<bool> Remove(long id)
    {
        var existingTitle = await Get(id);

        if (!existingTitle.success || existingTitle.entity is null)
        {
            return false;
        }

        _dbContext.CustomTitles.Remove(existingTitle.entity);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
