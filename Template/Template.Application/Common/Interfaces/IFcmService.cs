namespace Template.Dashboard.Common.Interfaces;

/// <summary>
/// Interface for Firebase Cloud Messaging (FCM) service
/// </summary>
public interface IFcmService
{
    /// <summary>
    /// Send notification to a single device
    /// </summary>
    Task<bool> SendNotificationAsync(
        string deviceToken, 
        string title, 
        string body, 
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification to multiple devices
    /// </summary>
    Task<FcmBatchResponse> SendNotificationToMultipleDevicesAsync(
        List<string> deviceTokens, 
        string title, 
        string body, 
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification to a topic
    /// </summary>
    Task<bool> SendNotificationToTopicAsync(
        string topic, 
        string title, 
        string body, 
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribe device token to a topic
    /// </summary>
    Task<bool> SubscribeToTopicAsync(
        string deviceToken, 
        string topic,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribe multiple device tokens to a topic
    /// </summary>
    Task<FcmBatchResponse> SubscribeToTopicAsync(
        List<string> deviceTokens, 
        string topic,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribe device token from a topic
    /// </summary>
    Task<bool> UnsubscribeFromTopicAsync(
        string deviceToken, 
        string topic,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribe multiple device tokens from a topic
    /// </summary>
    Task<FcmBatchResponse> UnsubscribeFromTopicAsync(
        List<string> deviceTokens, 
        string topic,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send data-only message (silent notification)
    /// </summary>
    Task<bool> SendDataMessageAsync(
        string deviceToken, 
        Dictionary<string, string> data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate device token
    /// </summary>
    Task<bool> ValidateDeviceTokenAsync(
        string deviceToken,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Response for batch FCM operations
/// </summary>
public class FcmBatchResponse
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<string> FailedTokens { get; set; } = new();
    public List<string> ErrorMessages { get; set; } = new();
    
    public bool IsSuccess => FailureCount == 0;
}

