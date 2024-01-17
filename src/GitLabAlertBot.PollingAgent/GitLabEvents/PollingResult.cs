using MediatR;

namespace GitLabAlertBot.PollingAgent.GitLabEvents;

public record PollingResult(
    string ProjectUrl,
    int ProjectId,
    string ProjectName,
    IReadOnlyCollection<GitLabEvent> GitLabEvents) : INotification;
