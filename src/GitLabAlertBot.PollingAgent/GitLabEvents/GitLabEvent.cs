using MediatR;

namespace GitLabAlertBot.PollingAgent.GitLabEvents;

public abstract record GitLabEvent(int GitLabProjectId, string GitLabProjectName, string Url);
