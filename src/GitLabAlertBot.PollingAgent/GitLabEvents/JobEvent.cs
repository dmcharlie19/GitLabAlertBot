using System.Text;

namespace GitLabAlertBot.PollingAgent.GitLabEvents;

public record JobEvent(
    int GitLabProjectId,
    string GitLabProjectName,
    string Url,
    //
    string? Branch,
    string UserName,
    DateTime? StartedAt,
    double Duration,
    string? Status,
    string? Stage,
    DateTime? FinishedAt) : GitLabEvent(GitLabProjectId, GitLabProjectName, Url)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Job info");
        sb.AppendLine($"on {GitLabProjectName} by {UserName}");
        sb.AppendLine($"Status: {Status}");
        sb.AppendLine($"Stage: {Stage}");
        sb.AppendLine($"Duration: {Duration} sec.");
        if (StartedAt.HasValue && FinishedAt.HasValue)
        {
            sb.AppendLine($"Started at: {StartedAt.Value.ToLocalTime()}");
            sb.AppendLine($"Est. duration: {(FinishedAt - StartedAt).Value.TotalSeconds} sec.");
        }
        sb.AppendLine(Url);

        return sb.ToString();
    }
}