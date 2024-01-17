using GitLabAlertBot.Domain.Invites;

namespace GitLabAlertBot.Domain;

public interface IUnitOfWork
{
    IChatRepository Chats { get; }
    IInviteRepository Invites { get; }

    Task SaveChangesAsync();
}
