# 🏛️ مستندات معماری داده و دامنه (Enterprise Domain & Data Architecture) - اپلیکیشن Ekip
## 🔴 فاز ۵: پنل ادمین ایزوله (Zero-Trust)، بوکمارک، موتور کشف ترندها و تنظیمات پویا

---

### بخش اول: زیرساخت ایزوله پنل مدیریت (`Ekip.Domain.Entities.Admin`)

#### ۱. مدل حساب کاربری مدیران (`AdminAccount`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.Admin`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین امنیتی |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای ادمین (UUIDv7). |
| `UserName` | `string` (Unique) | نام کاربری جهت ورود به پنل مدیریت. |
| `Email` | `string` (Unique) | ایمیل سازمانی ادمین. |
| `FirstName` | `string` | نام ادمین. |
| `LastName` | `string` | نام خانوادگی ادمین. |
| `PasswordHash` | `string` | هش رمز عبور (Argon2id با پارامترهای سنگین سازمانی). |
| `Role` | `AdminRole` | نقش سازمانی (`SuperAdmin = 1`, `Moderator`, `SupportAgent`, `CommercialManager`). |
| `TotpSecret` | `string?` | کلید Google Authenticator (در اولین ورود تنظیم و فعال می‌شود). |
| `IsTwoFactorActivated` | `bool` | وضعیت فعال بودن تایید دو مرحله‌ای. |
| `FailedLoginAttempts` | `int` | شمارنده تلاش‌های ناموفق ورود جهت قفل خودکار (پیش‌فرض: `0`). |
| `LockedUntilUtc` | `DateTime?` | زمان پایان قفل موقت اکانت در صورت حمله Brute-Force (UTC). |
| `IsLockedOutAt(DateTime utcNow)` | `bool` (Method) | بررسی قفل بودن حساب با زمان مرجع `utcNow`: `LockedUntilUtc.HasValue && LockedUntilUtc.Value > utcNow`. |
| `IsActive` | `bool` | وضعیت فعال بودن حساب ادمین (پیش‌فرض: `true`). |
| `LastLoginAtUtc` | `DateTime?` | زمان آخرین ورود موفق (UTC). |
| `AllowedIps` | `IReadOnlyCollection<string>` | لیست سفید IPهای مجاز ورود (`_allowedIps`). |
| `Permissions` | `IReadOnlyCollection<string>` | دسترسی‌های ریزدانه کپسوله‌شده (`_permissions`). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد حساب (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین ویرایش (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم حساب ادمین. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم (UTC). |

---

#### ۲. مدل نشست‌های اختصاصی ادمین (`AdminSession`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.Admin`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین امنیتی |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه نشست ادمین (UUIDv7). |
| `AdminId` | `Guid` (FK) | متصل به `AdminAccount.Id`. |
| `RefreshTokenHash` | `string` (Unique) | هش توکن نشست ادمین. |
| `TokenVersion` | `int` | شمارنده چرخش توکن (RTR) جهت تشخیص سرقت توکن ادمین (پیش‌فرض: `1`). |
| `IpAddress` | `string` | آدرس IP متصل‌شده جهت بررسی مداوم با نشست. |
| `UserAgent` | `string?` | مشخصات مرورگر و سیستم‌عامل ادمین. |
| `ExpiresAtUtc` | `DateTime` | زمان انقضای قطعی نشست (UTC). |
| `IsRevoked` | `bool` | وضعیت ابطال نشست (پیش‌فرض: `false`). |
| `RevokedAtUtc` | `DateTime?` | زمان ابطال نشست (UTC). |
| `IsActiveAt(DateTime utcNow)` | `bool` (Method) | بررسی فعال بودن نشست با زمان مرجع `utcNow`: `!IsRevoked && ExpiresAtUtc > utcNow`. |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد نشست (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین چرخش توکن (UTC). |

---

#### ۳. مدل لاگ امنیتی اقدامات ادمین (`AdminAuditLog`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`, `IAppendOnlyEntity` *(جدول Append-Only و تغییرناپذیر)*
* **مسیر:** `Ekip.Domain.Entities.Admin`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه لاگ (UUIDv7). |
| `AdminId` | `Guid` (FK) | ادمین انجام‌دهنده عملیات. |
| `ActionType` | `AdminActionType` | نوع عملیات (`BanUser`, `UnbanUser`, `ApproveMedia`, `RejectMedia`, `ResolveReport`, `AdjustReputation`, `AdjustWallet`, `UpdateSetting`). |
| `TargetEntity` | `string` | نام موجودیت هدف (`UserAccount`, `Ekip`, `MediaAsset`, `SystemSetting` و...). |
| `TargetId` | `Guid` | شناسه یکتای رکورد هدف. |
| `OldValueJson` | `string?` | مقادیر پیش از تغییر (JSON - نال‌پذیر در عملیات ایجاد اولیه). |
| `NewValueJson` | `string` | مقادیر پس از تغییر (JSON - اجباری جهت ثبت وضعیت جدید). |
| `Reason` | `string?` | دلیل ثبت‌شده توسط ادمین برای انجام این تغییر. |
| `AdminIpAddress` | `string` | آدرس IP ادمین در لحظه انجام عملیات. |
| `CreatedAtUtc` | `DateTime` | زمان دقیق رخداد (UTC). |

---

### بخش دوم: بوکمارک و موتور کشف ترندها (`Ekip` & `Trend`)

#### ۴. مدل اکیپ‌های نشان‌شده / بوکمارک (`SavedEkip`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`
* **مسیر:** `Ekip.Domain.Entities.Ekip`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه رکورد (UUIDv7). |
| `UserId` | `Guid` (FK) | کاربر ذخیره‌کننده (دارای Unique Composite Index روی `(UserId, EkipId)`). |
| `EkipId` | `Guid` (FK) | اکیپ ذخیره‌شده. |
| `CreatedAtUtc` | `DateTime` | زمان ذخیره اکیپ (UTC). |

---

#### ۵. مدل ترندهای محاسبه‌شده هر شهر (`CityTrendSnapshot`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`
* **مسیر:** `Ekip.Domain.Entities.Trend`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه اسنپ‌شات ترند (UUIDv7). |
| `CityId` | `Guid` (FK) | شهر هدف (متصل به `City.Id`). |
| `Type` | `TrendType` | نوع ترند (`TagOrTopic = 1`, `LocationName`, `SubCategory`). |
| `Keyword` | `string` | کلیدواژه یا مکان ترندشده. |
| `TrendScore` | `decimal` | امتیاز محاسبه‌شده نرخ رشد در ۲۴ ساعت گذشته. |
| `ActiveEkipsCount` | `int` | تعداد اکیپ‌های باز مرتبط با این ترند. |
| `Rank` | `int` | رتبه ترند در شهر. |
| `CreatedAtUtc` | `DateTime` | زمان محاسبه اسنپ‌شات (UTC). |

---

#### ۶. مدل لاگ جستجوهای کاربران (`SearchQueryLog`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`
* **مسیر:** `Ekip.Domain.Entities.Trend`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه لاگ جستجو (UUIDv7). |
| `UserId` | `Guid?` (FK) | کاربر جستجوکننده (اختیاری). |
| `CityId` | `Guid` (FK) | شهر محل جستجو. |
| `NormalizedQuery` | `string` | عبارت نرمالایزشده جستجو. |
| `ResultsCount` | `int` | تعداد نتایج بازگردانده‌شده. |
| `CreatedAtUtc` | `DateTime` | زمان انجام جستجو (UTC). |

---

### بخش سوم: تنظیمات پویا و Feature Flagهای سیستم (`Ekip.Domain.Entities.Configuration`)

#### ۷. مدل تنظیمات سراسری سیستم (`SystemSetting`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity` *(دارای کلید اصلی `Guid Id` هماهنگ با کل سیستم و ایندکس یکتا روی `Key`)*
* **مسیر:** `Ekip.Domain.Entities.Configuration`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای تنظیم (UUIDv7). |
| `Key` | `string` (Unique) | کلید یکتای متنی (مانند `app.force_update_version`). |
| `ValueJson` | `string` | مقدار ساختاریافته تنظیمات (JSON). |
| `Description` | `string` | شرح عملکرد تنظیم در پنل مدیریت. |
| `IsPublic` | `bool` | مجوز ارسال این تنظیم به کلاینت‌های عمومی (موبایل/وب). |
| `CreatedByAdminId` | `Guid` (FK) | ادمین ایجادکننده اولیه تنظیم. |
| `UpdatedByAdminId` | `Guid?` (FK) | آخرین ادمین ویرایش‌کننده تنظیم. |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد تنظیم (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین بروزرسانی (UTC). |