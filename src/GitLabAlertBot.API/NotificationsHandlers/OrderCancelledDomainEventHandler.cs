using GitLabAlertBot.PollingAgent.GitLabEvents;
using GitLabAlertBot.Telegram;
using MediatR;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.DomainEventHandlers;

public partial class OrderCancelledDomainEventHandler
                : INotificationHandler<PollingResult>
{
    private readonly BroadcastsProcessor _broadcastsProcessor;

    public OrderCancelledDomainEventHandler(BroadcastsProcessor broadcastsProcessor)
    {
        _broadcastsProcessor = broadcastsProcessor;
    }

    public async Task Handle(PollingResult notification, CancellationToken cancellationToken)
    {
        foreach (var item in notification.GitLabEvents)
        {
            await _broadcastsProcessor.ExecuteBroadcastAsync(item.ToString());
        }
    }
}
