# Authentication API Documentation

## Base URL
```
http://localhost:5049/api
```

## Endpoints

### 1. Login
تسجيل الدخول باستخدام البريد الإلكتروني وكلمة المرور.

**Endpoint:** `POST /Auth/login`

**Request Body:**
```json
{
  "email": "test@example.com",
  "password": "Test@1234"
}
```

**Response (Success):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "U+QJd92AxRmINbd+HjqDxO4H+P4gXXfgw/UqmKi+ydo=",
    "expiresAt": "2025-11-18T16:25:23.4069726Z",
    "employee": {
      "id": "fb3de9d0-d1c4-4f14-8e34-56484c3f7a08",
      "email": "test@example.com",
      "firstName": "Test",
      "lastName": "Employee",
      "fullName": "Test Employee",
      "department": "IT",
      "position": "Developer"
    }
  }
}
```

**Response (Error):**
```json
{
  "success": false,
  "message": "Invalid email or password",
  "data": null
}
```

---

### 2. Forget Password
طلب إعادة تعيين كلمة المرور.

**Endpoint:** `POST /Auth/forget-password`

**Request Body:**
```json
{
  "email": "test@example.com"
}
```

**Response:**
```json
{
  "success": true,
  "message": "If the email exists, a password reset link has been sent",
  "data": {
    "resetToken": "EsVZhBGU+CoQtp6mzjwEA780pSh//qj5E+II7E3HG0U="
  }
}
```

**ملاحظة:** في بيئة الإنتاج، يجب إرسال الـ token عبر البريد الإلكتروني وليس في الـ response.

---

### 3. Reset Password
إعادة تعيين كلمة المرور باستخدام الـ token.

**Endpoint:** `POST /Auth/reset-password`

**Request Body:**
```json
{
  "email": "test@example.com",
  "token": "EsVZhBGU+CoQtp6mzjwEA780pSh//qj5E+II7E3HG0U=",
  "newPassword": "NewTest@1234",
  "confirmPassword": "NewTest@1234"
}
```

**Password Requirements:**
- الحد الأدنى 8 أحرف
- يجب أن تحتوي على حرف كبير واحد على الأقل
- يجب أن تحتوي على حرف صغير واحد على الأقل
- يجب أن تحتوي على رقم واحد على الأقل
- يجب أن تحتوي على رمز خاص واحد على الأقل (@$!%*?&)

**Response (Success):**
```json
{
  "success": true,
  "message": "Password reset successful",
  "data": null
}
```

**Response (Error):**
```json
{
  "success": false,
  "message": "Invalid or expired reset token",
  "data": null
}
```

---

### 4. Refresh Token
تجديد الـ access token باستخدام الـ refresh token.

**Endpoint:** `POST /Auth/refresh-token`

**Request Body:**
```json
{
  "refreshToken": "U+QJd92AxRmINbd+HjqDxO4H+P4gXXfgw/UqmKi+ydo="
}
```

**Response (Success):**
```json
{
  "success": true,
  "message": "Token refreshed successfully",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "newRefreshTokenHere...",
    "expiresAt": "2025-11-18T17:25:23.4069726Z",
    "employee": {
      "id": "fb3de9d0-d1c4-4f14-8e34-56484c3f7a08",
      "email": "test@example.com",
      "firstName": "Test",
      "lastName": "Employee",
      "fullName": "Test Employee",
      "department": "IT",
      "position": "Developer"
    }
  }
}
```

**Response (Error):**
```json
{
  "success": false,
  "message": "Invalid or expired refresh token",
  "data": null
}
```

---

### 5. Get Current User
الحصول على معلومات المستخدم الحالي (يتطلب authentication).

**Endpoint:** `GET /Auth/me`

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Response:**
```json
{
  "userId": "fb3de9d0-d1c4-4f14-8e34-56484c3f7a08",
  "email": "test@example.com",
  "name": "Test Employee",
  "firstName": "Test",
  "lastName": "Employee",
  "department": "IT",
  "position": "Developer"
}
```

---

## Testing with cURL

### Login
```bash
curl -X POST "http://localhost:5049/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "Test@1234"}'
```

### Forget Password
```bash
curl -X POST "http://localhost:5049/api/Auth/forget-password" \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com"}'
```

### Reset Password
```bash
curl -X POST "http://localhost:5049/api/Auth/reset-password" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "token": "YOUR_RESET_TOKEN",
    "newPassword": "NewTest@1234",
    "confirmPassword": "NewTest@1234"
  }'
```

### Refresh Token
```bash
curl -X POST "http://localhost:5049/api/Auth/refresh-token" \
  -H "Content-Type: application/json" \
  -d '{"refreshToken": "YOUR_REFRESH_TOKEN"}'
```

### Get Current User
```bash
curl -X GET "http://localhost:5049/api/Auth/me" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

## Test Employee Credentials

تم إنشاء موظف تجريبي تلقائياً عند تشغيل المشروع:

- **Email:** test@example.com
- **Password:** Test@1234 (أو NewTest@1234 إذا تم تغييرها)
- **Role:** Employee
- **Department:** IT
- **Position:** Developer

---

## JWT Configuration

يتم تكوين JWT في `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!@#$%",
    "Issuer": "TemplateAPI",
    "Audience": "TemplateClient"
  }
}
```

**ملاحظة:** في بيئة الإنتاج، يجب تخزين الـ Key في متغيرات البيئة أو Azure Key Vault.

---

## Token Expiration

- **Access Token:** ساعة واحدة
- **Refresh Token:** 7 أيام
- **Reset Password Token:** ساعة واحدة

---

## Swagger UI

يمكنك الوصول إلى Swagger UI على:
```
http://localhost:5049/swagger
```

---

## Database Schema

### AspNetUsers (Employees)
- Id (Guid)
- Email
- FirstName
- LastName
- Department
- Position
- HireDate
- ResetPasswordToken
- ResetPasswordTokenExpiry
- ... (Identity fields)

### RefreshTokens
- Id (Guid)
- RefreshTokenHash
- ExpiresAt
- IsUsed
- IsRevoked
- DeviceId
- IpAddress
- UserId (FK to AspNetUsers)
- ... (Audit fields)

### AspNetRoles
- Id (Guid)
- Name (Admin, Manager, Employee)

---

## Error Handling

جميع الـ endpoints تُرجع response بالشكل التالي:

```json
{
  "success": true/false,
  "message": "رسالة توضيحية",
  "data": { ... } أو null
}
```

---

## Security Notes

1. **JWT Secret Key:** يجب تغيير الـ Key في الإنتاج وتخزينه بشكل آمن
2. **HTTPS:** يجب استخدام HTTPS في الإنتاج
3. **CORS:** قد تحتاج إلى تكوين CORS للسماح بالطلبات من الـ frontend
4. **Rate Limiting:** يُنصح بإضافة rate limiting لمنع هجمات brute force
5. **Email Service:** يجب تطبيق خدمة إرسال البريد الإلكتروني لـ forget password

