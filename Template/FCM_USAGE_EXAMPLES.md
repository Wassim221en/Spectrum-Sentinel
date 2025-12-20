# أمثلة عملية لاستخدام FCM Service

## مثال 1: إرسال إشعار ترحيبي عند إنشاء موظف جديد

### تعديل EmployeeCreatedIntegrationEventHandler

```csharp
using Microsoft.Extensions.Logging;
using Template.Application.Interfaces;
using Template.Dashboard.Events;
using Template.Domain.Events.Employee;
using Template.Domain.Events.Notification;

namespace Template.Dashboard.Events.Handlers;

public class EmployeeCreatedIntegrationEventHandler : IIntegrationEventHandler<EmployeeCreatedIntegrationEvent>
{
    private readonly ILogger<EmployeeCreatedIntegrationEventHandler> _logger;
    private readonly IEmailService _emailService;
    private readonly IEventBus _eventBus;

    public EmployeeCreatedIntegrationEventHandler(
        ILogger<EmployeeCreatedIntegrationEventHandler> logger,
        IEmailService emailService,
        IEventBus eventBus)
    {
        _logger = logger;
        _emailService = emailService;
        _eventBus = eventBus;
    }

    public async Task HandleAsync(EmployeeCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling EmployeeCreatedIntegrationEvent for employee {EmployeeId} - {Email}",
            @event.EmployeeId,
            @event.Email);

        try
        {
            // إرسال البريد الإلكتروني
            var employeeName = $"{@event.FirstName} {@event.LastName}";
            await _emailService.SendWelcomeEmailAsync(@event.Email, employeeName);

            // إرسال إشعار FCM إذا كان لديه Device Token
            if (!string.IsNullOrEmpty(@event.DeviceToken))
            {
                var notificationEvent = new SendNotificationIntegrationEvent(
                    @event.DeviceToken,
                    "مرحباً بك في الفريق! 🎉",
                    $"أهلاً {employeeName}، تم إنشاء حسابك بنجاح. نتمنى لك تجربة رائعة!",
                    new Dictionary<string, string>
                    {
                        { "type", "employee_welcome" },
                        { "employeeId", @event.EmployeeId.ToString() },
                        { "timestamp", DateTime.UtcNow.ToString("o") }
                    },
                    @event.EmployeeId.ToString()
                );

                await _eventBus.PublishAsync(notificationEvent, cancellationToken);
                
                _logger.LogInformation(
                    "Welcome notification queued for employee {EmployeeId}",
                    @event.EmployeeId);
            }

            _logger.LogInformation(
                "Successfully processed EmployeeCreatedIntegrationEvent for {Email}",
                @event.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing EmployeeCreatedIntegrationEvent for {Email}",
                @event.Email);
        }
    }
}
```

## مثال 2: إرسال إشعار عند نجاح إعادة تعيين كلمة المرور

### تعديل PasswordResetSuccessIntegrationEventHandler

```csharp
using Microsoft.Extensions.Logging;
using Template.Application.Interfaces;
using Template.Dashboard.Events;
using Template.Domain.Events.Employee;
using Template.Domain.Events.Notification;

namespace Template.Application.Events.Handlers;

public class PasswordResetSuccessIntegrationEventHandler : IIntegrationEventHandler<PasswordResetSuccessIntegrationEvent>
{
    private readonly IEmailService _emailService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<PasswordResetSuccessIntegrationEventHandler> _logger;

    public PasswordResetSuccessIntegrationEventHandler(
        IEmailService emailService,
        IEventBus eventBus,
        ILogger<PasswordResetSuccessIntegrationEventHandler> logger)
    {
        _emailService = emailService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task HandleAsync(PasswordResetSuccessIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling PasswordResetSuccessIntegrationEvent for employee: {Email}",
            @event.Email);

        try
        {
            // إرسال البريد الإلكتروني
            await _emailService.SendEmailAsync(
                @event.Email,
                "تم إعادة تعيين كلمة المرور بنجاح",
                $"مرحباً {@event.FullName},\n\n" +
                $"تم إعادة تعيين كلمة المرور الخاصة بك بنجاح.\n\n" +
                $"إذا لم تقم بهذا الإجراء، يرجى التواصل مع الدعم فوراً.\n\n" +
                $"تحياتنا,\nفريق Template");

            // إرسال إشعار FCM
            if (!string.IsNullOrEmpty(@event.DeviceToken))
            {
                var notificationEvent = new SendNotificationIntegrationEvent(
                    @event.DeviceToken,
                    "تم تغيير كلمة المرور 🔐",
                    "تم إعادة تعيين كلمة المرور الخاصة بك بنجاح. إذا لم تقم بهذا الإجراء، يرجى التواصل معنا فوراً.",
                    new Dictionary<string, string>
                    {
                        { "type", "password_reset_success" },
                        { "email", @event.Email },
                        { "timestamp", DateTime.UtcNow.ToString("o") },
                        { "action_required", "false" }
                    }
                );

                await _eventBus.PublishAsync(notificationEvent, cancellationToken);
            }

            _logger.LogInformation(
                "Password reset confirmation sent successfully to: {Email}",
                @event.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling PasswordResetSuccessIntegrationEvent for: {Email}",
                @event.Email);
        }
    }
}
```

