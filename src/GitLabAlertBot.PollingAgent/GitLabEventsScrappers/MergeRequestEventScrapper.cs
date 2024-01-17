using GitLabAlertBot.PollingAgent.GitLabEvents;
using GitLabApiClient;
using GitLabApiClient.Models;
using GitLabApiClient.Models.MergeRequests.Requests;
using GitLabApiClient.Models.MergeRequests.Responses;
using GitLabApiClient.Models.Notes.Responses;
using GitLabApiClient.Models.Projects.Responses;

namespace GitLabAlertBot.PollingAgent.GitLabEventsScrappers;

public class MergeRequestEventScrapper : IGitLabEventsScrapper
{
    public async Task<PollingResult?> ExecuteAsync(IGitLabClient gitLabClient, Project project, DateTime startTime)
    {
        if (!project.MergeRequestsEnabled)
        {
            return null;
        }

        IList<MergeRequest> mergeRequests = await gitLabClient.MergeRequests.GetAsync(project.Id,
            querryOptions => querryOptions.State = QueryMergeRequestState.Opened);

        if (mergeRequests.Count == 0)
        {
            return null;
        }

        List<GitLabEvent> events = new(mergeRequests.Count);
        foreach (var mr in mergeRequests)
        {
            if (mr.CreatedAt > startTime)
            {
                events.Add(new MergeRequestEvent(
                     GitLabProjectId: project.Id,
                     GitLabProjectName: project.NameWithNamespace,
                     Url: mr.WebUrl,

                     Title: mr.Title,
                     SourceBranch: mr.SourceBranch,
                     TargetBranch: mr.TargetBranch,
                     CreatedAt: mr.CreatedAt,
                     Author: mr.Author.Name,
                     Id: mr.Id,
                     UserNotesCount: mr.UserNotesCount));
            }

            IList<Note> notes = await gitLabClient.MergeRequests.GetNotesAsync(project.Id, mr.Iid,
                queryOptions => queryOptions.SortOrder = SortOrder.Ascending);

            notes = notes.Where(n => n.CreatedAt > startTime || n.UpdatedAt > startTime).ToList();
            if (notes.Any())
            {
                events.Add(new MergeRequestCommentsEvent(
                    GitLabProjectId: project.Id,
                    GitLabProjectName: project.NameWithNamespace,
                    Url: mr.WebUrl,
                    Comments: notes
                        .Select(n => Map(n))
                        .ToList()));
            }
        }

        return new PollingResult(
            ProjectUrl: project.WebUrl,
            ProjectId: project.Id,
            ProjectName: project.NameWithNamespace,
            GitLabEvents: events);
    }

    private MergeRequestComment Map(Note note)
    {
        return new MergeRequestComment(
            Body: note.Body,
            CreatedAt: note.CreatedAt,
            UpdatedAt: note.UpdatedAt,
            UserName: note.Author.Name);
    }
}
