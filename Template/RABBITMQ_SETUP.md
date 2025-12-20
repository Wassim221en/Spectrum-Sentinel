# RabbitMQ Setup Guide

## تشغيل RabbitMQ باستخدام Docker

### الطريقة السريعة (Quick Start)

```bash
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=guest \
  -e RABBITMQ_DEFAULT_PASS=guest \
  rabbitmq:3-management
```

### شرح الأوامر

- `-d`: تشغيل الـ container في الخلفية (detached mode)
- `--name rabbitmq`: تسمية الـ container
- `-p 5672:5672`: Port للاتصال بـ RabbitMQ (AMQP Protocol)
- `-p 15672:15672`: Port للـ Management UI
- `-e RABBITMQ_DEFAULT_USER=guest`: اسم المستخدم الافتراضي
- `-e RABBITMQ_DEFAULT_PASS=guest`: كلمة المرور الافتراضية
- `rabbitmq:3-management`: الـ Image مع Management UI

## الوصول إلى RabbitMQ

### Management UI
- **URL**: http://localhost:15672
- **Username**: guest
- **Password**: guest

### AMQP Connection
- **Host**: localhost
- **Port**: 5672
- **Username**: guest
- **Password**: guest

## أوامر Docker المفيدة

### إيقاف RabbitMQ
```bash
docker stop rabbitmq
```

### تشغيل RabbitMQ (بعد الإيقاف)
```bash
docker start rabbitmq
```

### حذف RabbitMQ Container
```bash
docker rm -f rabbitmq
```

### عرض Logs
```bash
docker logs rabbitmq
```

### عرض Logs بشكل مباشر (Follow)
```bash
docker logs -f rabbitmq
```

## التحقق من التشغيل

### 1. التحقق من أن Container يعمل
```bash
docker ps | grep rabbitmq
```

### 2. التحقق من الاتصال
افتح المتصفح وانتقل إلى: http://localhost:15672

يجب أن ترى صفحة تسجيل الدخول لـ RabbitMQ Management UI.

## استخدام Docker Compose (اختياري)

يمكنك إنشاء ملف `docker-compose.yml`:

```yaml
version: '3.8'

services:
  rabbitmq:
    image: rabbitmq:3-management
    container_name: rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq
    networks:
      - template_network

volumes:
  rabbitmq_data:

networks:
  template_network:
    driver: bridge
```

### تشغيل باستخدام Docker Compose
```bash
docker-compose up -d
```

### إيقاف باستخدام Docker Compose
```bash
docker-compose down
```

## مراقبة RabbitMQ

### من خلال Management UI

1. افتح http://localhost:15672
2. سجل الدخول باستخدام guest/guest
3. يمكنك مشاهدة:
   - **Overview**: نظرة عامة على النظام
   - **Connections**: الاتصالات النشطة
   - **Channels**: القنوات النشطة
   - **Exchanges**: الـ Exchanges المتاحة
   - **Queues**: الـ Queues والرسائل فيها

### من خلال Command Line

```bash
# عرض قائمة الـ Queues
docker exec rabbitmq rabbitmqctl list_queues

# عرض قائمة الـ Exchanges
docker exec rabbitmq rabbitmqctl list_exchanges

# عرض قائمة الـ Connections
docker exec rabbitmq rabbitmqctl list_connections

# عرض حالة RabbitMQ
docker exec rabbitmq rabbitmqctl status
```

## Troubleshooting

### المشكلة: لا يمكن الوصول إلى Management UI

**الحل**:
1. تأكد من أن Container يعمل: `docker ps`
2. تحقق من الـ Logs: `docker logs rabbitmq`
3. تأكد من أن Port 15672 غير مستخدم من برنامج آخر

### المشكلة: لا يمكن الاتصال من التطبيق

**الحل**:
1. تأكد من أن RabbitMQ يعمل
2. تحقق من إعدادات الاتصال في `appsettings.json`
3. تأكد من أن Port 5672 غير محجوب من Firewall

### المشكلة: الرسائل لا تصل

**الحل**:
1. تحقق من أن الـ Exchange تم إنشاؤه
2. تحقق من أن الـ Queue تم إنشاؤه
3. تحقق من الـ Binding بين Exchange و Queue
4. راجع الـ Logs في التطبيق

## تكوين Production

للـ Production، يُنصح بـ:

1. **تغيير Username و Password**:
```bash
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=your_username \
  -e RABBITMQ_DEFAULT_PASS=your_strong_password \
  rabbitmq:3-management
```

2. **استخدام Volumes للبيانات**:
```bash
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -v rabbitmq_data:/var/lib/rabbitmq \
  -e RABBITMQ_DEFAULT_USER=your_username \
  -e RABBITMQ_DEFAULT_PASS=your_strong_password \
  rabbitmq:3-management
```

3. **تكوين Memory Limits**:
```bash
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -m 2g \
  --memory-swap 2g \
  -e RABBITMQ_DEFAULT_USER=your_username \
  -e RABBITMQ_DEFAULT_PASS=your_strong_password \
  rabbitmq:3-management
```

## الخطوات التالية

بعد تشغيل RabbitMQ:

1. قم بتشغيل التطبيق: `dotnet run --project Template.API`
2. قم بإنشاء موظف جديد عبر API
3. راقب الـ Logs لرؤية Integration Events
4. افتح RabbitMQ Management UI لرؤية الرسائل

## موارد إضافية

- [RabbitMQ Official Documentation](https://www.rabbitmq.com/documentation.html)
- [RabbitMQ Docker Hub](https://hub.docker.com/_/rabbitmq)
- [RabbitMQ Management Plugin](https://www.rabbitmq.com/management.html)

