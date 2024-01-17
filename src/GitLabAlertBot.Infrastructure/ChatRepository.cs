using GitLabAlertBot.Domain;
using GitLabAlertBot.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace TelegramImageBot.Infrastructure.Repositories;

internal class ChatRepository : IChatRepository
{
    private readonly BotDbContext _dbContext;

    public ChatRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Chat chat)
    {
        await _dbContext.Chats.AddAsync(chat);
    }

    public async Task<Chat?> GetAsync(long chatId)
    {
        return await _dbContext.Chats.FirstOrDefaultAsync(chat => chat.ChatId == chatId);
    }

    public async Task<Chat?> GetByUsernameAsync(string username)
    {
        return await _dbContext.Chats.FirstOrDefaultAsync(chat => chat.Username == username);
    }

    public async Task<List<long>> GetChatsIdNotBlockedAsync(int skip, int take)
    {
        return await _dbContext.Chats
            .OrderBy(c => c.ChatId)
            .Where(chat => chat.IsBlocked == false)
            .Skip(skip)
            .Take(take)
            .Select(chat => chat.ChatId)
            .ToListAsync();
    }
}