## مثال 3: إرسال إشعارات جماعية لجميع الموظفين

### إنشاء Announcement Service

```csharp
using Template.Application.Interfaces;
using Template.Domain.Interfaces;
using Template.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Template.Application.Services;

public interface IAnnouncementService
{
    Task<AnnouncementResult> SendAnnouncementToAllEmployeesAsync(
        string title, 
        string message, 
        Dictionary<string, string>? data = null);
    
    Task<AnnouncementResult> SendAnnouncementToTopicAsync(
        string topic, 
        string title, 
        string message, 
        Dictionary<string, string>? data = null);
}

public class AnnouncementService : IAnnouncementService
{
    private readonly IFcmService _fcmService;
    private readonly IRepository _repository;
    private readonly ILogger<AnnouncementService> _logger;

    public AnnouncementService(
        IFcmService fcmService,
        IRepository repository,
        ILogger<AnnouncementService> logger)
    {
        _fcmService = fcmService;
        _repository = repository;
        _logger = logger;
    }

    public async Task<AnnouncementResult> SendAnnouncementToAllEmployeesAsync(
        string title, 
        string message, 
        Dictionary<string, string>? data = null)
    {
        _logger.LogInformation("Sending announcement to all employees: {Title}", title);

        try
        {
            // جلب جميع Device Tokens للموظفين النشطين
            var deviceTokens = await _repository
                .GetAll<Employee>()
                .Where(e => !string.IsNullOrEmpty(e.DeviceToken) && !e.DateDeleted.HasValue)
                .Select(e => e.DeviceToken!)
                .ToListAsync();

            if (!deviceTokens.Any())
            {
                _logger.LogWarning("No device tokens found for employees");
                return new AnnouncementResult
                {
                    Success = false,
                    Message = "No employees with device tokens found"
                };
            }

            // إضافة بيانات إضافية
            var notificationData = data ?? new Dictionary<string, string>();
            notificationData["type"] = "announcement";
            notificationData["timestamp"] = DateTime.UtcNow.ToString("o");

            // إرسال الإشعارات
            var result = await _fcmService.SendNotificationToMultipleDevicesAsync(
                deviceTokens,
                title,
                message,
                notificationData
            );

            _logger.LogInformation(
                "Announcement sent. Success: {SuccessCount}, Failed: {FailureCount}",
                result.SuccessCount,
                result.FailureCount);

            return new AnnouncementResult
            {
                Success = result.IsSuccess,
                Message = $"Sent to {result.SuccessCount} employees, failed for {result.FailureCount}",
                SuccessCount = result.SuccessCount,
                FailureCount = result.FailureCount,
                FailedTokens = result.FailedTokens
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending announcement to all employees");
            return new AnnouncementResult
            {
                Success = false,
                Message = "Error sending announcement"
            };
        }
    }

    public async Task<AnnouncementResult> SendAnnouncementToTopicAsync(
        string topic, 
        string title, 
        string message, 
        Dictionary<string, string>? data = null)
    {
        _logger.LogInformation("Sending announcement to topic: {Topic}", topic);

        try
        {
            var notificationData = data ?? new Dictionary<string, string>();
            notificationData["type"] = "topic_announcement";
            notificationData["topic"] = topic;
            notificationData["timestamp"] = DateTime.UtcNow.ToString("o");

            var result = await _fcmService.SendNotificationToTopicAsync(
                topic,
                title,
                message,
                notificationData
            );

            return new AnnouncementResult
            {
                Success = result,
                Message = result ? "Announcement sent to topic successfully" : "Failed to send announcement to topic"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending announcement to topic: {Topic}", topic);
            return new AnnouncementResult
            {
                Success = false,
                Message = "Error sending announcement to topic"
            };
        }
    }
}

public class AnnouncementResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<string> FailedTokens { get; set; } = new();
}
```

## مثال 4: إضافة Device Token للموظف

### تعديل Employee Entity

```csharp
// إضافة خاصية DeviceToken في Employee Entity
public class Employee : User
{
    // ... الخصائص الموجودة
    
    public string? DeviceToken { get; private set; }
    
    public void UpdateDeviceToken(string? deviceToken)
    {
        DeviceToken = deviceToken;
    }
}
```

### إنشاء Command لتحديث Device Token

```csharp
using MediatR;
using Template.Application.Core;

namespace Template.Application.Employee.Commands.UpdateDeviceToken;

public class UpdateDeviceTokenCommand
{
    public class Request : IRequest<OperationResponse>
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string DeviceToken { get; set; } = string.Empty;
    }
}
```

### Handler للـ Command

