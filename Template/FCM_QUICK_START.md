# FCM Service - دليل البدء السريع

## ✅ ما تم إنجازه

تم إنشاء FCM Service متكاملة تشمل:

### 1. الملفات المنشأة

#### Application Layer
- ✅ `Template.Application/Interfaces/IFcmService.cs` - Interface الخدمة
- ✅ `Template.Application/DTOs/Notification/FcmNotificationDto.cs` - DTOs
- ✅ `Template.Application/Events/Handlers/SendNotificationIntegrationEventHandler.cs`
- ✅ `Template.Application/Events/Handlers/SendBulkNotificationIntegrationEventHandler.cs`
- ✅ `Template.Application/Events/Handlers/SendTopicNotificationIntegrationEventHandler.cs`

#### Domain Layer
- ✅ `Template.Domain/Events/Notification/SendNotificationIntegrationEvent.cs`
- ✅ `Template.Domain/Events/Notification/SendBulkNotificationIntegrationEvent.cs`
- ✅ `Template.Domain/Events/Notification/SendTopicNotificationIntegrationEvent.cs`

#### Infrastructure Layer
- ✅ `Template.Infrastructe/Services/FcmService.cs` - التطبيق الكامل

#### API Layer
- ✅ `Template.API/Controllers/NotificationController.cs` - REST API Endpoints

#### Documentation
- ✅ `FCM_SERVICE_DOCUMENTATION.md` - التوثيق الشامل
- ✅ `FCM_USAGE_EXAMPLES.md` - أمثلة عملية
- ✅ `FCM_QUICK_START.md` - هذا الملف

### 2. التسجيل في DI Container
- ✅ تم تسجيل `IFcmService` و `FcmService`
- ✅ تم تسجيل `HttpClient` للـ FCM
- ✅ تم تسجيل جميع Event Handlers

### 3. الإعدادات
- ✅ تم إضافة FCM Configuration في `appsettings.json`

## 🚀 خطوات البدء

### الخطوة 1: إضافة FCM Credentials

افتح `Template.API/appsettings.json` وحدّث القيم التالية:

```json
{
  "FCM": {
    "ServerKey": "YOUR_ACTUAL_FCM_SERVER_KEY",
    "SenderId": "YOUR_ACTUAL_SENDER_ID",
    "ProjectId": "YOUR_FIREBASE_PROJECT_ID"
  }
}
```

#### كيفية الحصول على Credentials:

