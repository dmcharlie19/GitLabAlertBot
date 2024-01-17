using BotApplication.Contracts;
using BotApplication.Controls.Dto;
using GitLabAlertBot.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TelegramImageBot.Application;

namespace GitLabAlertBot.Telegram;

public class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<UpdateHandler> _logger;
    private IServiceScopeFactory _scopeFactory;

    public UpdateHandler(ILogger<UpdateHandler> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task HandleUpdate(IHandleDto handlerData)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        try
        {
            switch (handlerData)
            {
                case HandleMessageDto handleMessage:
                    if (handleMessage.MessgeText.StartsWith("/"))
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<CommandsHandler>();
                        await handler.HandleCommand(handleMessage);
                    }
                    break;

                case HandleChatBlockDto chatBlockDto:
                    var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var chat = await uof.Chats.GetAsync(chatBlockDto.ChatId) ?? throw new InvalidOperationException();
                    chat.IsBlocked = true;
                    await uof.SaveChangesAsync();
                    break;

                default:
                    _logger.LogError("Unsupported update type");
                    break;
            }
        }
        catch
        {
            throw;
        }
    }
}
