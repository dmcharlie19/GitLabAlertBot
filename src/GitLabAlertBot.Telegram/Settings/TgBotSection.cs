namespace GitLabAlertBot.Telegram;

public class TgBotSection
{
    public const string SectionName = "TgBot";

    public string Token { get; set; } = string.Empty;
    public string BotName { get; set; } = string.Empty;
    public List<long> Supervisors { get; set; } = new();
}
