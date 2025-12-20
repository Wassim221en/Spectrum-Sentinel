# CQRS & MediatR Implementation Documentation

## نظرة عامة

تم إعادة هيكلة المشروع لاستخدام **CQRS Pattern** (Command Query Responsibility Segregation) مع **MediatR** لفصل العمليات وتحسين قابلية الصيانة والاختبار.

---

## البنية المعمارية

### 1. **Commands** (الأوامر)
الأوامر هي العمليات التي تُغير حالة النظام (Create, Update, Delete).

**الموقع:** `Template.Application/Features/Auth/Commands/`

#### Commands المُنفذة:
- **LoginCommand**: تسجيل الدخول
- **ForgetPasswordCommand**: طلب إعادة تعيين كلمة المرور
- **ResetPasswordCommand**: إعادة تعيين كلمة المرور
- **RefreshTokenCommand**: تجديد الـ access token

### 2. **Handlers** (معالجات الأوامر)
كل Command له Handler خاص به يحتوي على منطق العمل.

**الموقع:** `Template.Application/Features/Auth/Commands/{CommandName}/`

---

## هيكل الملفات

```
Template.Application/
├── Features/
│   └── Auth/
│       ├── Commands/
│       │   ├── Login/
│       │   │   ├── LoginCommand.cs
│       │   │   └── LoginCommandHandler.cs
│       │   ├── ForgetPassword/
│       │   │   ├── ForgetPasswordCommand.cs
│       │   │   └── ForgetPasswordCommandHandler.cs
│       │   ├── ResetPassword/
│       │   │   ├── ResetPasswordCommand.cs
│       │   │   └── ResetPasswordCommandHandler.cs
│       │   └── RefreshToken/
│       │       ├── RefreshTokenCommand.cs
│       │       └── RefreshTokenCommandHandler.cs
│       └── Queries/
│           └── GetCurrentUser/
│               └── (للتطوير المستقبلي)
├── DTOs/
│   └── Auth/
│       ├── LoginDto.cs
│       ├── ForgetPasswordDto.cs
│       ├── ResetPasswordDto.cs
│       ├── RefreshTokenDto.cs
│       ├── AuthResponseDto.cs
│       └── LoginResponseDto.cs
└── Interfaces/
    ├── IAuthService.cs
    └── IEmailService.cs
```

---

## أمثلة على الكود

### 1. Command Example

<augment_code_snippet path="Template/Template.Application/Features/Auth/Commands/Login/LoginCommand.cs" mode="EXCERPT">
```csharp
public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;
```
</augment_code_snippet>

### 2. Handler Example

<augment_code_snippet path="Template/Template.Application/Features/Auth/Commands/Login/LoginCommandHandler.cs" mode="EXCERPT">
```csharp
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var loginDto = new LoginDto
        {
            Email = request.Email,
            Password = request.Password
        };

        return await _authService.LoginAsync(loginDto, cancellationToken);
    }
}
```
</augment_code_snippet>

### 3. Controller Usage

