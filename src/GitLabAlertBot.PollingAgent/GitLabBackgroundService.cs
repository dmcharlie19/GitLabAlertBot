using GitLabAlertBot.PollingAgent.GitLabEventsScrappers;
using GitLabApiClient;
using GitLabApiClient.Models.Groups.Responses;
using GitLabApiClient.Models.Projects.Responses;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MediatR;
using GitLabAlertBot.PollingAgent.Settings;
using GitLabAlertBot.PollingAgent.GitLabEvents;
using Microsoft.Extensions.DependencyInjection;

namespace GitLabAlertBot.PollingAgent;

public class GitLabBackgroundService : BackgroundService
{
    private readonly IEnumerable<IGitLabEventsScrapper> _eventsScrappers;
    private readonly ILogger<GitLabBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly GitLabAlertSection _settings;
    private readonly IGitLabClient _client;

    public GitLabBackgroundService(
        IEnumerable<IGitLabEventsScrapper> eventsScrappers,
        IOptions<GitLabAlertSection> settings,
        ILogger<GitLabBackgroundService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _settings = settings.Value;
        _eventsScrappers = eventsScrappers;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _client = new GitLabClient(_settings.ServerUrl, _settings.Token);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PeriodicTimer periodicTimer = new(TimeSpan.FromSeconds(_settings.RequestRepeatInSec));
        DateTime lastCollectionTime = DateTime.UtcNow - TimeSpan.FromMinutes(60);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecutePolling(lastCollectionTime);
                lastCollectionTime = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to execute gitlab events polling, msg = {0}", ex.Message);
            }

            await periodicTimer.WaitForNextTickAsync(stoppingToken);
        }
    }

    private async Task ExecutePolling(DateTime lastCollectionTime)
    {
        var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();


        await foreach (PollingResult result in CollectGitlabNewEventsAsync(lastCollectionTime))
        {
            try
            {
                await mediator.Publish(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed handle polling result, ex msg = {ex.Message}");
            }
        }
    }

    private async IAsyncEnumerable<PollingResult> CollectGitlabNewEventsAsync(DateTime lastCollectionTime)
    {
        Group group = await _client.Groups.GetAsync(_settings.GroupId);

        if (group == null)
        {
            throw new Exception($"Group {_settings.GroupId} seems not to exist.");
        }

        IList<Project>? projects = group.Projects;

        foreach (Project? project in projects)
        {
            foreach (var scrapper in _eventsScrappers)
            {
                try
                {
                    var result = await scrapper.ExecuteAsync(_client, project, lastCollectionTime);
                    if (result is not null &&
                        result.GitLabEvents.Any())
                    {
                        yield return result;
                    }
                }
                finally
                {
                }
            }
        }
    }


}
