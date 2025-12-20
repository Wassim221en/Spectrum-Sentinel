namespace Template.Domain.Events.Notification;

/// <summary>
/// Integration event for sending FCM notification to a single device
/// </summary>
public class SendNotificationIntegrationEvent : IntegrationEvent
{
    public List<string> DeviceTokens { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public Dictionary<string, string>? Data { get; private set; }
   

    public SendNotificationIntegrationEvent(
        List<string> deviceTokens,
        string title,
        string body,
        Dictionary<string, string>? data)
    {
        DeviceTokens = deviceTokens;
        Title = title;
        Body = body;
        Data = data;
    }
}

