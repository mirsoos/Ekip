# 🏛️ مستندات معماری داده (Enterprise Data Architecture) - اپلیکیشن Ekip
## 🟠 فاز ۴: تراکنش‌های مالی، داده‌های پایه جغرافیایی، دستگاه‌ها، Outbox Pattern و زیرساخت امنیتی (۵ مدل)

---

### ۱. مدل پایه استان‌ها و شهرها (`City`)
*جدول مرجع جهت فیلد `city_id` در پروفایل کاربران، اکیپ‌ها، فیلتر وکتوری و تبلیغات مکان همراه با مختصات مرکز شهر برای Viewport اولیه نقشه.*

| نام فیلد (Field) | نوع داده (Type) | اجباری؟ | توضیحات و قوانین معماری |
| :--- | :--- | :---: | :--- |
| `id` | UUID (PK) | بله | شناسه یکتای شهر. |
| `name` | String | بله | نام شهر (مثلاً: تهران، شیراز، استانبول). |
| `province_name` | String | بله | نام استان. |
| `slug` | String (Unique) | بله | نام انگلیسی و لینک‌پذیر شهر (جهت اشتراک‌گذاری). |
| `center_point` | `GEOGRAPHY(Point, 4326)` | بله | مختصات جغرافیایی مرکز شهر (Spatial Indexed). |
| `default_zoom_level` | Float | بله | سطح زوم پیش‌فرض نقشه در کلاینت React Native. |
| `is_active` | Boolean | بله | وضعیت فعال بودن سرویس در شهر. |

---

### ۲. مدل تراکنش‌های مالی و درگاه پرداخت (`PaymentTransaction`)
*پشتیبانی از خرید اشتراک‌های چندسطحی با لاگ مالی تغییرناپذیر (Immutable Audit Log) و رعایت Idempotency.*

| نام فیلد (Field) | نوع داده (Type) | اجباری؟ | توضیحات و قوانین بیزینس |
| :--- | :--- | :---: | :--- |
| `id` | UUID (PK) | بله | شناسه یکتای تراکنش داخلی (UUIDv7). |
| `idempotency_key` | String (Unique) | بله | **کلید یکتا:** جلوگیری از کسر وجه مکرر یا ایجاد فاکتور تکراری حین قطعی شبکه. |
| `user_id` | UUID (FK) | بله | کاربر پرداخت‌کننده (متصل به `UserAccount`). |
| `plan_id` | UUID (FK) | بله | پلن اشتراک مورد تقاضا (`SubscriptionPlan`). |
| `promo_code_id` | UUID (FK) | خیر | کد تخفیف اعمال‌شده (`PromoCode`). |
| `base_amount` | Decimal / BigInt| بله | مبلغ پایه پلن (ریال/واحد پولی). |
| `discount_amount` | Decimal / BigInt| بله | مبلغ تخفیف کسرشده. |
| `final_payable_amount` | Decimal / BigInt| بله | مبلغ نهایی ارسالی به درگاه پرداخت. |
| `payment_gateway` | Enum | بله | درگاه پرداخت (`ZARINPAL`, `ZIBAL`, `STRIPE`, `CAFEBAZAAR`, `MYKET`). |
| `authority_or_token` | String (Indexed)| خیر | توکن اولیه دریافتی از درگاه پیش از هدایت به صفحه پرداخت. |
| `reference_id` | String (Unique) | خیر | کد پیگیری نهایی شاپرک/درگاه پس از تایید تراکنش (`Verify`). |
| `status` | Enum | بله | `PENDING`, `SUCCESS` (فعال‌سازی آنی رکورد در `UserSubscription`), `FAILED`, `REFUNDED`. |
| `paid_at` | Timestamp | خیر | زمان دقیق تایید پرداخت (UTC). |
| `created_at` | Timestamp | بله | زمان صدور فاکتور (UTC). |

---

### ۳. مدل نشست‌ها و دستگاه‌های کاربر (`UserDeviceSession`)
*مدیریت امنیت اکانت، نگهداری Refresh Token با مکانیزم چرخش (Rotation) و ارسال Push Notification به گوشی.*

