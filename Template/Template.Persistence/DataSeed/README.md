# Data Seeder

هذا الملف يحتوي على DataSeed لإضافة بيانات أولية للنظام.

## الأدوار (Roles)

يتم إنشاء 4 أدوار افتراضية:

### 1. Admin
- **الحالة**: Active
- **الصلاحيات**:
  - Employees.View
  - Employees.Create
  - Employees.Update
  - Employees.Delete
  - Roles.View
  - Roles.Create
  - Roles.Update
  - Roles.Delete

### 2. Manager
- **الحالة**: Active
- **الصلاحيات**:
  - Employees.View
  - Employees.Create
  - Employees.Update
  - Roles.View

### 3. Employee
- **الحالة**: Active
- **الصلاحيات**:
  - Employees.View

### 4. Viewer
- **الحالة**: Active
- **الصلاحيات**:
  - Employees.View
  - Roles.View

## الموظفين (Employees)

يتم إنشاء 10 موظفين افتراضيين:

| الاسم الأول | الاسم الأخير | اسم المستخدم | البريد الإلكتروني | رقم الهاتف | كلمة المرور | الحالة | الدور |
|------------|--------------|--------------|-------------------|------------|------------|--------|-------|
| Admin | User | admin | admin@example.com | +1234567890 | Admin@123 | Active | Admin |
| John | Manager | john.manager | john.manager@example.com | +1234567891 | Manager@123 | Active | Manager |
| Jane | Smith | jane.smith | jane.smith@example.com | +1234567892 | Employee@123 | Active | Employee |
| Bob | Johnson | bob.johnson | bob.johnson@example.com | +1234567893 | Employee@123 | Active | Employee |
| Alice | Williams | alice.williams | alice.williams@example.com | +1234567894 | Viewer@123 | Active | Viewer |
| Charlie | Brown | charlie.brown | charlie.brown@example.com | +1234567895 | Employee@123 | Inactive | Employee |
| David | Davis | david.davis | david.davis@example.com | +1234567896 | Manager@123 | Active | Manager |
| Emma | Wilson | emma.wilson | emma.wilson@example.com | +1234567897 | Employee@123 | Active | Employee |
| Frank | Moore | frank.moore | frank.moore@example.com | +1234567898 | Viewer@123 | Active | Viewer |
| Grace | Taylor | grace.taylor | grace.taylor@example.com | +1234567899 | Employee@123 | Active | Employee |

## كيفية الاستخدام

يتم تشغيل DataSeed تلقائياً عند بدء التطبيق من خلال `Program.cs`:

```csharp
// Seed Data
using (var scope = app.Services.CreateScope())
{
    await DataSeeder.SeedDataAsync(scope.ServiceProvider);
}
```

## ملاحظات

- يتم التحقق من وجود البيانات قبل الإضافة لتجنب التكرار
- جميع الموظفين لديهم تأكيد البريد الإلكتروني ورقم الهاتف
- كلمات المرور تتبع سياسة الأمان المطلوبة (8 أحرف على الأقل، حرف كبير، حرف صغير، رقم، رمز خاص)
- يتم طباعة رسائل في Console لتتبع عملية الإضافة

## تسجيل الدخول

يمكنك استخدام أي من الحسابات التالية لتسجيل الدخول:

- **Admin**: `admin` / `Admin@123`
- **Manager**: `john.manager` / `Manager@123`
- **Employee**: `jane.smith` / `Employee@123`
- **Viewer**: `alice.williams` / `Viewer@123`