1. اذهب إلى [Firebase Console](https://console.firebase.google.com/)
2. اختر مشروعك أو أنشئ مشروع جديد
3. اذهب إلى **Project Settings** (⚙️) > **Cloud Messaging**
4. انسخ:
   - **Server Key** من "Cloud Messaging API (Legacy)"
   - **Sender ID** من "Cloud Messaging"
   - **Project ID** من "General" tab

### الخطوة 2: تشغيل المشروع

```bash
cd Template/Template.API
dotnet run
```

### الخطوة 3: اختبار الـ API

#### اختبار إرسال إشعار لجهاز واحد

```bash
curl -X POST "https://localhost:5001/api/notification/send" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceToken": "YOUR_DEVICE_TOKEN",
    "notification": {
      "title": "اختبار الإشعار",
      "body": "هذا إشعار تجريبي من FCM Service",
      "data": {
        "test": "true"
      }
    }
  }'
```

#### الحصول على Device Token

من تطبيق Android/iOS، استخدم Firebase SDK:

**Android (Kotlin):**
```kotlin
FirebaseMessaging.getInstance().token.addOnCompleteListener { task ->
    if (task.isSuccessful) {
        val token = task.result
        // أرسل هذا Token إلى الـ API
    }
}
```

**iOS (Swift):**
```swift
Messaging.messaging().token { token, error in
    if let token = token {
        // أرسل هذا Token إلى الـ API
    }
}
```

## 📋 API Endpoints المتاحة

| Method | Endpoint | الوصف |
|--------|----------|-------|
| POST | `/api/notification/send` | إرسال إشعار لجهاز واحد |
| POST | `/api/notification/send-bulk` | إرسال إشعارات جماعية |
| POST | `/api/notification/send-to-topic` | إرسال لموضوع |
| POST | `/api/notification/subscribe-to-topic` | الاشتراك في موضوع |
| POST | `/api/notification/unsubscribe-from-topic` | إلغاء الاشتراك |
| POST | `/api/notification/validate-token` | التحقق من Token |
| POST | `/api/notification/send-async` | إرسال عبر Event Bus |

## 💡 أمثلة سريعة

### 1. الاستخدام المباشر في الكود

```csharp
public class MyService
{
    private readonly IFcmService _fcmService;

    public MyService(IFcmService fcmService)
    {
        _fcmService = fcmService;
    }

    public async Task SendNotification()
    {
        await _fcmService.SendNotificationAsync(
            "device_token_here",
            "عنوان الإشعار",
            "محتوى الإشعار",
            new Dictionary<string, string>
            {
                { "key", "value" }
            }
        );
    }
}
```

### 2. الاستخدام عبر Event Bus

```csharp
public class MyService
{
    private readonly IEventBus _eventBus;

    public MyService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task QueueNotification()
    {
        var notificationEvent = new SendNotificationIntegrationEvent(
            "device_token_here",
            "عنوان الإشعار",
            "محتوى الإشعار"
        );

        await _eventBus.PublishAsync(notificationEvent);
    }
}
```

### 3. إرسال لعدة أجهزة

```csharp
var tokens = new List<string> { "token1", "token2", "token3" };

var result = await _fcmService.SendNotificationToMultipleDevicesAsync(
    tokens,
    "إشعار جماعي",
    "رسالة لجميع المستخدمين"
);

Console.WriteLine($"نجح: {result.SuccessCount}, فشل: {result.FailureCount}");
```

### 4. استخدام Topics

```csharp
// الاشتراك في موضوع
await _fcmService.SubscribeToTopicAsync(deviceTokens, "news");

// إرسال لموضوع
await _fcmService.SendNotificationToTopicAsync(
    "news",
    "خبر جديد",
    "تحديث مهم في التطبيق"
);
```

## 🔧 التكامل مع النظام الحالي

### إضافة Device Token للموظف

1. أضف خاصية `DeviceToken` في `Employee` Entity:

```csharp
public string? DeviceToken { get; private set; }

public void UpdateDeviceToken(string? deviceToken)
{
    DeviceToken = deviceToken;
}
```

2. أنشئ Migration:

```bash
dotnet ef migrations add AddDeviceTokenToEmployee -p Template.Persistence -s Template.API
dotnet ef database update -p Template.Persistence -s Template.API
```

3. أضف Endpoint لتحديث Device Token:

```csharp
[HttpPut("device-token")]
[Authorize]
public async Task<IActionResult> UpdateDeviceToken([FromBody] string deviceToken)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var employee = await _userManager.FindByIdAsync(userId);
    
    employee.UpdateDeviceToken(deviceToken);
    await _userManager.UpdateAsync(employee);
    
    return Ok();
}
```

## 🎯 حالات الاستخدام الشائعة

### 1. إشعار ترحيبي عند التسجيل
```csharp
// في EmployeeCreatedIntegrationEventHandler
var notificationEvent = new SendNotificationIntegrationEvent(
    employee.DeviceToken,
    "مرحباً بك! 🎉",
    $"أهلاً {employee.FullName}، تم إنشاء حسابك بنجاح"
);
await _eventBus.PublishAsync(notificationEvent);
```

### 2. إشعار عند تغيير كلمة المرور
```csharp
// في PasswordResetSuccessIntegrationEventHandler
var notificationEvent = new SendNotificationIntegrationEvent(
    employee.DeviceToken,
    "تم تغيير كلمة المرور 🔐",
    "تم إعادة تعيين كلمة المرور بنجاح"
);
await _eventBus.PublishAsync(notificationEvent);
```

### 3. إعلان لجميع الموظفين
```csharp
var deviceTokens = await _context.Employees
    .Where(e => !string.IsNullOrEmpty(e.DeviceToken))
    .Select(e => e.DeviceToken)
    .ToListAsync();

await _fcmService.SendNotificationToMultipleDevicesAsync(
    deviceTokens,
    "إعلان مهم",
    "اجتماع عام غداً الساعة 10 صباحاً"
);
```

## 🐛 استكشاف الأخطاء

### خطأ: "Unauthorized"
- تأكد من صحة FCM Server Key في appsettings.json
- تأكد من تفعيل Cloud Messaging API في Firebase Console

### خطأ: "Invalid registration token"
- Device Token غير صحيح أو منتهي الصلاحية
- اطلب من التطبيق إرسال Token جديد

### لا يصل الإشعار
- تأكد من تثبيت Firebase SDK في التطبيق
- تأكد من أن التطبيق لديه أذونات الإشعارات
- تحقق من الـ Logs في Firebase Console

## 📚 المزيد من المعلومات

- راجع `FCM_SERVICE_DOCUMENTATION.md` للتوثيق الشامل
- راجع `FCM_USAGE_EXAMPLES.md` لأمثلة متقدمة
- [Firebase Cloud Messaging Documentation](https://firebase.google.com/docs/cloud-messaging)

## ✨ المميزات

- ✅ إرسال لجهاز واحد أو عدة أجهزة
- ✅ دعم Topics للإشعارات المجمعة
- ✅ معالجة الأخطاء والـ Logging
- ✅ Async Processing عبر RabbitMQ
- ✅ Batch Operations مع تقارير مفصلة
- ✅ REST API جاهز للاستخدام
- ✅ Integration Events للتكامل السلس
- ✅ أمان وإخفاء Tokens في الـ Logs

## 🎉 جاهز للاستخدام!

الآن لديك FCM Service متكاملة وجاهزة للاستخدام. فقط أضف FCM Credentials وابدأ بإرسال الإشعارات!

---

**تم إنشاء هذه الخدمة بواسطة Senior Backend Developer** 🚀

