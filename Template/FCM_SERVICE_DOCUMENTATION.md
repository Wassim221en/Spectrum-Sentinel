# Firebase Cloud Messaging (FCM) Service Documentation

## نظرة عامة

تم تطوير خدمة FCM متكاملة لإرسال الإشعارات Push Notifications إلى تطبيقات الموبايل (Android & iOS) باستخدام Firebase Cloud Messaging.

## المكونات الأساسية

### 1. Interface Layer (Application)

#### IFcmService Interface
موجود في: `Template.Application/Interfaces/IFcmService.cs`

يوفر الواجهة التالية:
- `SendNotificationAsync()` - إرسال إشعار لجهاز واحد
- `SendNotificationToMultipleDevicesAsync()` - إرسال إشعار لعدة أجهزة
- `SendNotificationToTopicAsync()` - إرسال إشعار لموضوع (Topic)
- `SubscribeToTopicAsync()` - الاشتراك في موضوع
- `UnsubscribeFromTopicAsync()` - إلغاء الاشتراك من موضوع
- `SendDataMessageAsync()` - إرسال رسالة بيانات فقط (Silent Notification)
- `ValidateDeviceTokenAsync()` - التحقق من صحة Device Token

### 2. Implementation Layer (Infrastructure)

#### FcmService Class
موجود في: `Template.Infrastructe/Services/FcmService.cs`

يحتوي على:
- تطبيق كامل لجميع وظائف IFcmService
- معالجة الأخطاء والـ Logging
- دعم Batch Operations
- إخفاء Device Tokens في الـ Logs للأمان

### 3. DTOs (Data Transfer Objects)

موجودة في: `Template.Application/DTOs/Notification/`

- `FcmNotificationDto` - بيانات الإشعار الأساسية
- `SendNotificationToDeviceDto` - إرسال لجهاز واحد
- `SendNotificationToMultipleDevicesDto` - إرسال لعدة أجهزة
- `SendNotificationToTopicDto` - إرسال لموضوع
- `SubscribeToTopicDto` - الاشتراك في موضوع
- `UnsubscribeFromTopicDto` - إلغاء الاشتراك
- `FcmResponseDto` - استجابة العمليات

### 4. Integration Events

موجودة في: `Template.Domain/Events/Notification/`

- `SendNotificationIntegrationEvent` - حدث إرسال إشعار لجهاز
- `SendBulkNotificationIntegrationEvent` - حدث إرسال إشعارات جماعية
- `SendTopicNotificationIntegrationEvent` - حدث إرسال لموضوع

### 5. Event Handlers

موجودة في: `Template.Application/Events/Handlers/`

- `SendNotificationIntegrationEventHandler`
- `SendBulkNotificationIntegrationEventHandler`
- `SendTopicNotificationIntegrationEventHandler`

### 6. API Controller

موجود في: `Template.API/Controllers/NotificationController.cs`

يوفر Endpoints التالية:
- `POST /api/notification/send` - إرسال إشعار مباشر
- `POST /api/notification/send-bulk` - إرسال إشعارات جماعية
- `POST /api/notification/send-to-topic` - إرسال لموضوع
- `POST /api/notification/subscribe-to-topic` - الاشتراك في موضوع
- `POST /api/notification/unsubscribe-from-topic` - إلغاء الاشتراك
- `POST /api/notification/validate-token` - التحقق من Token
- `POST /api/notification/send-async` - إرسال عبر Event Bus (Async)

## الإعدادات (Configuration)

### appsettings.json

```json
{
  "FCM": {
    "ServerKey": "YOUR_FCM_SERVER_KEY_HERE",
    "SenderId": "YOUR_FCM_SENDER_ID_HERE",
    "ProjectId": "YOUR_FIREBASE_PROJECT_ID_HERE"
  }
}
```

### الحصول على FCM Credentials

