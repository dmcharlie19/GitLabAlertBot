namespace GitLabAlertBot.PollingAgent.Settings;

public class GitLabAlertSection
{
    public const string SectionName = "GitLabAlert";

    public string ServerUrl { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public int RequestRepeatInSec { get; set; }

    public int GroupId { get; set; }
}
