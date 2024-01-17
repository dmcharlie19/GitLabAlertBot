using System.Text;

namespace GitLabAlertBot.PollingAgent.GitLabEvents;

public record MergeRequestEvent(
    int GitLabProjectId,
    string GitLabProjectName,
    string Url,
    //
    string Title,
    string SourceBranch,
    string TargetBranch,
    DateTime CreatedAt,
    string Author,
    int Id,
    int UserNotesCount) : GitLabEvent(GitLabProjectId, GitLabProjectName, Url)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("New merge request");
        sb.AppendLine(GitLabProjectName);
        sb.AppendLine(Title);
        sb.AppendLine($"from \"{SourceBranch}\" to \"{TargetBranch}\" ");
        sb.AppendLine($"by {Author}");
        sb.AppendLine($"at {CreatedAt.ToLocalTime()}");
        sb.AppendLine(Url);

        return sb.ToString();
    }
}