| نام فیلد (Field) | نوع داده (Type) | اجباری؟ | توضیحات و قوانین معماری |
| :--- | :--- | :---: | :--- |
| `id` | UUID (PK) | بله | شناسه نشست/دستگاه. |
| `user_id` | UUID (FK) | بله | شناسه کاربر (متصل به `UserAccount`). |
| `device_type` | Enum | بله | `ANDROID`, `IOS`, `WEB`, `DESKTOP`. |
| `device_name` | String | خیر | مدل دستگاه یا مشخصات سیستم‌عامل. |
| `fcm_push_token` | String (Indexed)| خیر | توکن فایربیس/APNs جهت ارسال پوش‌نوتیفیکیشن هنگام بسته بودن اپلیکیشن. |
| `refresh_token_hash` | String (Unique) | بله | **Refresh Token Rotation (RTR):** هش توکن ریفرش؛ با هر بار مصرف باطل شده و توکن جدید صادر می‌شود. استفاده مجدد از توکن باطل‌شده نشان‌دهنده نشت اطلاعاتی بوده و تمامی نشست‌های کاربر را درجا ابطال می‌کند. |
| `ip_address` | String | خیر | آخرین IP متصل‌شده جهت لاگ‌های امنیتی. |
| `last_active_at` | Timestamp | بله | زمان آخرین فعالیت (UTC). |
| `expires_at` | Timestamp | بله | زمان انقضای قطعی نشست (UTC). |

---

### ۴. مدل رویدادهای سیستمی جهت حل تضاد داده (`OutboxEvent`)
*پیاده‌سازی Transactional Outbox Pattern جهت تضمین ارسال رویدادها به ورکرها، Redis و نوتیفیکیشن در قالب یک تراکنش اتمیک (ACID) با دیتابیس.*

| نام فیلد (Field) | نوع داده (Type) | اجباری؟ | توضیحات مهندسی سیستم‌های توزیع‌شده |
| :--- | :--- | :---: | :--- |
| `id` | UUID (PK) | بله | شناسه یکتای رویداد (UUIDv7). |
| `aggregate_type` | String | بله | موجودیت مبدا (`EKIP`, `APPLICATION`, `USER`, `PAYMENT`). |
| `aggregate_id` | UUID | بله | شناسه رکورد در جدول اصلی. |
| `event_type` | String | بله | نام رویداد (مانند `EkipCreated`, `ApplicationApproved`, `PaymentSucceeded`). |
| `payload` | JSONB | بله | داده کامل رویداد ارسالی به Background Worker / Event Bus. |
| `status` | Enum | بله | `PENDING`, `PUBLISHED`, `FAILED`. |
| `retry_count` | Integer | بله | تعداد دفعات تلاش ناموفق برای انتشار. |
| `created_at` | Timestamp | بله | زمان درج همگام با تراکنش دیتابیس (UTC). |

---

### ۵. ساختار حافظه موقت کدهای تایید و وضعیت لحظه‌ای (`Ephemeral Redis Schema`)
*داده‌های دارای طول عمر کوتاه (TTL) و ترافیک نوشتن بالا مستقر در Redis:*

| الگوی کلید در Redis (Key Pattern) | نوع ساختار داده | زمان انقضا (TTL) | کاربرد در معماری Ekip |
| :--- | :--- | :---: | :--- |
| `idempotency:{user_id}:{idempotency_key}` | String | `120 Seconds` | ذخیره وضعیت و پاسخ درخواست‌های در حال پردازش جهت جلوگیری از درخواست‌های تکراری. |
| `otp:register:{phone_number}` | String (Hash) | `120 Seconds` | نگهداری کد تایید پیامکی ثبت‌نام + محدودکننده تلاش (Rate Limiting). |
| `otp:2fa:{user_id}` | String (Hash) | `120 Seconds` | کد تایید دو مرحله‌ای لاگین. |
| `otp:reset_pw:{phone_or_email}` | String (Hash) | `300 Seconds` | کد بازیابی رمز عبور. |
| `presence:online:{user_id}` | String | `60 Seconds` | وضعیت آنلاین لحظه‌ای کاربر (تمدید با هر Heartbeat پکت gRPC/WebSocket). |
| `quota:ai_daily:{user_id}:{date}` | Integer (Counter)| `24 Hours` | شمارنده سریع مصرف روزانه پیام‌های هوش مصنوعی. |