using GitLabAlertBot.PollingAgent.GitLabEventsScrappers;
using GitLabAlertBot.PollingAgent.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GitLabAlertBot.PollingAgent;

public static class ServiceExtensions
{
    public static IServiceCollection AddGitlabPollingServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<GitLabAlertSection>()
            .BindConfiguration(GitLabAlertSection.SectionName);

        services.AddSingleton<IGitLabEventsScrapper, JobRequestEventScrapper>();
        services.AddSingleton<IGitLabEventsScrapper, MergeRequestEventScrapper>();

        services.AddHostedService<GitLabBackgroundService>();

        return services;
    }
}
