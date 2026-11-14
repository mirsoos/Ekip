# 🏛️ مستندات معماری داده و دامنه (Enterprise Domain & Data Architecture) - اپلیکیشن Ekip
## 🟠 فاز ۴: جغرافیای بین‌المللی سه لایه، کیف پول و پرداخت هیبرید، نشست‌های امنیتی دستگاه‌ها و حافظه موقت (Redis)

---

### بخش اول: معماری جغرافیایی بین‌المللی (`Ekip.Domain.Entities.Location`)

#### ۱. مدل کشورها و ریجن‌ها (`Country`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.Location`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای کشور (UUIDv7). |
| `NativeName` | `string` | نام کشور به زبان محلی (مثلاً: `ایران`, `Türkiye`). |
| `EnglishName` | `string` | نام بین‌المللی کشور به انگلیسی (مثلاً: `Iran`, `Turkey`). |
| `IsoCode` | `string` (Unique) | کد استاندارد دوحرفی ISO 3166-1 (مثلاً `IR`, `TR`, `AE`) جهت تشخیص خودکار ریجن با IP. |
| `PhonePrefix` | `string` | پیش‌شماره تلفن کشور (مثلاً `+98`). |
| `CurrencyCode` | `string` | واحد پولی استاندارد ISO 4217 کشور (مثلاً `IRR`, `TRY`, `USD`). |
| `DefaultTimeZoneId` | `string` | منطقه زمانی پیش‌فرض کشور (مثلاً `Asia/Tehran`). |
| `DisplayOrder` | `int` | اولویت نمایش در لیست انتخاب ریجن (پیش‌فرض: `0`). |
| `IsActive` | `bool` | وضعیت فعال بودن سرویس در این کشور (پیش‌فرض: `true`). |
| `Provinces` | `IReadOnlyCollection<Province>` | لیست کپسوله‌شده استان‌ها/ایالت‌های کشور (`_provinces`). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین ویرایش (UTC). |

---

#### ۲. مدل استان‌ها و ایالت‌ها (`Province`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.Location`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای استان (UUIDv7). |
| `CountryId` | `Guid` (FK) | متصل به `Country.Id`. |
| `NativeName` | `string` | نام استان به زبان محلی (مثلاً: `مازندران`). |
| `EnglishName` | `string` | نام استان به انگلیسی (مثلاً: `Mazandaran`). |
| `DisplayOrder` | `int` | اولویت نمایش در لیست استان‌ها (پیش‌فرض: `0`). |
| `IsActive` | `bool` | وضعیت فعال بودن استان (پیش‌فرض: `true`). |
| `Cities` | `IReadOnlyCollection<City>` | لیست کپسوله‌شده شهرهای استان (`_cities`). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین ویرایش (UTC). |

---

#### ۳. مدل شهرها (`City`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.Location`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای شهر (UUIDv7). |
| `CountryId` | `Guid` (FK) | کلید مستقیم به کشور جهت جستجوی سریع شهر در سطح ریجن. |
| `ProvinceId` | `Guid` (FK) | متصل به `Province.Id`. |
| `NativeName` | `string` | نام شهر به زبان محلی (مثلاً: `چالوس`). |
| `EnglishName` | `string` | نام شهر به انگلیسی (مثلاً: `Chalus`). |
| `CenterPoint` | `GeoLocation` | مختصات جغرافیایی مرکز شهر جهت تنظیم Viewport اولیه نقشه و شعاع جستجو. |
| `DefaultZoomLevel` | `double` | سطح زوم پیش‌فرض نقشه در React Native (پیش‌فرض: `12.0`). |
| `DisplayOrder` | `int` | اولویت نمایش در لیست شهرها (پیش‌فرض: `0`). |
| `IsActive` | `bool` | وضعیت فعال بودن سرویس در شهر (پیش‌فرض: `true`). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین ویرایش (UTC). |

---

### بخش دوم: موتور کیف پول، دفتر کل مالی و پرداخت هیبرید (`Ekip.Domain.Entities.Billing`)

