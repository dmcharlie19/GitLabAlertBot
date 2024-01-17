using GitLabAlertBot.PollingAgent.GitLabEvents;
using GitLabApiClient;
using GitLabApiClient.Models.Projects.Responses;

namespace GitLabAlertBot.PollingAgent.GitLabEventsScrappers;

public partial interface IGitLabEventsScrapper
{
    Task<PollingResult?> ExecuteAsync(IGitLabClient gitLabClient, Project project, DateTime startTime);
}