1. اذهب إلى [Firebase Console](https://console.firebase.google.com/)
2. اختر مشروعك أو أنشئ مشروع جديد
3. اذهب إلى Project Settings > Cloud Messaging
4. انسخ:
   - **Server Key** من Legacy server key
   - **Sender ID** من Sender ID

## طريقة الاستخدام

### 1. الاستخدام المباشر (Direct Usage)

```csharp
public class MyService
{
    private readonly IFcmService _fcmService;

    public MyService(IFcmService fcmService)
    {
        _fcmService = fcmService;
    }

    public async Task SendWelcomeNotification(string deviceToken, string userName)
    {
        var result = await _fcmService.SendNotificationAsync(
            deviceToken,
            "مرحباً بك!",
            $"أهلاً {userName}، نتمنى لك تجربة رائعة",
            new Dictionary<string, string>
            {
                { "type", "welcome" },
                { "userId", "123" }
            }
        );

        if (result)
        {
            // تم الإرسال بنجاح
        }
    }
}
```

### 2. الاستخدام عبر Event Bus (Async)

```csharp
public class EmployeeService
{
    private readonly IEventBus _eventBus;

    public EmployeeService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task NotifyEmployee(string deviceToken)
    {
        var notificationEvent = new SendNotificationIntegrationEvent(
            deviceToken,
            "إشعار جديد",
            "لديك مهمة جديدة",
            new Dictionary<string, string>
            {
                { "taskId", "456" }
            }
        );

        await _eventBus.PublishAsync(notificationEvent);
    }
}
```

### 3. الاستخدام عبر API

#### إرسال إشعار لجهاز واحد

```bash
curl -X POST "https://your-api.com/api/notification/send" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceToken": "device_token_here",
    "notification": {
      "title": "عنوان الإشعار",
      "body": "محتوى الإشعار",
      "data": {
        "key1": "value1",
        "key2": "value2"
      }
    }
  }'
```

#### إرسال إشعارات جماعية

```bash
curl -X POST "https://your-api.com/api/notification/send-bulk" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceTokens": [
      "token1",
      "token2",
      "token3"
    ],
    "notification": {
      "title": "إشعار جماعي",
      "body": "رسالة لجميع المستخدمين",
      "data": {
        "type": "announcement"
      }
    }
  }'
```

#### إرسال لموضوع (Topic)

```bash
curl -X POST "https://your-api.com/api/notification/send-to-topic" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "topic": "news",
    "notification": {
      "title": "خبر جديد",
      "body": "تحديث مهم في التطبيق"
    }
  }'
```

#### الاشتراك في موضوع

```bash
curl -X POST "https://your-api.com/api/notification/subscribe-to-topic" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceTokens": ["token1", "token2"],
    "topic": "news"
  }'
```

## المميزات

### 1. معالجة الأخطاء
- معالجة شاملة للأخطاء في جميع المستويات
- Logging تفصيلي للعمليات
- إخفاء Device Tokens في الـ Logs للأمان

### 2. Batch Operations
- إرسال إشعارات لعدة أجهزة دفعة واحدة
- تقرير مفصل عن النجاح والفشل
- تحديد الـ Tokens الفاشلة

### 3. Topic Management
- الاشتراك وإلغاء الاشتراك في المواضيع
- إرسال إشعارات لجميع المشتركين في موضوع

### 4. Async Processing
- معالجة غير متزامنة عبر RabbitMQ
- عدم حجب الـ Request الأساسي
- إمكانية إعادة المحاولة

### 5. Validation
- التحقق من صحة Device Tokens
- Validation للـ Input في الـ Controller

## أمثلة متقدمة

### إرسال إشعار عند إنشاء موظف جديد

```csharp
public class CreateEmployeeCommandHandler
{
    private readonly IEventBus _eventBus;

    public async Task Handle(CreateEmployeeCommand command)
    {
        // ... إنشاء الموظف

        // إرسال إشعار ترحيبي
        var notificationEvent = new SendNotificationIntegrationEvent(
            employee.DeviceToken,
            "مرحباً بك في الفريق!",
            $"أهلاً {employee.FullName}، تم إنشاء حسابك بنجاح",
            new Dictionary<string, string>
            {
                { "type", "employee_created" },
                { "employeeId", employee.Id.ToString() }
            }
        );

        await _eventBus.PublishAsync(notificationEvent);
    }
}
```

### إرسال إشعار جماعي لجميع الموظفين

```csharp
public class AnnouncementService
{
    private readonly IFcmService _fcmService;
    private readonly IRepository _repository;

    public async Task SendAnnouncementToAll(string title, string message)
    {
        // جلب جميع Device Tokens
        var deviceTokens = await _repository
            .GetAll<Employee>()
            .Where(e => !string.IsNullOrEmpty(e.DeviceToken))
            .Select(e => e.DeviceToken)
            .ToListAsync();

        // إرسال الإشعار
        var result = await _fcmService.SendNotificationToMultipleDevicesAsync(
            deviceTokens,
            title,
            message,
            new Dictionary<string, string>
            {
                { "type", "announcement" },
                { "timestamp", DateTime.UtcNow.ToString() }
            }
        );

        // معالجة النتيجة
        if (result.FailureCount > 0)
        {
            // معالجة الـ Tokens الفاشلة
            // يمكن حذفها من قاعدة البيانات أو إعادة المحاولة
        }
    }
}
```

## الأمان

1. **إخفاء Tokens في الـ Logs**: يتم إخفاء Device Tokens في الـ Logs لحماية خصوصية المستخدمين
2. **Authorization**: جميع Endpoints محمية بـ `[Authorize]` attribute
3. **Validation**: التحقق من صحة جميع المدخلات
4. **HTTPS**: يجب استخدام HTTPS في الـ Production

## الاختبار

### اختبار الإرسال المباشر

```csharp
[Fact]
public async Task SendNotification_ShouldReturnTrue_WhenSuccessful()
{
    // Arrange
    var fcmService = new FcmService(configuration, logger, httpClientFactory);
    var deviceToken = "valid_device_token";

    // Act
    var result = await fcmService.SendNotificationAsync(
        deviceToken,
        "Test Title",
        "Test Body"
    );

    // Assert
    Assert.True(result);
}
```

## الخطوات التالية

1. ✅ إضافة FCM Credentials في appsettings.json
2. ✅ اختبار الإرسال لجهاز واحد
3. ✅ اختبار الإرسال الجماعي
4. ✅ إعداد Topics حسب الحاجة
5. ✅ دمج الإشعارات مع باقي الـ Features

## الدعم والمساعدة

للمزيد من المعلومات:
- [Firebase Cloud Messaging Documentation](https://firebase.google.com/docs/cloud-messaging)
- [FCM HTTP v1 API](https://firebase.google.com/docs/reference/fcm/rest/v1/projects.messages)

---

**ملاحظة**: تأكد من تحديث FCM Server Key و Sender ID في appsettings.json قبل الاستخدام.

