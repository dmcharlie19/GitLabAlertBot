using BotApplication.Contracts;
using GitLabAlertBot.Domain;
using Microsoft.Extensions.Logging;

namespace GitLabAlertBot.Telegram;

public class BroadcastsProcessor
{
    private readonly ILogger<BroadcastsProcessor> _logger;
    private readonly ITgBot _bot;
    private readonly IUnitOfWork _unitOfWork;
    private CancellationTokenSource _cts = new();

    public BroadcastsProcessor(ILogger<BroadcastsProcessor> logger, ITgBot bot, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _bot = bot;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteBroadcastAsync(string message)
    {
        int chatsCounter = 0;
        int sendCounter = 0;
        int failSendCounter = 0;
        const int Take = 100;
        List<long> chats;

        while (!_cts.IsCancellationRequested)
        {
            chats = await _unitOfWork.Chats.GetChatsIdNotBlockedAsync(chatsCounter, Take);
            chatsCounter += chats.Count();

            if (chats.Count == 0)
            {
                _logger.LogInformation($"Broadcasting message completed. Sended {sendCounter} messages, failed {failSendCounter} messages");
                break;
            }

            foreach (var chatId in chats)
            {
                try
                {
                    await _bot.SendTextMessageAsync(chatId, message);
                    sendCounter++;
                }
                catch (Exception ex)
                {
                    failSendCounter++;
                    _logger.LogError($"Failed send broadcast msg to user {chatId}, ex msg = {ex.Message}");

                    if (ex.Message.Contains("Forbidden: bot was blocked by the user"))
                    {
                        var chat = await _unitOfWork.Chats.GetAsync(chatId);
                        if (chat != null)
                        {
                            chat.IsBlocked = true;
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromMilliseconds(50));
            }
        }
    }
}