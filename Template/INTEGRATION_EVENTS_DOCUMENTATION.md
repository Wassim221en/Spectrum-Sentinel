# Integration Events with RabbitMQ - Documentation

## نظرة عامة (Overview)

تم تطبيق نظام Integration Events مع RabbitMQ في المشروع لتمكين التواصل بين الخدمات المختلفة (Microservices Communication) بشكل غير متزامن (Asynchronous).

## البنية المعمارية (Architecture)

### 1. Domain Layer
يحتوي على:
- **IntegrationEvent**: الـ Base Class لجميع Integration Events
- **Integration Events**: الأحداث المختلفة مثل:
  - `EmployeeCreatedIntegrationEvent`
  - `EmployeeUpdatedIntegrationEvent`
  - `EmployeeDeletedIntegrationEvent`
  - `LabCreatedIntegrationEvent`

### 2. Application Layer
يحتوي على:
- **IEventBus**: Interface للـ Event Bus
- **IIntegrationEventHandler<TEvent>**: Interface للـ Event Handlers
- **Event Handlers**: معالجات الأحداث مثل:
  - `EmployeeCreatedIntegrationEventHandler`
  - `EmployeeDeletedIntegrationEventHandler`
  - `LabCreatedIntegrationEventHandler`

### 3. Infrastructure Layer
يحتوي على:
- **RabbitMQEventBus**: تطبيق IEventBus باستخدام RabbitMQ
- **RabbitMQSettings**: إعدادات الاتصال بـ RabbitMQ

## التكوين (Configuration)

### 1. appsettings.json

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "ExchangeName": "template_integration_events",
    "ExchangeType": "topic",
    "QueueName": "template_api_queue",
    "RetryCount": 3,
    "ConnectionTimeout": 30,
    "AutomaticRecoveryEnabled": true,
    "NetworkRecoveryInterval": 5
  }
}
```

### 2. Program.cs

```csharp
// Configure RabbitMQ Settings
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection(RabbitMQSettings.SectionName));

// Register Event Bus
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

// Register Integration Event Handlers
builder.Services.AddScoped<EmployeeCreatedIntegrationEventHandler>();
builder.Services.AddScoped<EmployeeDeletedIntegrationEventHandler>();
builder.Services.AddScoped<LabCreatedIntegrationEventHandler>();

// Configure Event Bus Subscriptions
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<EmployeeCreatedIntegrationEvent, EmployeeCreatedIntegrationEventHandler>();
eventBus.Subscribe<EmployeeDeletedIntegrationEvent, EmployeeDeletedIntegrationEventHandler>();
eventBus.Subscribe<LabCreatedIntegrationEvent, LabCreatedIntegrationEventHandler>();

// Start consuming events (optional)
// eventBus.StartConsuming();
```

## كيفية الاستخدام (Usage)

### 1. نشر حدث (Publishing an Event)

```csharp
public class AddEmployeeHandler : IRequestHandler<AddEmployeeCommand.Request, OperationResponse<EmployeeRes>>
{
    private readonly IEventBus _eventBus;

    public AddEmployeeHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<OperationResponse<EmployeeRes>> Handle(
        AddEmployeeCommand.Request request, 
        CancellationToken cancellationToken)
    {
        // Create employee logic...
        
        // Publish integration event
        var integrationEvent = new EmployeeCreatedIntegrationEvent(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.PhoneNumber);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
        
        return response;
    }
}
```

### 2. إنشاء Integration Event جديد

```csharp
namespace Template.Domain.Events.YourModule;

public class YourEntityCreatedIntegrationEvent : IntegrationEvent
{
    public Guid EntityId { get; private set; }
    public string Name { get; private set; }

    public YourEntityCreatedIntegrationEvent(Guid entityId, string name)
    {
        EntityId = entityId;
        Name = name;
    }
}
```

### 3. إنشاء Event Handler

```csharp
namespace Template.Dashboard.Events.Handlers;

public class YourEntityCreatedIntegrationEventHandler 
    : IIntegrationEventHandler<YourEntityCreatedIntegrationEvent>
{
    private readonly ILogger<YourEntityCreatedIntegrationEventHandler> _logger;

    public YourEntityCreatedIntegrationEventHandler(
        ILogger<YourEntityCreatedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(
        YourEntityCreatedIntegrationEvent @event, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling YourEntityCreatedIntegrationEvent for entity {EntityId}",
            @event.EntityId);

        // Your business logic here
        
        await Task.CompletedTask;
    }
}
```

### 4. تسجيل Handler جديد

في `Program.cs`:

```csharp
// Register handler
builder.Services.AddScoped<YourEntityCreatedIntegrationEventHandler>();

