# 🏛️ مستندات معماری داده و دامنه (Enterprise Domain & Data Architecture) - اپلیکیشن Ekip
## 🟠 فاز ۴: داده‌های پایه جغرافیایی، تراکنش‌های مالی، نشست‌های امنیتی دستگاه‌ها و ساختار حافظه موقت (Redis)

---

### بخش اول: موجودیت‌های دامنه در `Ekip.Domain`

#### ۱. مدل پایه استان‌ها و شهرها (`City`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.Location`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای شهر (UUIDv7). |
| `Name` | `string` | نام شهر (مثلاً: تهران، شیراز، اصفهان). |
| `ProvinceName` | `string` | نام استان. |
| `Slug` | `string` (Unique) | نام انگلیسی و یکتای شهر جهت لینک‌ها و URL (مثلاً `tehran`). |
| `CenterPoint` | `GeoLocation` | مختصات جغرافیایی مرکز شهر جهت تنظیم Viewport اولیه نقشه (Value Object). |
| `DefaultZoomLevel` | `double` | سطح زوم پیش‌فرض نقشه در کلاینت React Native (پیش‌فرض: `12.0`). |
| `DisplayOrder` | `int` | اولویت نمایش در لیست انتخاب شهر (پیش‌فرض: `0`). |
| `IsActive` | `bool` | وضعیت فعال بودن سرویس در شهر (پیش‌فرض: `true`). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت شهر (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین ویرایش (UTC). |

---

#### ۲. مدل تراکنش‌های مالی و درگاه پرداخت (`PaymentTransaction`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity` *(لاگ مالی تغییرناپذیر؛ فاقد حذف نرم یا فیزیکی)*
* **مسیر:** `Ekip.Domain.Entities.Billing`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای تراکنش داخلی (UUIDv7). |
| `IdempotencyKey` | `Guid` (Unique) | کلید یکتا جهت جلوگیری از ایجاد فاکتور یا کسر وجه تکراری در تلاش مجدد شبکه. |
| `UserId` | `Guid` (FK) | کاربر پرداخت‌کننده (متصل به `UserAccount.Id`). |
| `SubscriptionPlanId` | `Guid` (FK) | پلن اشتراک مورد تقاضا (متصل به `SubscriptionPlan.Id`). |
| `PromoCodeId` | `Guid?` (FK) | شناسه کد تخفیف اعمال‌شده (متصل به `PromoCode.Id` - اختیاری). |
| `BaseAmount` | `decimal` | مبلغ پایه پلن پیش از اعمال تخفیف. |
| `DiscountAmount` | `decimal` | مبلغ تخفیف کسرشده (پیش‌فرض: `0`). |
| `FinalPayableAmount` | `decimal` | مبلغ نهایی قابل پرداخت (`BaseAmount - DiscountAmount`). |
| `Currency` | `string` | واحد پولی تراکنش (مانند `IRR`, `USD` - هماهنگ با `SubscriptionPrice`). |
| `Gateway` | `PaymentGateway` | درگاه پرداخت (`ZarinPal`, `Zibal`, `Stripe`, `CafeBazaar`, `Myket`). |
| `AuthorityOrToken` | `string?` (Indexed) | توکن اولیه دریافتی از درگاه پیش از هدایت به صفحه پرداخت. |
| `ReferenceId` | `string?` (Unique) | کد پیگیری نهایی شاپرک/درگاه پس از تایید تراکنش (`Verify`). |
| `Status` | `PaymentStatus` | وضعیت تراکنش (`Pending = 1`, `Succeeded`, `Failed`, `Refunded`). |
| `FailureReason` | `string?` | علت یا کد خطای برگشتی از درگاه در صورت ناموفق بودن پرداخت. |
| `PaidAtUtc` | `DateTime?` | زمان دقیق تایید پرداخت (UTC). |
| `RefundedAtUtc` | `DateTime?` | زمان عودت وجه به کاربر (UTC). |
| `CreatedAtUtc` | `DateTime` | زمان صدور فاکتور (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر وضعیت تراکنش (UTC). |

---

#### ۳. مدل نشست‌ها و دستگاه‌های کاربر (`UserDeviceSession`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`
* **مسیر:** `Ekip.Domain.Entities.User`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین امنیتی |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه نشست/دستگاه (UUIDv7). |
| `UserId` | `Guid` (FK) | شناسه کاربر (متصل به `UserAccount.Id`). |
| `DeviceType` | `DeviceType` | نوع دستگاه (`Android = 1`, `Ios`, `Web`, `Desktop`). |
| `DeviceName` | `string?` | مدل دستگاه یا مشخصات سیستم‌عامل. |
| `AppVersion` | `string?` | نسخه کلاینت نصب‌شده روی دستگاه. |
| `FcmPushToken` | `string?` (Indexed) | توکن Firebase / APNs جهت ارسال Push Notification. |
| `RefreshTokenHash` | `string` (Unique) | هش توکن ریفرش با مکانیزم چرخش (`Refresh Token Rotation`). |
| `IpAddress` | `string?` | آخرین IP متصل‌شده جهت لاگ‌های امنیتی. |
| `LastActiveAtUtc` | `DateTime` | زمان آخرین فعالیت یا تمدید نشست (UTC). |
| `ExpiresAtUtc` | `DateTime` | زمان انقضای قطعی نشست (UTC). |
| `IsRevoked` | `bool` | وضعیت ابطال دستی یا امنیتی نشست (پیش‌فرض: `false`). |
| `RevokedAtUtc` | `DateTime?` | زمان ابطال نشست (UTC). |
| `IsCurrentlyActive` | `bool` (Computed) | پراپرتی محاسباتی: `!IsRevoked && ExpiresAtUtc > DateTime.UtcNow`. |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد اولیه نشست (UTC). |

---

### بخش دوم: کاتالوگ Enumهای فاز ۴ (`Ekip.Domain.Enums`)

* **`PaymentGateway` (`Enums.Billing`):** `ZarinPal = 1`, `Zibal`, `Stripe`, `CafeBazaar`, `Myket`
* **`PaymentStatus` (`Enums.Billing`):** `Pending = 1`, `Succeeded`, `Failed`, `Refunded`
* **`DeviceType` (`Enums.User`):** `Android = 1`, `Ios`, `Web`, `Desktop`

---

### بخش سوم: زیرساخت پیام‌رسانی اتمیک و حافظه موقت (`Ekip.Infrastructure`)

#### ۱. مدیریت Transactional Outbox Pattern
* جداول مربوط به الگوی Outbox (`OutboxMessage`, `OutboxState`, `InboxState`) در لایه `Ekip.Domain` تعریف نمی‌شوند تا اصل Persistence Ignorance حفظ گردد.
* این جداول به صورت بومی در لایه `Ekip.Infrastructure` توسط پکیج `MassTransit.EntityFrameworkCoreIntegration` مدیریت و در قالب یک تراکنش اتمیک (ACID) با دیتابیس PostgreSQL ثبت می‌شوند.

#### ۲. ساختار حافظه موقت در Redis (`Ephemeral Redis Schema`)

| الگوی کلید در Redis (Key Pattern) | نوع ساختار داده | زمان انقضا (TTL) | کاربرد در معماری Ekip |
| :--- | :--- | :---: | :--- |
| `idempotency:{user_id}:{idempotency_key}` | String | `120 Seconds` | قفل توزیع‌شده و کش پاسخ درخواست‌های در حال پردازش جهت جلوگیری از ارسال تکراری. |
| `otp:register:{phone_number}` | Hash | `120 Seconds` | نگهداری کد تایید پیامکی ثبت‌نام به همراه شمارنده تلاش ناموفق (Rate Limiting). |
| `otp:2fa:{user_id}` | Hash | `120 Seconds` | کد تایید دو مرحله‌ای ورود به حساب. |
| `otp:reset_pw:{phone_or_email}` | Hash | `300 Seconds` | کد بازیابی رمز عبور. |
| `presence:online:{user_id}` | String | `60 Seconds` | وضعیت آنلاین لحظه‌ای کاربر (تمدید با هر Heartbeat در gRPC). |
| `quota:ai_daily:{user_id}:{date}` | Integer (Counter) | `24 Hours` | شمارنده سریع مصرف روزانه پیام‌های هوش مصنوعی کاربر. |