using GitLabAlertBot.Domain.Invites;
using Microsoft.EntityFrameworkCore;

namespace GitLabAlertBot.Infrastructure;

public class InviteRepository : IInviteRepository
{
    private readonly BotDbContext _dbContext;

    public InviteRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Invite invite)
    {
        await _dbContext.Invites.AddAsync(invite);
    }

    public async Task<Invite?> GetByCodeAsync(Guid code)
    {
        return await _dbContext.Invites.FirstOrDefaultAsync(x => x.Code == code);
    }
}
