using Microsoft.Extensions.Logging;
using Template.API.Dashboard.Events;
using Template.Application.Interfaces;
using Template.Dashboard.Common.Interfaces;
using Template.Domain.Events.Notification;

namespace Template.Dashboard.Events.Handlers;

/// <summary>
/// Handler for SendNotificationIntegrationEvent
/// Sends FCM notification to a single device
/// </summary>
public class SendNotificationIntegrationEventHandler : IIntegrationEventHandler<SendNotificationIntegrationEvent>
{
    private readonly IFcmService _fcmService;
    private readonly ILogger<SendNotificationIntegrationEventHandler> _logger;

    public SendNotificationIntegrationEventHandler(
        IFcmService fcmService,
        ILogger<SendNotificationIntegrationEventHandler> logger)
    {
        _fcmService = fcmService;
        _logger = logger;
    }

    public async Task HandleAsync(SendNotificationIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        foreach (var deviceToken in @event.DeviceTokens)
        {
            var result = await _fcmService.SendNotificationAsync(
                deviceToken,
                @event.Title,
                @event.Body,
                @event.Data,
                cancellationToken);

            if (result)
            {
                _logger.LogInformation(
                    "Successfully sent FCM notification to device: {DeviceToken}",
                    MaskToken(deviceToken));
            }
            else
            {
                _logger.LogWarning(
                    "Failed to send FCM notification to device: {DeviceToken}",
                    MaskToken(deviceToken));
            }
        }
    }
    

    private string MaskToken(string token)
    {
        if (string.IsNullOrEmpty(token) || token.Length <= 8)
            return "****";

        return $"{token.Substring(0, 4)}...{token.Substring(token.Length - 4)}";
    }
}

