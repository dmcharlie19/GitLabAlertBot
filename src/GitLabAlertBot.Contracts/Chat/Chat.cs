namespace GitLabAlertBot.Domain;

public class Chat
{
    public Chat(long chatId, string username)
    {
        ChatId = chatId;
        Username = username;

        RegistrationDate = DateTime.UtcNow;
    }

    public long ChatId { get; protected set; }
    public string Username { get; protected set; } = string.Empty;
    public bool IsBlocked { get; set; } = false;

    public DateTime RegistrationDate { get; protected set; }
}
