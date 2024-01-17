using System.Text;

namespace GitLabAlertBot.PollingAgent.GitLabEvents;

public record MergeRequestComment(
    string Body,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string UserName)
{
    public override string ToString()
    {
        return @$"
    ""{Body}""
    from {UserName}
    created at {CreatedAt.ToLocalTime()}
    updated at {UpdatedAt.ToLocalTime()}";
    }
}

public record MergeRequestCommentsEvent(
    int GitLabProjectId,
    string GitLabProjectName,
    string Url,
    //
    List<MergeRequestComment> Comments) : GitLabEvent(GitLabProjectId, GitLabProjectName, Url)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("New comment in merge request");
        sb.AppendLine();
        sb.AppendLine($"Project: {GitLabProjectName}");
        sb.AppendLine(Url);
        sb.AppendLine();

        foreach (var comment in Comments)
        {
            sb.AppendLine(comment.ToString());
        }
        return sb.ToString();
    }
}