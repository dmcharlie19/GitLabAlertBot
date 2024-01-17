namespace GitLabAlertBot.Domain;

public interface IChatRepository
{
    Task<Chat?> GetAsync(long chatId);

    Task<Chat?> GetByUsernameAsync(string username);

    Task<List<long>> GetChatsIdNotBlockedAsync(int skip, int take);

    Task AddAsync(Chat chat);
}