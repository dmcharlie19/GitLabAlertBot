namespace GitLabAlertBot.Domain.Invites;

public class Invite
{
    public Invite(long inviter, Guid code)
    {
        Inviter = inviter;
        Code = code;
        CreationDate = DateTime.UtcNow;
    }

    public int Id { get; protected set; }

    public long Inviter { get; protected set; }

    public Guid Code { get; protected set; }

    public bool IsUsed { get; protected set; } = false;

    public DateTime CreationDate { get; protected set; }

    public bool IsExpired()
    {
        return (CreationDate - DateTime.Now) > TimeSpan.FromDays(1);
    }

    public void SetUsed()
    {
        IsUsed = true;
    }

    public Chat? Chat { get; protected set; }
}