#### ۴. مدل کیف پول داخلی کاربر (`UserWallet`)
* **وراثت:** `Entity`, `IAggregateRoot`, `IAuditableEntity` *(رابطه ۱ به ۱ با `UserAccount.Id`)*
* **مسیر:** `Ekip.Domain.Entities.Billing`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین مالی |
| :--- | :--- | :--- |
| `UserId` | `Guid` (PK, FK) | شناسه کاربر صاحب کیف پول. |
| `Balance` | `decimal` | موجودی کل کیف پول (پیش‌فرض: `0`). |
| `LockedBalance` | `decimal` | موجودی مسدودشده موقت حین هدایت به درگاه بانک در پرداخت هیبرید جهت جلوگیری از Double-Spending (پیش‌فرض: `0`). |
| `AvailableBalance` | `decimal` (Computed) | موجودی قابل استفاده در لحظه (`Balance - LockedBalance`). |
| `Currency` | `string` | واحد پولی کیف پول (مانند `IRR`, `USD`). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد کیف پول (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تراکنش (UTC). |

---

#### ۵. مدل دفتر کل گردش حساب کیف پول (`WalletLedger`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`, `IAppendOnlyEntity` *(جدول Append-Only و تغییرناپذیر)*
* **مسیر:** `Ekip.Domain.Entities.Billing`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین مالی |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه گردش مالی کیف پول (UUIDv7). |
| `IdempotencyKey` | `Guid` (Unique) | کلید یکتا جهت جلوگیری از ثبت مضاعف تراکنش. |
| `UserId` | `Guid` (FK) | کاربر صاحب کیف پول. |
| `Type` | `WalletTransactionType` | نوع تراکنش (`DirectTopUp`, `ReferralReward`, `PromoGift`, `RefundCredit`, `PurchaseDeduction`, `AdminAdjustment`). |
| `AmountDelta` | `decimal` | مبلغ تغییر (مثبت برای واریز، منفی برای برداشت). |
| `BalanceAfter` | `decimal` | مانده حساب کیف پول بلافاصله پس از اعمال این تراکنش. |
| `Currency` | `string` | واحد پولی. |
| `ReferenceEntityId` | `Guid?` | شناسه رکورد مرجع (`PaymentTransaction`, `Ekip`, `UserAccount`). |
| `Description` | `string?` | شرح تراکنش جهت نمایش در تاریخچه کیف پول کاربر. |
| `CreatedAtUtc` | `DateTime` | زمان دقیق ثبت تراکنش (UTC). |

---

#### ۶. مدل تراکنش‌های خرید و درگاه پرداخت (`PaymentTransaction`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity` *(لاگ مالی تغییرناپذیر؛ پشتیبانی از پرداخت مستقیم، کیف پول کامل و هیبرید)*
* **مسیر:** `Ekip.Domain.Entities.Billing`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین مالی |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای فاکتور/تراکنش (UUIDv7). |
| `IdempotencyKey` | `Guid` (Unique) | کلید یکتا جهت جلوگیری از کسر وجه یا صدور فاکتور تکراری. |
| `UserId` | `Guid` (FK) | کاربر پرداخت‌کننده. |
| `Purpose` | `PaymentPurpose` | هدف پرداخت (`SubscriptionPurchase`, `WalletTopUp`, `EkipTicket`, `AdCampaignTopUp`). |
| `ReferenceEntityId` | `Guid` | شناسه موجودیت هدف (شناسه پلن اشتراک، اکیپ، یا کیف پول). |
| `PromoCodeId` | `Guid?` (FK) | کد تخفیف اعمال‌شده (اختیاری). |
| `BaseAmount` | `decimal` | مبلغ پایه پیش از تخفیف و کسر کیف پول. |
| `DiscountAmount` | `decimal` | مبلغ کسرشده بابت کد تخفیف (پیش‌فرض: `0`). |
| `WalletAmountUsed` | `decimal` | مبلغ تامین‌شده از محل کیف پول داخلی در پرداخت هیبرید یا کامل (پیش‌فرض: `0`). |
| `GatewayPayableAmount` | `decimal` | مبلغ نهایی ارسالی به درگاه بانک (`BaseAmount - DiscountAmount - WalletAmountUsed`). |
| `Currency` | `string` | واحد پولی تراکنش. |
| `Gateway` | `PaymentGateway` | درگاه پرداخت (`InternalWallet`, `ZarinPal`, `Zibal`, `Stripe`, `CafeBazaar`, `Myket`). |
| `AuthorityOrToken` | `string?` (Indexed) | توکن اولیه درگاه پرداخت. |
| `ReferenceId` | `string?` (Unique) | کد پیگیری نهایی شاپرک/درگاه پس از تایید (`Verify`). |
| `Status` | `PaymentStatus` | وضعیت تراکنش (`Pending`, `Succeeded`, `Failed`, `Refunded`). |
| `FailureReason` | `string?` | علت یا کد خطای برگشتی از درگاه. |
| `PaidAtUtc` | `DateTime?` | زمان تایید نهایی پرداخت (UTC). |
| `RefundedAtUtc` | `DateTime?` | زمان عودت وجه (UTC). |
| `CreatedAtUtc` | `DateTime` | زمان صدور فاکتور (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر وضعیت (UTC). |

---

### بخش سوم: امنیت نشست‌ها و دستگاه‌های کاربر (`Ekip.Domain.Entities.User`)

#### ۷. مدل نشست‌ها و دستگاه‌های فعال (`UserDeviceSession`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.User`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین امنیتی |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه نشست (UUIDv7). |
| `UserId` | `Guid` (FK) | شناسه کاربر (دارای Unique Index ترکیبی روی `(UserId, DeviceId)`). |
| `DeviceId` | `string` | شناسه یکتای سخت‌افزار گوشی جهت جلوگیری از ایجاد نشست‌های تکراری برای یک دستگاه واحد. |
| `DeviceType` | `DeviceType` | پلتفرم دستگاه (`Android = 1`, `Ios`, `Web`, `Desktop`). |
| `DeviceName` | `string?` | نام/مدل دستگاه (مثلاً `Samsung Galaxy S24`). |
| `AppVersion` | `string?` | نسخه اپلیکیشن کلاینت. |
| `FcmPushToken` | `string?` (Indexed) | توکن Firebase / APNs جهت ارسال Push Notification. |
| `RefreshTokenHash` | `string` (Unique) | هش توکن ریفرش فعلی. |
| `TokenVersion` | `int` | شمارنده چرخش توکن (RTR) جهت تشخیص آنی سرقت توکن و حملات Replay (پیش‌فرض: `1`). |
| `IpAddress` | `string?` | آخرین آدرس IP متصل‌شده. |
| `LastActiveAtUtc` | `DateTime` | زمان آخرین فعالیت یا چرخش توکن (UTC). |
| `ExpiresAtUtc` | `DateTime` | زمان انقضای قطعی نشست (UTC). |
| `IsRevoked` | `bool` | وضعیت ابطال نشست (پیش‌فرض: `false`). |
| `RevokedAtUtc` | `DateTime?` | زمان ابطال نشست (UTC). |
| `RevokedReason` | `string?` | علت ابطال (`UserLogout`, `TokenReuseDetected`, `AdminBan`). |
| `IsActiveAt(DateTime utcNow)` | `bool` (Method) | بررسی فعال بودن نشست با زمان مرجع `utcNow`: `!IsRevoked && ExpiresAtUtc > utcNow`. |
| `CreatedAtUtc` | `DateTime` | زمان اولین ورود با این دستگاه (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین بروزرسانی نشست (UTC). |

---

### بخش چهارم: زیرساخت پیام‌رسانی اتمیک و حافظه موقت (`Ekip.Infrastructure`)

#### ۱. مدیریت Transactional Outbox Pattern
* جداول `OutboxMessage`, `OutboxState`, `InboxState` به صورت بومی در لایه `Ekip.Infrastructure` توسط `MassTransit.EntityFrameworkCoreIntegration` مدیریت می‌شوند.

#### ۲. ساختار حافظه موقت در Redis (`Ephemeral Redis Schema`)

| الگوی کلید در Redis (Key Pattern) | نوع ساختار داده | زمان انقضا (TTL) | کاربرد در معماری Ekip |
| :--- | :--- | :---: | :--- |
| `idempotency:{user_id}:{idempotency_key}` | String | `120 Seconds` | قفل توزیع‌شده و کش پاسخ درخواست‌های در حال پردازش. |
| `otp:register:{phone_number}` | Hash | `120 Seconds` | کد تایید پیامکی ثبت‌نام + شمارنده تلاش ناموفق. |
| `otp:2fa:{user_id}` | Hash | `120 Seconds` | کد تایید دو مرحله‌ای لاگین. |
| `otp:reset_pw:{phone_or_email}` | Hash | `300 Seconds` | کد بازیابی رمز عبور. |
| `presence:online:{user_id}` | String | `60 Seconds` | وضعیت آنلاین لحظه‌ای کاربر. |
| `quota:ai_daily:{user_id}:{date}` | Integer (Counter) | `24 Hours` | شمارنده سریع مصرف روزانه پیام‌های هوش مصنوعی. |