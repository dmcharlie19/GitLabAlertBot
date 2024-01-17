namespace GitLabAlertBot.Domain.Invites;

public interface IInviteRepository
{
    Task AddAsync(Invite invite);

    Task<Invite?> GetByCodeAsync(Guid code);
}