```csharp
using MediatR;
using Microsoft.AspNetCore.Identity;
using Template.Application.Core;
using Template.Domain.Entities;

namespace Template.Application.Employee.Commands.UpdateDeviceToken;

public class UpdateDeviceTokenCommandHandler : IRequestHandler<UpdateDeviceTokenCommand.Request, OperationResponse>
{
    private readonly UserManager<Domain.Entities.Employee> _userManager;
    private readonly ILogger<UpdateDeviceTokenCommandHandler> _logger;

    public UpdateDeviceTokenCommandHandler(
        UserManager<Domain.Entities.Employee> userManager,
        ILogger<UpdateDeviceTokenCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<OperationResponse> Handle(
        UpdateDeviceTokenCommand.Request request, 
        CancellationToken cancellationToken)
    {
        var employee = await _userManager.FindByIdAsync(request.EmployeeId);
        
        if (employee == null)
            return new HttpMessage("Employee not found", HttpStatusCode.NotFound);

        employee.UpdateDeviceToken(request.DeviceToken);
        var result = await _userManager.UpdateAsync(employee);

        if (!result.Succeeded)
            return new HttpMessage("Failed to update device token", HttpStatusCode.BadRequest);

        _logger.LogInformation(
            "Device token updated for employee: {EmployeeId}",
            request.EmployeeId);

        return OperationResponse.Ok();
    }
}
```

### إضافة Endpoint في Controller

```csharp
[HttpPut("device-token")]
[Authorize]
public async Task<IActionResult> UpdateDeviceToken([FromBody] UpdateDeviceTokenDto dto)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if (string.IsNullOrEmpty(userId))
        return Unauthorized();

    var command = new UpdateDeviceTokenCommand.Request
    {
        EmployeeId = userId,
        DeviceToken = dto.DeviceToken
    };

    var result = await _mediator.Send(command);
    return result.ToActionResult();
}
```

## مثال 5: استخدام Topics للإشعارات المجمعة

### إنشاء Topic Management Service

```csharp
public interface ITopicManagementService
{
    Task<bool> SubscribeEmployeeToTopicAsync(string employeeId, string topic);
    Task<bool> UnsubscribeEmployeeFromTopicAsync(string employeeId, string topic);
    Task<bool> SubscribeAllEmployeesToTopicAsync(string topic);
}

public class TopicManagementService : ITopicManagementService
{
    private readonly IFcmService _fcmService;
    private readonly IRepository _repository;
    private readonly ILogger<TopicManagementService> _logger;

    public TopicManagementService(
        IFcmService fcmService,
        IRepository repository,
        ILogger<TopicManagementService> logger)
    {
        _fcmService = fcmService;
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> SubscribeEmployeeToTopicAsync(string employeeId, string topic)
    {
        var employee = await _repository.GetByIdAsync<Employee>(Guid.Parse(employeeId));
        
        if (employee == null || string.IsNullOrEmpty(employee.DeviceToken))
            return false;

        return await _fcmService.SubscribeToTopicAsync(employee.DeviceToken, topic);
    }

    public async Task<bool> UnsubscribeEmployeeFromTopicAsync(string employeeId, string topic)
    {
        var employee = await _repository.GetByIdAsync<Employee>(Guid.Parse(employeeId));
        
        if (employee == null || string.IsNullOrEmpty(employee.DeviceToken))
            return false;

        return await _fcmService.UnsubscribeFromTopicAsync(employee.DeviceToken, topic);
    }

    public async Task<bool> SubscribeAllEmployeesToTopicAsync(string topic)
    {
        var deviceTokens = await _repository
            .GetAll<Employee>()
            .Where(e => !string.IsNullOrEmpty(e.DeviceToken) && !e.DateDeleted.HasValue)
            .Select(e => e.DeviceToken!)
            .ToListAsync();

        if (!deviceTokens.Any())
            return false;

        var result = await _fcmService.SubscribeToTopicAsync(deviceTokens, topic);
        return result.IsSuccess;
    }
}
```

## Topics المقترحة

- `all_employees` - جميع الموظفين
- `managers` - المدراء فقط
- `department_{id}` - موظفو قسم معين
- `lab_{id}` - موظفو مختبر معين
- `urgent` - الإشعارات العاجلة
- `announcements` - الإعلانات العامة

## ملاحظات مهمة

1. **تحديث Device Token**: يجب على التطبيق إرسال Device Token عند تسجيل الدخول أو عند تحديثه
2. **معالجة Tokens الفاشلة**: يجب حذف أو تحديث Tokens التي فشل الإرسال إليها
3. **الأمان**: تأكد من أن الموظف يمكنه فقط تحديث Device Token الخاص به
4. **الأداء**: استخدم Topics للإشعارات الجماعية بدلاً من إرسال لكل جهاز على حدة
5. **Testing**: اختبر الإشعارات على أجهزة حقيقية قبل الإطلاق

---

**تم إنشاء هذه الأمثلة لتسهيل دمج FCM Service مع نظامك الحالي.**

