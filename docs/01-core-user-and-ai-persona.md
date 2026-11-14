# 🏛️ مستندات معماری داده و دامنه (Enterprise Domain & Data Architecture) - اپلیکیشن Ekip
## 🟢 فاز ۱: هویت، موتور اشتراک، گیمیفیکیشن مبتنی بر Ledger، امنیت محتوا و بردار هوش مصنوعی (`pgvector`)

> **قراردادهای پایه معماری دامنه (Base Contracts):**
> * **`Entity`:** کلاس پایه مدیریت رویدادهای دامنه (`DomainEvents`).
> * **`BaseEntity`:** مشتق‌شده از `Entity` و دارای کلید اصلی `Guid Id` مبتنی بر **UUIDv7** (`Guid.CreateVersion7()`).
> * **`ICreatedAuditable`:** دارای `DateTime CreatedAtUtc`.
> * **`IAuditableEntity`:** مشتق‌شده از `ICreatedAuditable` و دارای `DateTime? UpdatedAtUtc`.
> * **`ISoftDeletable`:** دارای `bool IsDeleted` و `DateTime? DeletedAtUtc` (حذف نرم برای حفظ یکپارچگی کلیدهای خارجی و تاریخچه).
> * **`IAppendOnlyEntity`:** مارکر اینترفیس برای موجودیت‌های تغییرناپذیر و فقط-افزودنی (`ReputationLedger`, `WalletLedger`, `AdminAuditLog`) جهت جلوگیری از ویرایش یا حذف در زمان اجرا.
>
> **قراردادهای مدیریت خطا و نتیجه (`Ekip.Domain.Results` & `Ekip.Domain.Errors`):**
> * **الگوی `Result` خالص (`readonly struct`):** برای تمامی خطاهای قابل انتظار بیزینس و اعتبارسنجی ورودی کاربر از `Result` و `Result<TValue>` (پیاده‌سازی‌شده با `IResult` و `IResult<TSelf>` بدون Heap Allocation در مسیر موفقیت) استفاده می‌شود.
> * **مرز `Result` در برابر Exception:** خطاهای ورودی کاربر و قوانین دامنه از طریق `Result` برمی‌گردند؛ متدهای داخلی سرور هنگام نقض قرارداد برنامه‌نویس از Guard Clause (`ArgumentException` / `ArgumentOutOfRangeException`) استفاده می‌کنند. برای بازسازی سریع ValueObjectها هنگام خواندن از دیتابیس بدون سربار اعتبارسنجی، از متد `internal Rehydrate(...)` به همراه `InternalsVisibleTo("Ekip.Persistence")` استفاده می‌شود.
> * **ساختار `Error` و کد خطا:** هر خطا دارای `Code` ماشینی با فرمت **`MODULE.ERROR_CODE`** (`UPPER_SNAKE`)، `Message` انگلیسی مختصر (فقط جهت لاگ/Fallback و نه متن UI)، `ErrorType`، و `Metadata` اختیاری برای پارامترهای چندزبانه‌سازی (`i18n`) است. خطاهای چندتایی در `ValidationError` (دارای `ImmutableArray<Error>`) تجمیع و Flatten می‌شوند.
> * **سازمان‌دهی کاتالوگ خطاها:** خطاها به تفکیک ماژول بیزینسی در `Ekip.Domain/Errors/` نگهداری می‌شوند (مانند `LocationErrors` با پیشوند `LOCATION.*`).

---

### بخش اول: هسته هویت و پروفایل (Identity & Profile)

