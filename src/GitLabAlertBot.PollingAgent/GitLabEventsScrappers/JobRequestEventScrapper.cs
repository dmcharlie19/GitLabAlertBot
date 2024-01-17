using GitLabAlertBot.PollingAgent.GitLabEvents;
using GitLabApiClient;
using GitLabApiClient.Models.Job.Responses;
using GitLabApiClient.Models.Projects.Responses;

namespace GitLabAlertBot.PollingAgent.GitLabEventsScrappers;

public class JobRequestEventScrapper : IGitLabEventsScrapper
{
    public async Task<PollingResult?> ExecuteAsync(IGitLabClient gitLabClient, Project project, DateTime startTime)
    {
        // NOTE project.PublicJobs -- надо как-то учитывать?
        if (!project.JobsEnabled)
        {
            return null;
        }

        IList<Job> jobs = await gitLabClient.Projects.GetJobsAsync(project);

        return new PollingResult(project.WebUrl, project.Id, project.NameWithNamespace, jobs.Select(j => Map(project, j))
            .Where(je => je.StartedAt > startTime)
            .ToArray());
    }

    private JobEvent Map(Project project, Job j)
    {
        return new JobEvent(
            GitLabProjectId: project.Id,
            GitLabProjectName: project.NameWithNamespace,
            j.WebUrl,

            Branch: j.Ref,
            UserName: j.User is not null ? j.User.Name : string.Empty,
            StartedAt: j.StartedAt,
            Duration: j.Duration,
            Status: j.Status,
            Stage: j.Stage,
            FinishedAt: j.FinishedAt);
    }
}