<augment_code_snippet path="Template/Template.API/Controllers/AuthController.cs" mode="EXCERPT">
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
{
    var command = new LoginCommand(loginDto.Email, loginDto.Password);
    var result = await _mediator.Send(command, cancellationToken);

    if (!result.Success)
    {
        return BadRequest(result);
    }

    return Ok(result);
}
```
</augment_code_snippet>

---

## Email Service Integration

تم إضافة خدمة إرسال البريد الإلكتروني باستخدام **MailKit**.

### IEmailService Interface

<augment_code_snippet path="Template/Template.Application/Interfaces/IEmailService.cs" mode="EXCERPT">
```csharp
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendPasswordResetEmailAsync(string to, string resetToken, string employeeName);
    Task SendWelcomeEmailAsync(string to, string employeeName);
}
```
</augment_code_snippet>

### Email Configuration

في `appsettings.json`:

```json
{
  "Email": {
    "From": "noreply@templateapi.com",
    "SmtpServer": "smtp.gmail.com",
    "Port": "587",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

**ملاحظة:** لاستخدام Gmail:
1. قم بتفعيل 2-Factor Authentication
2. أنشئ App Password من [Google Account Settings](https://myaccount.google.com/apppasswords)
3. استخدم App Password في الـ configuration

---

## MediatR Registration

في `Program.cs`:

```csharp
// Register MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(IAuthService).Assembly);
});

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
```

---

## فوائد استخدام CQRS & MediatR

### 1. **Separation of Concerns**
- فصل منطق العمل عن Controllers
- كل Command/Query له Handler مستقل

### 2. **Testability**
- سهولة اختبار Handlers بشكل منفصل
- Mock dependencies بسهولة

### 3. **Maintainability**
- كود منظم وسهل القراءة
- سهولة إضافة features جديدة

### 4. **Scalability**
- يمكن إضافة Behaviors (Logging, Validation, Caching)
- يمكن تطبيق Pipeline Behaviors

### 5. **Single Responsibility**
- كل Handler مسؤول عن عملية واحدة فقط

---

## Pipeline Behaviors (للتطوير المستقبلي)

يمكن إضافة Behaviors لتطبيق Cross-Cutting Concerns:

### 1. Validation Behavior
```csharp
public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // Validate request
        // If invalid, throw ValidationException
        return await next();
    }
}
```

### 2. Logging Behavior
```csharp
public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // Log request
        var response = await next();
        // Log response
        return response;
    }
}
```

### 3. Performance Behavior
```csharp
public class PerformanceBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();
        
        if (stopwatch.ElapsedMilliseconds > 500)
        {
            // Log slow request
        }
        
        return response;
    }
}
```

---

## Testing Examples

### Unit Test for Handler

```csharp
public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var mockAuthService = new Mock<IAuthService>();
        mockAuthService
            .Setup(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthResponseDto { Success = true });
        
        var handler = new LoginCommandHandler(mockAuthService.Object);
        var command = new LoginCommand("test@example.com", "Test@1234");
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.True(result.Success);
    }
}
```

---

## API Endpoints

جميع الـ endpoints تعمل بنفس الطريقة السابقة، لكن الآن تستخدم MediatR في الخلفية:

### 1. Login
```bash
POST /api/Auth/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "NewTest@1234"
}
```

### 2. Forget Password
```bash
POST /api/Auth/forget-password
Content-Type: application/json

{
  "email": "test@example.com"
}
```

**ملاحظة:** الآن يتم إرسال البريد الإلكتروني تلقائياً (إذا تم تكوين SMTP بشكل صحيح).

### 3. Reset Password
```bash
POST /api/Auth/reset-password
Content-Type: application/json

{
  "email": "test@example.com",
  "token": "RESET_TOKEN_FROM_EMAIL",
  "newPassword": "NewPassword@1234",
  "confirmPassword": "NewPassword@1234"
}
```

### 4. Refresh Token
```bash
POST /api/Auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "YOUR_REFRESH_TOKEN"
}
```

---

## Packages المستخدمة

```xml
<PackageReference Include="MediatR" Version="13.1.0" />
<PackageReference Include="MailKit" Version="4.14.1" />
<PackageReference Include="MimeKit" Version="4.14.0" />
```

---

## Next Steps (التطوير المستقبلي)

1. ✅ إضافة Validation Behavior باستخدام FluentValidation
2. ✅ إضافة Logging Behavior
3. ✅ إضافة Unit Tests للـ Handlers
4. ✅ إضافة Queries للقراءة (GetCurrentUser, GetEmployeeById, etc.)
5. ✅ إضافة Caching Behavior
6. ✅ إضافة Transaction Behavior
7. ✅ إضافة Authorization Behavior

---

## الخلاصة

تم إعادة هيكلة المشروع بنجاح لاستخدام:
- ✅ **CQRS Pattern** لفصل Commands و Queries
- ✅ **MediatR** لإدارة الـ requests
- ✅ **Email Service** لإرسال البريد الإلكتروني
- ✅ **Clean Architecture** مع separation of concerns واضح

المشروع الآن أكثر قابلية للصيانة والاختبار والتوسع! 🎉

