using BotApplication.Contracts;
using BotApplication.Controls.Controllers;
using BotApplication.Controls.Dto;
using GitLabAlertBot.Domain;
using GitLabAlertBot.Domain.Invites;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GitLabAlertBot.Telegram;

public class CommandsHandler : ICommandsController
{
    private readonly ILogger<CommandsHandler> _logger;
    private readonly ITgBot _bot;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TgBotSection _options;

    public CommandsHandler(
        ILogger<CommandsHandler> logger,
        ITgBot bot,
        IUnitOfWork unitOfWork,
        IOptions<TgBotSection> options)
    {
        _logger = logger;
        _bot = bot;
        _unitOfWork = unitOfWork;
        _options = options.Value;
    }

    public async Task HandleCommand(HandleMessageDto messageDto)
    {
        var command = messageDto.MessgeText.Split(' ')[0];
        string? argument = messageDto.MessgeText.Split(' ').ElementAtOrDefault(1);

        switch (command)
        {
            case "/start":
                await OnStart(messageDto, argument);
                break;

            case "/invite":
                await OnInvite(messageDto);
                break;

            default:
                break;
        }
    }

    private async Task OnStart(HandleMessageDto messageDto, string? argument)
    {
        if (await _unitOfWork.Chats.GetAsync(messageDto.ChatId) == null)
        {
            _logger.LogInformation($"New registration! income = {argument}");

            if (argument != null)
            {
                if (Guid.TryParse(argument, out Guid invitingCode))
                {
                    var invite = await _unitOfWork.Invites.GetByCodeAsync(invitingCode);
                    if (invite != null &&
                        invite.IsUsed == false &&
                        !invite.IsExpired())
                    {
                        Chat chat = new(
                           chatId: messageDto.ChatId,
                           username: messageDto.Username);

                        await _unitOfWork.Chats.AddAsync(chat);
                        await _unitOfWork.SaveChangesAsync();

                        await _bot.SendTextMessageAsync(chat.ChatId, "Welcome to alert bot!");
                    }
                }
            }

            await _bot.SendTextMessageAsync(messageDto.ChatId, "Registration not allowed without invite");
        }
    }

    private async Task OnInvite(HandleMessageDto messageDto)
    {
        var chat = await _unitOfWork.Chats.GetAsync(messageDto.ChatId);
        if (chat != null)
        {
            var code = Guid.NewGuid();
            await _unitOfWork.Invites.AddAsync(new Invite(chat.ChatId, code));
            await _unitOfWork.SaveChangesAsync();

            await _bot.SendTextMessageAsync(messageDto.ChatId, $"Invite with url: \n https://t.me/{_options.BotName}?start={code}");
        }
    }
}
