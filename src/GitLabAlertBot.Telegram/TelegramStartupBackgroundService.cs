using BotApplication.Contracts;
using GitLabAlertBot.Domain;
using GitLabAlertBot.Telegram;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TelegramImageBot.Application;

namespace GitLabAlertBot.PollingAgent;

public class TelegramStartupBackgroundService : BackgroundService
{
    private readonly IUpdateHandler _updateHandler;
    private readonly ITgBot _bot;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TgBotSection _options;
    private readonly ILogger<TelegramStartupBackgroundService> _logger;

    public TelegramStartupBackgroundService(
        ILogger<TelegramStartupBackgroundService> logger,
        IUpdateHandler updateHandler,
        ITgBot bot,
        IOptions<TgBotSection> options,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _updateHandler = updateHandler;
        _bot = bot;
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await AddSupervisors();
            await _bot.StartBot(_options.Token, _updateHandler.HandleUpdate);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to execute gitlab events polling, msg = {0}", ex.Message);
        }
    }

    private async Task AddSupervisors()
    {
        using var scope = _scopeFactory.CreateScope();
        var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        foreach (var user in _options.Supervisors)
        {
            if (await uof.Chats.GetAsync(user) == null)
            {
                await uof.Chats.AddAsync(new Chat(user, string.Empty));
            }
        }

        await uof.SaveChangesAsync();
    }
}