#### ۱. مدل حساب کاربری و امنیت (`UserAccount`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.User`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای کاربر (UUIDv7 - تولید خودکار در `BaseEntity`). |
| `Username` | `string` | نام کاربری یکتا (B-Tree Indexed). |
| `PhoneNumber` | `string?` | شماره موبایل یکتا (نال‌پذیر جهت پشتیبانی از ثبت‌نام اولیه با ایمیل یا OAuth). |
| `Email` | `string?` | ایمیل یکتا (اختیاری). |
| `PasswordHash` | `string?` | هش رمز عبور (Argon2id - نال‌پذیر در صورت ورود صرف با OAuth). |
| `AccountStatus` | `UserAccountStatus` | وضعیت حساب کاربری (پیش‌فرض: `Active`). |
| `TwoFactorEnabled` | `bool` | وضعیت فعال بودن تایید دو مرحله‌ای (پیش‌فرض: `false`). |
| `IsFaceVerified` | `bool` | تیک تایید چهره توسط هوش مصنوعی (پیش‌فرض: `false`). |
| `IsNationalIdVerified` | `bool` | تیک تایید هویت رسمی / کد ملی (پیش‌فرض: `false`). |
| `NationalIdHash` | `string?` | هش کد ملی (HMAC-SHA256) جهت جلوگیری از ساخت اکانت‌های متعدد. |
| `ReferralCode` | `string` | کد دعوت اختصاصی و یکتای کاربر. |
| `ReferredById` | `Guid?` (FK) | شناسه کاربر معرف (نال‌پذیر). |
| `HasCompletedTour` | `bool` | وضعیت مشاهده تور اولیه اپلیکیشن (پیش‌فرض: `false`). |
| `TermsAcceptedAt` | `DateTime` | زمان دقیق پذیرش قوانین و مقررات (UTC). |
| `OAuthProviders` | `IReadOnlyCollection<OAuthProviderLogin>` | لیست سرویس‌های OAuth متصل‌شده (ذخیره به صورت `JSONB`). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد حساب (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین ویرایش حساب (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم حساب کاربری. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم حساب کاربری (UTC). |

---

#### ۲. مدل پروفایل عمومی و حریم خصوصی (`UserProfile`)
* **وراثت:** `Entity`, `IAuditableEntity` *(رابطه ۱ به ۱ با `UserAccount` بدون نیاز به `ISoftDeletable` مستقل)*
* **مسیر:** `Ekip.Domain.Entities.User`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `UserId` | `Guid` (PK, FK) | کلید اصلی و خارجی متصل به `UserAccount.Id`. |
| `FirstName` | `string` | نام. |
| `LastName` | `string` | نام خانوادگی. |
| `UserGender` | `Gender` | جنسیت کاربر (`Male`, `Female`). |
| `BirthDate` | `DateOnly` | تاریخ تولد (مبنای محاسبه سن). |
| `CityId` | `Guid?` (FK) | شهر محل سکونت (متصل به `City` - اختیاری در ثبت‌نام اولیه). |
| `AvatarAssetId` | `Guid?` | شناسه یکتای فایل آواتار (جهت ساخت آدرس Zero-Join در CDN با الگوی `users/{UserId}/{AvatarAssetId}`). |
| `BannerAssetId` | `Guid?` | شناسه یکتای فایل بنر پروفایل (با الگوی `users/{UserId}/{BannerAssetId}`). |
| `Bio` | `string?` | بیوگرافی کوتاه کاربر. |
| `JobTitle` | `string?` | عنوان شغلی. |
| `MbtiTypes` | `PersonalMbtiTypes` | تیپ شخصیتی ۱۶گانه MBTI کاربر. |
| `MoodWidget` | `MoodWidget?` | ویجت وضعیت روحی کاربر (ذخیره به صورت `JSONB`). |
| `SocialLinks` | `IReadOnlyCollection<SocialLink>` | لیست شبکه‌های اجتماعی کاربر (ذخیره به صورت `JSONB`). |
| `Interests` | `IReadOnlyCollection<string>` | آرایه تگ‌های علاقه‌مندی (`varchar[]` - دارای ایندکس GIN). |
| `IsSearchable` | `bool` | مجوز نمایش پروفایل در نتایج جستجو (پیش‌فرض: `true`). |
| `ShowContacts` | `bool` | مجوز نمایش لیست دوستان به دیگران (پیش‌فرض: `true`). |
| `ShowLastSeen` | `bool` | مجوز نمایش وضعیت آنلاین و آخرین بازدید (پیش‌فرض: `true`). |
| `LastSeenAtUtc` | `DateTime?` | زمان آخرین فعالیت (سینک‌شده از روی Redis هنگام قطع اتصال - UTC). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد پروفایل (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین ویرایش پروفایل (UTC). |

---

### بخش دوم: موتور اشتراک چندسطحی و مدیریت سهمیه (Subscription & Quota Engine)

#### ۳. مدل تعریف پلن‌های اشتراک (`SubscriptionPlan`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`
* **مسیر:** `Ekip.Domain.Entities.Subscription`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای پلن (UUIDv7). |
| `Code` | `string` | کد سیستمی یکتای پلن (مانند `FREE_TIER`, `PLUS_MONTHLY`, `PRO_VIP`). |
| `Title` | `string` | نام نمایشی پلن. |
| `Price` | `SubscriptionPrice` | مبلغ و واحد پولی پلن (ValueObject). |
| `Entitlements` | `PlanEntitlements` | قوانین، دسترسی‌ها و سهمیه‌های پلن (ذخیره به صورت `JSONB`). |
| `DurationDays` | `int` | مدت اعتبار پلن به روز. |
| `IsActive` | `bool` | وضعیت فعال بودن پلن جهت خرید. |
| `CreatedAtUtc` | `DateTime` | زمان تعریف پلن (UTC). |

---

#### ۴. مدل وضعیت اشتراک فعال کاربر (`UserSubscription`)
* **وراثت:** `Entity`, `IAuditableEntity` *(رابطه ۱ به ۱ با `UserAccount`؛ تاریخچه مالی خریدها در `PaymentTransaction` نگهداری می‌شود)*
* **مسیر:** `Ekip.Domain.Entities.Subscription`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `UserId` | `Guid` (PK, FK) | کلید اصلی و خارجی متصل به `UserAccount.Id` (هر کاربر دقیقاً ۱ رکورد وضعیت دارد). |
| `IdempotencyKey` | `Guid` | کلید یکتای آخرین تراکنش اعمال‌شده جهت جلوگیری از اعمال مضاعف. |
| `SubscriptionPlanId` | `Guid` (FK) | پلن اشتراک فعلی کاربر (متصل به `SubscriptionPlan.Id`). |
| `StartsAtUtc` | `DateTime` | زمان شروع دوره اشتراک فعلی (UTC). |
| `ExpiresAtUtc` | `DateTime?` | زمان انقضای اشتراک (برای پلن رایگان `null` و برای پلن‌های پولی دارای تاریخ است - UTC). |
| `IsActiveAt(DateTime utcNow)` | `bool` (Method) | بررسی لحظه‌ای فعال بودن اشتراک با دریافت زمان مرجع `utcNow` (بدون وابستگی به `DateTime.UtcNow`): `ExpiresAtUtc is null \|\| ExpiresAtUtc > utcNow`. |
| `QuotaUsage` | `QuotaUsage` | وضعیت مصرف سهمیه روزانه هوش مصنوعی به عنوان پشتیبان پایدار Redis (`JSONB`). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد رکورد (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تمدید یا بروزرسانی سهمیه (UTC). |

---

### بخش سوم: موتور گیمیفیکیشن، اعتماد سیستم (Honor) و دفتر کل (Reputation Engine)

#### ۵. مدل دفتر کل رویدادهای اعتبار و تجربه (`ReputationLedger`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`, `IAppendOnlyEntity` *(جدول Append-Only و تغییرناپذیر)*
* **مسیر:** `Ekip.Domain.Entities.Gamification`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه تراکنش اعتباری (UUIDv7). |
| `UserId` | `Guid` (FK) | کاربر هدف که امتیاز دریافت یا از دست داده است. |
| `ActorId` | `Guid?` (FK) | کاربر یا ادمینِ عاملِ رخداد (جهت ردیابی و جلوگیری از XP Farming). |
| `Description` | `string?` | توضیحات یا دلیل ثبت رویداد (به‌ویژه در تغییرات دستی ادمین). |
| `IdempotencyKey` | `Guid` | کلید یکتا جهت جلوگیری از ثبت دوباره امتیاز در پیام‌های ناهمگام RabbitMQ. |
| `EventType` | `ReputationEventType` | نوع رویداد امتیازی (از بین ۱۶ رویداد پایه سیستم). |
| `XpDelta` | `int` | میزان تغییر XP در این رویداد. |
| `XpBalanceAfter` | `int` | موجودی کل XP کاربر بلافاصله پس از اعمال این تراکنش. |
| `HonorDelta` | `int` | میزان تغییر نمره اعتماد مخفی سیستم (Honor - مثبت یا منفی). |
| `HonorBalanceAfter` | `int` | موجودی کل Honor کاربر بلافاصله پس از اعمال این تراکنش. |
| `ReferenceEntityType` | `string?` | نام موجودیت مرجع (مانند `Ekip`, `Mission`, `SubscriptionPlan`, `ProfileReview`). |
| `ReferenceEntityId` | `Guid?` | شناسه رکورد مرجع. |
| `CreatedAtUtc` | `DateTime` | زمان دقیق وقوع رویداد (UTC). |

---

#### ۶. مدل خلاصه وضعیت گیمیفیکیشن کاربر (`UserGamificationSummary`)
* **وراثت:** `Entity`, `IAggregateRoot` *(نمای Materialized State با رابطه ۱ به ۱ برای کوئری‌های O(1))*
* **مسیر:** `Ekip.Domain.Entities.Gamification`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `UserId` | `Guid` (PK, FK) | شناسه کاربر (متصل به `UserAccount.Id`). |
| `TotalXp` | `int` | مجموع کل XP کسب‌شده. |
| `CurrentLevel` | `int` | سطح فعلی کاربر (پیش‌فرض: `1`). |
| `NextLevelXp` | `int` | میزان XP مورد نیاز جهت رسیدن به سطح بعدی (برای رندر Progress Bar در کلاینت). |
| `PublicScore` | `decimal` | میانگین دقیق امتیازات ۱ تا ۵ دریافتی از سایر اعضا (`RatingScoresSum / RatingsCount`). |
| `RatingScoresSum` | `int` | مجموع کل ستاره‌های دریافتی (جهت محاسبه دقیق میانگین بدون خطای گرد شدن). |
| `RatingsCount` | `int` | تعداد کل دفعات امتیازدهی به کاربر. |
| `TotalHonor` | `int` | نمره اعتماد داخلی سیستم به کاربر (مخفی از دید کاربر؛ پیش‌فرض: `0` خنثی). |
| `AggregatedCounters` | `IReadOnlyCollection<AggregatedCounters>` | لیست شمارنده‌های تجمیعی به تفکیک هر `ReputationEventType` (`JSONB`). |

---

### بخش چهارم: امنیت محتوای عمومی، نظرات و گراف ارتباطی (Trust, Safety & Social Graph)

#### ۷. مدل مدیریت رسانه‌های عمومی و اسکن Anti-NSFW (`MediaAsset`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable` *(مخصوص رسانه‌های عمومی مانند کاور اکیپ)*
* **مسیر:** `Ekip.Domain.Entities.Media`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای رسانه (UUIDv7). |
| `UploaderId` | `Guid` (FK) | کاربر آپلودکننده (متصل به `UserAccount.Id`). |
| `UsageContext` | `MediaUsageContext` | محل استفاده رسانه (مانند `EkipCover`). |
| `StorageKey` | `string` | کلید/مسیر فایل در Object Storage (S3/MinIO). |
| `NsfwProbability` | `decimal?` | نمره احتمال محتوای نامناسب (`null` پیش از اسکن، `0.0` تا `1.0` پس از اسکن). |
| `ModerationStatus` | `MediaModerationStatus` | وضعیت بررسی (`Pending`, `Approved`, `Rejected`, `EscalatedToAdmin`). |
| `Error` | `string?` | علت رد شدن یا خطای سرویس اسکن هوش مصنوعی. |
| `ScannedAtUtc` | `DateTime?` | زمان اتمام اسکن هوش مصنوعی (UTC). |
| `CreatedAtUtc` | `DateTime` | زمان آپلود (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر وضعیت (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم رسانه. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم رسانه (UTC). |

---

#### ۸. مدل نظرات پروفایل و امتیازدهی پس از اکیپ (`ProfileReview`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.Review`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه نظر (UUIDv7). |
| `IdempotencyKey` | `Guid` | کلید یکتا جهت جلوگیری از ثبت نظر تکراری در قطعی شبکه. |
| `TargetUserId` | `Guid` (FK) | صاحب پروفایل دریافت‌کننده امتیاز. |
| `AuthorUserId` | `Guid` (FK) | نویسنده نظر (مشروط به وضعیت `CHECKED_IN` در اکیپ مربوطه). |
| `EkipId` | `Guid` (FK) | اکیپ مشترکِ مبدا امتیازدهی. |
| `Score` | `int` | امتیاز عددی بین `1` تا `5`. |
| `CommentText` | `string?` | متن نظر (در صورت خالی بودن، بدون نیاز به اسکن AI مستقیم تایید می‌شود). |
| `ToxicityScore` | `decimal?` | نمره سمّی بودن/فحاشی متن (`null` پیش از اسکن AI). |
| `ModerationStatus` | `ProfileReviewModerationStatus` | وضعیت بررسی (`PendingByAI`, `Approved`, `Rejected`, `PendingByAdmin`). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت نظر (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین ویرایش (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم نظر. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم نظر (UTC). |

---

#### ۹. مدل گراف ارتباطی کاربران (`UserRelation`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.User`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه رابطه (UUIDv7). |
| `RequesterId` | `Guid` (FK) | کاربر آغازگر درخواست (یا فرد بلاک‌کننده در وضعیت `Blocked`). |
| `ReceiverId` | `Guid` (FK) | کاربر دریافت‌کننده درخواست (یا فرد بلاک‌شده). |
| `Status` | `UserRelationStatus` | وضعیت رابطه (`Pending`, `Accepted`, `Rejected`, `UnFollowed`, `Blocked`). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد رابطه (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر وضعیت رابطه (UTC). |

---

### بخش پنجم: معماری بردار هوش مصنوعی و پرسونای پویا (PostgreSQL `pgvector`)

#### ۱۰. مدل بردار هویت و پرسونای هوش مصنوعی (`UserIdentityVector`)
* **وراثت:** `Entity`, `IAggregateRoot`, `IAuditableEntity` *(رابطه ۱ به ۱ با `UserAccount`؛ جایگزین بهینه کالکشن‌های مجزا)*
* **مسیر:** `Ekip.Domain.Entities.AI`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `UserId` | `Guid` (PK, FK) | شناسه یکتای کاربر (متصل به `UserAccount.Id`). |
| `PersonalityVector` | `float[]` | بردار معنایی ۱۵۳۶ بعدی (`vector(1536)`) حاصل از ترکیب `Bio`, `Interests`, `MBTI` و `AiPersonaSummary`. |
| `AiPersonaSummary` | `string?` | خلاصه پویا و تجمیعی شناخت چت‌بات هوش مصنوعی از سلیقه و محدودیت‌های کاربر. |
| `CityId` | `Guid?` | شناسه شهر جهت Pre-filtering سریع بدون نیاز به Join در جستجوی برداری. |
| `UserGender` | `Gender` | جنسیت جهت Pre-filtering. |
| `BirthYear` | `int` | سال تولد جهت فیلتر بازه سنی در مچ‌میکینگ. |
| `CurrentLevel` | `int` | سطح فعلی کاربر. |
| `TotalHonor` | `int` | نمره اعتماد سیستم به کاربر. |
| `CreatedAtUtc` | `DateTime` | زمان تولید اولیه بردار (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین بازتولید بردار یا بروزرسانی پرسونا (UTC). |

---

### بخش ششم: کاتالوگ کامل ValueObjectها (`sealed record`) و Enumهای فاز ۱

#### الف) ValueObjectهای فاز ۱ (`Ekip.Domain.ValueObjects`)

| نام رکورد (Record) | مسیر (Namespace) | ساختار فیلدها (Properties & Types) |
| :--- | :--- | :--- |
| **`OAuthProviderLogin`** | `ValueObjects.User` | `string Provider`, `string ProviderUserId`, `string? Email`, `DateTime LinkedAtUtc` |
| **`MoodWidget`** | `ValueObjects.User` | `string? MoodEmoji`, `string? MoodDescription`, `string? MoodColor`, `DateTime CreatedAtUtc` |
| **`SocialLink`** | `ValueObjects.User` | `string Platform`, `string Url`, `DateTime LinkedAtUtc` |
| **`SubscriptionPrice`** | `ValueObjects.Subscription` | `decimal Amount`, `string Currency` |
| **`PlanEntitlements`** | `ValueObjects.Subscription` | `int MaxActiveEkips`, `int DailyAiMessagesQuota`, `bool CanCreateRecurringEkips`, `bool CanCreateBusinessEkips`, `IReadOnlyDictionary<string, string>? CustomPerks = null` |
| **`QuotaUsage`** | `ValueObjects.Subscription` | `int Used`, `int Limit`, `DateTime? ResetAtUtc` |
| **`AggregatedCounters`** | `ValueObjects.Gamification` | `ReputationEventType EventType`, `int Count` |

---

#### ب) Enumهای فاز ۱ (`Ekip.Domain.Enums`)

* **`UserAccountStatus` (`Enums.User`):** `Active`, `Suspended`, `ShadowBanned`, `Banned`
* **`Gender` (`Enums.User`):** `Male`, `Female`
* **`PersonalMbtiTypes` (`Enums.User`):** ۱۶ تیپ استاندارد MBTI (`INTJ`, `INTP`, `ENTJ`, `ENTP`, `INFJ`, `INFP`, `ENFJ`, `ENFP`, `ISTJ`, `ISFJ`, `ESTJ`, `ESFJ`, `ISTP`, `ISFP`, `ESTP`, `ESFP`)
* **`UserRelationStatus` (`Enums.User`):** `Pending = 1`, `Accepted`, `Rejected`, `UnFollowed`, `Blocked`
* **`ReputationEventType` (`Enums.Gamification`):** `EkipCreated`, `EkipCompleted`, `HostCanceledLate`, `LeftEkip`, `KickedFromEkip`, `NoShowPenalty`, `UserRated`, `UnwantedCompanionInvite`, `BlockedByUser`, `ReportConfirmed`, `NsfwViolation`, `FaceVerified`, `NationalIdVerified`, `ReferralBonus`, `MissionCompleted`, `SubscriptionPurchased`, `AdminManualAdjustment`
* **`MediaUsageContext` (`Enums.Media`):** `EkipCover = 1` *(و سایر رسانه‌های عمومی)*
* **`MediaModerationStatus` (`Enums.PublicMedia` / `Enums.Media`):** `Pending = 1`, `Approved`, `Rejected`, `EscalatedToAdmin`
* **`ProfileReviewModerationStatus` (`Enums.Review`):** `PendingByAI = 1`, `Approved`, `Rejected`, `PendingByAdmin`