// Subscribe to event
eventBus.Subscribe<YourEntityCreatedIntegrationEvent, YourEntityCreatedIntegrationEventHandler>();
```

## تشغيل RabbitMQ (Running RabbitMQ)

### باستخدام Docker

```bash
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=guest \
  -e RABBITMQ_DEFAULT_PASS=guest \
  rabbitmq:3-management
```

### الوصول إلى Management UI

- URL: http://localhost:15672
- Username: guest
- Password: guest

## الميزات (Features)

### 1. Automatic Recovery
- يتم إعادة الاتصال تلقائياً في حالة انقطاع الاتصال
- يمكن تكوين فترة إعادة المحاولة عبر `NetworkRecoveryInterval`

### 2. Message Persistence
- الرسائل يتم حفظها على القرص (Durable)
- لا تفقد الرسائل في حالة إعادة تشغيل RabbitMQ

### 3. Topic Exchange
- يستخدم Topic Exchange للتوجيه المرن
- Routing Key = Event Type Name

### 4. Error Handling
- في حالة فشل معالجة الرسالة، يتم إعادتها إلى الـ Queue
- يمكن تكوين عدد المحاولات عبر `RetryCount`

## Best Practices

### 1. Event Design
- اجعل الـ Events immutable (استخدم `private set`)
- ضع فقط البيانات الضرورية في الـ Event
- لا تضع كائنات معقدة أو Domain Entities

### 2. Handler Design
- اجعل الـ Handlers idempotent (يمكن تنفيذها عدة مرات بنفس النتيجة)
- لا ترمي Exceptions إلا في حالات الأخطاء الحرجة
- استخدم Logging بشكل مناسب

### 3. Performance
- استخدم Scoped lifetime للـ Handlers
- استخدم Singleton lifetime للـ Event Bus
- تجنب العمليات الثقيلة في الـ Handlers

### 4. Testing
- اختبر الـ Handlers بشكل منفصل
- استخدم Mock للـ IEventBus في Unit Tests
- اختبر سيناريوهات الفشل والإعادة

## Monitoring

### 1. Logging
جميع العمليات يتم تسجيلها:
- نشر الأحداث
- استقبال الأحداث
- معالجة الأحداث
- الأخطاء

### 2. RabbitMQ Management UI
يمكنك مراقبة:
- عدد الرسائل في الـ Queue
- معدل النشر والاستهلاك
- الاتصالات النشطة
- الأخطاء

## Troubleshooting

### 1. لا يتم نشر الأحداث
- تأكد من تشغيل RabbitMQ
- تحقق من إعدادات الاتصال في appsettings.json
- راجع الـ Logs للأخطاء

### 2. لا يتم استقبال الأحداث
- تأكد من استدعاء `eventBus.StartConsuming()`
- تحقق من تسجيل الـ Subscriptions
- تأكد من تسجيل الـ Handlers في DI Container

### 3. الرسائل تتراكم في الـ Queue
- تحقق من أداء الـ Handlers
- راجع الـ Logs للأخطاء في المعالجة
- تأكد من أن الـ Handlers لا ترمي Exceptions

## أمثلة إضافية (Additional Examples)

### مثال: إرسال إشعار عند حذف موظف

```csharp
public class EmployeeDeletedIntegrationEventHandler 
    : IIntegrationEventHandler<EmployeeDeletedIntegrationEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmployeeDeletedIntegrationEventHandler> _logger;

    public async Task HandleAsync(
        EmployeeDeletedIntegrationEvent @event, 
        CancellationToken cancellationToken = default)
    {
        // Send notification to admin
        await _emailService.SendEmailAsync(
            "admin@example.com",
            "Employee Deleted",
            $"Employee {@event.Email} has been deleted");
            
        _logger.LogInformation(
            "Sent deletion notification for employee {Email}",
            @event.Email);
    }
}
```

## الخلاصة (Summary)

تم تطبيق نظام Integration Events احترافي يدعم:
- ✅ نشر واستقبال الأحداث بشكل غير متزامن
- ✅ التواصل بين الخدمات المختلفة
- ✅ معالجة الأخطاء وإعادة المحاولة
- ✅ Logging شامل
- ✅ قابلية التوسع والصيانة
- ✅ Best Practices في التصميم

