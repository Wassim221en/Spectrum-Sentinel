using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Template.Application.Interfaces;
using Template.Dashboard.Common.Interfaces;

namespace Template.Infrastructe.Services;

/// <summary>
/// Firebase Cloud Messaging (FCM) Service Implementation
/// </summary>
public class FcmService : IFcmService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FcmService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _serverKey;
    private readonly string _senderId;
    private readonly string _fcmApiUrl = "https://fcm.googleapis.com/fcm/send";

    public FcmService(
        IConfiguration configuration, 
        ILogger<FcmService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("FCM");
        
        _serverKey = _configuration["FCM:ServerKey"] ?? throw new ArgumentNullException("FCM:ServerKey");
        _senderId = _configuration["FCM:SenderId"] ?? throw new ArgumentNullException("FCM:SenderId");
        
        // Configure HttpClient
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={_serverKey}");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sender", $"id={_senderId}");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
    }

    /// <summary>
    /// Send notification to a single device
    /// </summary>
    public async Task<bool> SendNotificationAsync(
        string deviceToken, 
        string title, 
        string body, 
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                to = deviceToken,
                priority = "high",
                notification = new
                {
                    title,
                    body,
                    sound = "default",
                    badge = "1"
                },
                data = data ?? new Dictionary<string, string>()
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_fcmApiUrl, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "FCM notification sent successfully to device: {DeviceToken}. Response: {Response}",
                    MaskToken(deviceToken),
                    responseContent);
                return true;
            }

            _logger.LogError(
                "Failed to send FCM notification to device: {DeviceToken}. Status: {StatusCode}, Response: {Response}",
                MaskToken(deviceToken),
                response.StatusCode,
                responseContent);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending FCM notification to device: {DeviceToken}",
                MaskToken(deviceToken));
            return false;
        }
    }

    /// <summary>
    /// Send notification to multiple devices
    /// </summary>
    public async Task<FcmBatchResponse> SendNotificationToMultipleDevicesAsync(
        List<string> deviceTokens, 
        string title, 
        string body, 
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        var response = new FcmBatchResponse();

        if (deviceTokens == null || !deviceTokens.Any())
        {
            _logger.LogWarning("No device tokens provided for batch notification");
            return response;
        }

        try
        {
            var payload = new
            {
                registration_ids = deviceTokens,
                priority = "high",
                notification = new
                {
                    title,
                    body,
                    sound = "default",
                    badge = "1"
                },
                data = data ?? new Dictionary<string, string>()
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync(_fcmApiUrl, content, cancellationToken);
            var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
            {
                var fcmResponse = JsonSerializer.Deserialize<FcmMulticastResponse>(responseContent);
                
                if (fcmResponse != null)
                {
                    response.SuccessCount = fcmResponse.success;
                    response.FailureCount = fcmResponse.failure;

                    // Identify failed tokens
                    if (fcmResponse.results != null)
                    {
                        for (int i = 0; i < fcmResponse.results.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(fcmResponse.results[i].error))
                            {
                                response.FailedTokens.Add(deviceTokens[i]);
                                response.ErrorMessages.Add(fcmResponse.results[i].error);
                            }
                        }
                    }
                }

                _logger.LogInformation(
                    "FCM batch notification sent. Success: {SuccessCount}, Failure: {FailureCount}",
                    response.SuccessCount,
                    response.FailureCount);
            }
            else
            {
                response.FailureCount = deviceTokens.Count;
                response.FailedTokens = deviceTokens;
                
                _logger.LogError(
                    "Failed to send FCM batch notification. Status: {StatusCode}, Response: {Response}",
                    httpResponse.StatusCode,
                    responseContent);
            }
        }
        catch (Exception ex)
        {
            response.FailureCount = deviceTokens.Count;
            response.FailedTokens = deviceTokens;
            
            _logger.LogError(
                ex,
                "Error sending FCM batch notification to {Count} devices",
                deviceTokens.Count);
        }

        return response;
    }

    /// <summary>
    /// Send notification to a topic
    /// </summary>
    public async Task<bool> SendNotificationToTopicAsync(
        string topic, 
        string title, 
        string body, 
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                to = $"/topics/{topic}",
                priority = "high",
                notification = new
                {
                    title,
                    body,
                    sound = "default",
                    badge = "1"
                },
                data = data ?? new Dictionary<string, string>()
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_fcmApiUrl, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "FCM notification sent successfully to topic: {Topic}. Response: {Response}",
                    topic,
                    responseContent);
                return true;
            }

            _logger.LogError(
                "Failed to send FCM notification to topic: {Topic}. Status: {StatusCode}, Response: {Response}",
                topic,
                response.StatusCode,
                responseContent);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending FCM notification to topic: {Topic}",
                topic);
            return false;
        }
    }

    /// <summary>
    /// Subscribe device token to a topic
    /// </summary>
    public async Task<bool> SubscribeToTopicAsync(
        string deviceToken, 
        string topic,
        CancellationToken cancellationToken = default)
    {
        return await ManageTopicSubscriptionAsync(
            new List<string> { deviceToken }, 
            topic, 
            true, 
            cancellationToken);
    }

    /// <summary>
    /// Subscribe multiple device tokens to a topic
    /// </summary>
    public async Task<FcmBatchResponse> SubscribeToTopicAsync(
        List<string> deviceTokens, 
        string topic,
        CancellationToken cancellationToken = default)
    {
        return await ManageTopicSubscriptionBatchAsync(
            deviceTokens, 
            topic, 
            true, 
            cancellationToken);
    }

    /// <summary>
    /// Unsubscribe device token from a topic
    /// </summary>
    public async Task<bool> UnsubscribeFromTopicAsync(
        string deviceToken, 
        string topic,
        CancellationToken cancellationToken = default)
    {
        return await ManageTopicSubscriptionAsync(
            new List<string> { deviceToken }, 
            topic, 
            false, 
            cancellationToken);
    }

    /// <summary>
    /// Unsubscribe multiple device tokens from a topic
    /// </summary>
    public async Task<FcmBatchResponse> UnsubscribeFromTopicAsync(
        List<string> deviceTokens,
        string topic,
        CancellationToken cancellationToken = default)
    {
        return await ManageTopicSubscriptionBatchAsync(
            deviceTokens,
            topic,
            false,
            cancellationToken);
    }

    /// <summary>
    /// Send data-only message (silent notification)
    /// </summary>
    public async Task<bool> SendDataMessageAsync(
        string deviceToken,
        Dictionary<string, string> data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                to = deviceToken,
                priority = "high",
                data = data
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_fcmApiUrl, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "FCM data message sent successfully to device: {DeviceToken}",
                    MaskToken(deviceToken));
                return true;
            }

            _logger.LogError(
                "Failed to send FCM data message to device: {DeviceToken}. Status: {StatusCode}",
                MaskToken(deviceToken),
                response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending FCM data message to device: {DeviceToken}",
                MaskToken(deviceToken));
            return false;
        }
    }

    /// <summary>
    /// Validate device token by attempting to send a test message
    /// </summary>
    public async Task<bool> ValidateDeviceTokenAsync(
        string deviceToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                to = deviceToken,
                dry_run = true,
                data = new { test = "validation" }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_fcmApiUrl, content, cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error validating device token: {DeviceToken}",
                MaskToken(deviceToken));
            return false;
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Manage topic subscription for single or multiple tokens
    /// </summary>
    private async Task<bool> ManageTopicSubscriptionAsync(
        List<string> deviceTokens,
        string topic,
        bool subscribe,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var action = subscribe ? "subscribe" : "unsubscribe";
            var url = $"https://iid.googleapis.com/iid/v1:batch{action.Substring(0, 1).ToUpper()}{action.Substring(1)}";

            var payload = new
            {
                to = $"/topics/{topic}",
                registration_tokens = deviceTokens
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Successfully {Action}d {Count} device(s) to/from topic: {Topic}",
                    action,
                    deviceTokens.Count,
                    topic);
                return true;
            }

            _logger.LogError(
                "Failed to {Action} device(s) to/from topic: {Topic}. Status: {StatusCode}, Response: {Response}",
                action,
                topic,
                response.StatusCode,
                responseContent);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error managing topic subscription for topic: {Topic}",
                topic);
            return false;
        }
    }

    /// <summary>
    /// Manage topic subscription for multiple tokens with detailed response
    /// </summary>
    private async Task<FcmBatchResponse> ManageTopicSubscriptionBatchAsync(
        List<string> deviceTokens,
        string topic,
        bool subscribe,
        CancellationToken cancellationToken = default)
    {
        var response = new FcmBatchResponse();

        if (deviceTokens == null || !deviceTokens.Any())
        {
            _logger.LogWarning("No device tokens provided for topic subscription");
            return response;
        }

        try
        {
            var action = subscribe ? "subscribe" : "unsubscribe";
            var url = $"https://iid.googleapis.com/iid/v1:batch{action.Substring(0, 1).ToUpper()}{action.Substring(1)}";

            var payload = new
            {
                to = $"/topics/{topic}",
                registration_tokens = deviceTokens
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync(url, content, cancellationToken);
            var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
            {
                var topicResponse = JsonSerializer.Deserialize<FcmTopicManagementResponse>(responseContent);

                if (topicResponse?.results != null)
                {
                    for (int i = 0; i < topicResponse.results.Count; i++)
                    {
                        if (string.IsNullOrEmpty(topicResponse.results[i].error))
                        {
                            response.SuccessCount++;
                        }
                        else
                        {
                            response.FailureCount++;
                            response.FailedTokens.Add(deviceTokens[i]);
                            response.ErrorMessages.Add(topicResponse.results[i].error);
                        }
                    }
                }

                _logger.LogInformation(
                    "Topic {Action} completed. Success: {SuccessCount}, Failure: {FailureCount}",
                    action,
                    response.SuccessCount,
                    response.FailureCount);
            }
            else
            {
                response.FailureCount = deviceTokens.Count;
                response.FailedTokens = deviceTokens;

                _logger.LogError(
                    "Failed to {Action} devices to/from topic: {Topic}. Status: {StatusCode}",
                    action,
                    topic,
                    httpResponse.StatusCode);
            }
        }
        catch (Exception ex)
        {
            response.FailureCount = deviceTokens.Count;
            response.FailedTokens = deviceTokens;

            _logger.LogError(
                ex,
                "Error managing topic subscription for {Count} devices",
                deviceTokens.Count);
        }

        return response;
    }

    /// <summary>
    /// Mask device token for logging (show only first and last 4 characters)
    /// </summary>
    private string MaskToken(string token)
    {
        if (string.IsNullOrEmpty(token) || token.Length <= 8)
            return "****";

        return $"{token.Substring(0, 4)}...{token.Substring(token.Length - 4)}";
    }

    #endregion

    #region Response Models

    private class FcmMulticastResponse
    {
        public int success { get; set; }
        public int failure { get; set; }
        public List<FcmResult> results { get; set; } = new();
    }

    private class FcmResult
    {
        public string? message_id { get; set; }
        public string? error { get; set; }
    }

    private class FcmTopicManagementResponse
    {
        public List<FcmTopicResult> results { get; set; } = new();
    }

    private class FcmTopicResult
    {
        public string? error { get; set; }
    }

    #endregion
}

