using GitLabAlertBot.Domain;
using GitLabAlertBot.Domain.Invites;
using GitLabAlertBot.Infrastructure;
using TelegramImageBot.Infrastructure.Repositories;

namespace EmptyBot.Contracts.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BotDbContext _dbContext;
    private readonly ChatRepository _chatRepository;
    private readonly InviteRepository _inviteRepository;

    public UnitOfWork(BotDbContext dbContext)
    {
        _dbContext = dbContext;
        _chatRepository = new(_dbContext);
        _inviteRepository = new(_dbContext);
    }

    public IChatRepository Chats => _chatRepository;

    public IInviteRepository Invites => _inviteRepository;

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
