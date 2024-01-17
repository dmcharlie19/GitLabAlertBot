using BotApplication.Contracts;
using BotApplication.Telegram;
using GitLabAlertBot.PollingAgent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramImageBot.Application;

namespace GitLabAlertBot.Telegram;

public static class ServiceExtensions
{
    public static IServiceCollection AddTelegramServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<TgBotSection>()
            .BindConfiguration(TgBotSection.SectionName);

        services.AddSingleton<ITgBot, TgBot>();
        services.AddSingleton<IUpdateHandler, UpdateHandler>();
        services.AddScoped<CommandsHandler>();
        services.AddScoped<BroadcastsProcessor>();

        services.AddHostedService<TelegramStartupBackgroundService>();

        return services;
    }
}
