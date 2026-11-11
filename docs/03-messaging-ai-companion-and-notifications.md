
# 🏛️ مستندات معماری مهندسی دامنه (Enterprise Domain Architecture) - اپلیکیشن Ekip
## 🟣 فاز ۳ (نسخه اصلاح‌شده): موتور پیام‌رسان، چت‌بات هوش مصنوعی، مرکز نوتیفیکیشن، گزارش تخلف، تیکت پشتیبانی و کد تخفیف

---

### بخش اول: موتور پیام‌رسان یکپارچه (`Ekip.Domain.Entities.Messaging`)

#### ۱. مدل اتاق گفتگو (`Conversation`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.Messaging`
* **نوآوری معماری (Zero-Join Direct Chat Lookup):** در چت‌های دو نفره (`Direct`)، برای اینکه بدون `JOIN` زدن روی جدول اعضا بتوانیم در زمان $O(1)$ بفهمیم بین کاربر A و B قبلاً چتی وجود داشته یا نه (و با یک Unique Index در دیتابیس جلوی ساخته شدن دو چت موازی در Race Condition را بگیریم)، شناسه کوچک‌تر در `ParticipantOneId` و شناسه بزرگ‌تر در `ParticipantTwoId` قرار می‌گیرد (`Guid.CompareTo`). در گپ گروهی اکیپ (`EkipGroup`)، این دو فیلد `null` هستند و `EkipId` مقدار دارد.

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای اتاق گفتگو (UUIDv7). |
| `Type` | `ConversationType` | نوع گفتگو (`Direct = 1`, `EkipGroup = 2`). |
| `EkipId` | `Guid?` (FK, Unique) | شناسه اکیپ (فقط برای نوع `EkipGroup`؛ در نوع `Direct` نال است). |
| `ParticipantOneId` | `Guid?` (FK) | شناسه کاربر اول در چت `Direct` (همیشه `Min(UserA, UserB)` جهت Unique Index). |
| `ParticipantTwoId` | `Guid?` (FK) | شناسه کاربر دوم در چت `Direct` (همیشه `Max(UserA, UserB)`). |
| `ThemeId` | `string?` | تم گرافیکی اتاق چت (منتقل‌شده از پروفایل فاز ۱ به چت). |
| `LastMessageId` | `Guid?` | شناسه آخرین پیام ارسال‌شده در این گفتگو. |
| `LastMessagePreview` | `string?` | خلاصه متنی آخرین پیام (حداکثر ۱۰۰ کاراکتر جهت رندر سریع لیست چت‌ها). |
| `LastMessageAtUtc` | `DateTime?` | زمان ارسال آخرین پیام (جهت `ORDER BY` لیست گفتگوها - UTC). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد گفتگو (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر در گفتگو (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم گفتگو. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم گفتگو (UTC). |

---

#### ۲. مدل اعضای اتاق گفتگو و وضعیت مطالعه (`ConversationParticipant`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity` *(مستقل جهت آپدیت سریع تیک خوانده‌شدن و `UnreadCount` بدون قفل کردن کل اتاق گفتگو)*
* **مسیر:** `Ekip.Domain.Entities.Messaging`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه رکورد (UUIDv7). |
| `ConversationId` | `Guid` (FK) | متصل به `Conversation.Id` (دارای Unique Index ترکیبی با `UserId`). |
| `UserId` | `Guid` (FK) | کاربر عضو گفتگو (متصل به `UserAccount.Id`). |
| `LastReadMessageId` | `Guid?` | آخرین پیامی که این کاربر دیده است (مبنای نمایش تیک دوم `Seen`). |
| `LastReadAtUtc` | `DateTime?` | زمان آخرین مطالعه گفتگو توسط این کاربر (UTC). |
| `UnreadCount` | `int` | شمارنده پیام‌های نخوانده این کاربر (پیش‌فرض: `0`). |
| `IsMuted` | `bool` | وضعیت بی‌صدا کردن نوتیفیکیشن‌های این چت (پیش‌فرض: `false`). |
| `IsActive` | `bool` | آیا کاربر هنوز در این چت حضور دارد؟ (اگر از اکیپ لفت دهد یا کیک شود `false` می‌شود تا تاریخچه حفظ گردد ولی دیگر پیام جدید نبیند). |
| `CreatedAtUtc` | `DateTime` | زمان ورود به گفتگو (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر وضعیت (UTC). |

---

#### ۳. مدل پیام‌های چت (`ChatMessage`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.Messaging`
* **اصلاح معماری:** حذف کامل `media_url` خام! به جای آن از Value Object تایپ‌شده `ChatAttachment?` (شامل `AssetId`، سایز، مدت زمان ویس، ابعاد عکس) و برای لوکیشن از `GeoLocation?` استفاده می‌شود. همچنین برای قابلیت «پین کردن پیام در گپ اکیپ»، فیلد `IsPinned` تعبیه شده است.

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه سروری پیام (UUIDv7 - مرتب زمانی). |
| `IdempotencyKey` | `Guid` (Unique) | شناسه یکتای تولیدشده در کلاینت (`ClientMessageId`) جهت جلوگیری از درج پیام تکراری در قطع و وصل اینترنت. |
| `ConversationId` | `Guid` (FK) | متصل به `Conversation.Id`. |
| `SenderId` | `Guid` (FK) | فرستنده پیام (متصل به `UserAccount.Id`). |
| `ReplyToMessageId` | `Guid?` (FK) | شناسه پیامی که به آن ریپلای زده شده است (اختیاری). |
| `Type` | `ChatMessageType` | نوع پیام (`Text`, `Image`, `Video`, `Voice`, `Location`, `SystemEvent`). |
| `ContentText` | `string?` | متن پیام یا کپشن مدیا. |
| `Attachment` | `ChatAttachment?` | مشخصات فایل مدیا (Value Object ذخیره به صورت `JSONB` - بدون URL هاردکد شده). |
| `Location` | `GeoLocation?` | مختصات مکانی (مخصوص پیام‌های از نوع `Location`). |
| `IsEdited` | `bool` | آیا پیام ویرایش شده است؟ (پیش‌فرض: `false`). |
| `IsPinned` | `bool` | آیا پیام در بالای گپ پین شده است؟ (پیش‌فرض: `false`). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت پیام در سرور (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین ویرایش پیام (UTC). |
| `IsDeleted` | `bool` | حذف نرم پیام برای دو طرف. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم پیام (UTC). |

---

### بخش دوم: موتور چت‌بات هوش مصنوعی (`Ekip.Domain.Entities.AI`)

#### ۴. مدل نشست گفتگوی هوش مصنوعی (`AIConversation`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.AI`
* **اصلاح معماری:** حذف فیلد مرده `is_memory_extracted` و افزودن `TotalTokensUsed` جهت مانیتورینگ مصرف کل نشست.

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه نشست مکالمه با AI (UUIDv7). |
| `UserId` | `Guid` (FK) | کاربر صاحب مکالمه. |
| `Title` | `string` | عنوان مکالمه. |
| `ContextMode` | `AIContextMode` | حالت مکالمه (`GeneralCompanion`, `EkipDiscovery`, `EkipCreationAssist`). |
| `TotalTokensUsed` | `int` | مجموع کل توکن‌های مصرف‌شده در این نشست (پیش‌فرض: `0`). |
| `CreatedAtUtc` | `DateTime` | زمان شروع نشست (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین پیام رد و بدل شده (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم تاریخچه چت توسط کاربر. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم (UTC). |

---

#### ۵. مدل پیام‌های هوش مصنوعی (`AIMessage`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable` *(پیام‌های AI ویرایش نمی‌شوند، پس فقط `ICreatedAuditable` است)*
* **مسیر:** `Ekip.Domain.Entities.AI`
* **اصلاح معماری:** جایگزینی `structured_payload` مبهم با کالکشن Strongly-Typed از شناسه‌های اکیپ‌های پیشنهادی (`SuggestedEkipIds` - دقیقاً منطبق بر قرارداد gRPC خط ۱۹۲).

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه پیام (UUIDv7). |
| `IdempotencyKey` | `Guid` | جلوگیری از ارسال پرامپت تکراری و کسر دوباره سهمیه روزانه کاربر در قطعی شبکه. |
| `AIConversationId` | `Guid` (FK) | متصل به `AIConversation.Id`. |
| `SenderRole` | `AIMessageRole` | نقش فرستنده (`User = 1`, `Assistant = 2`). |
| `Content` | `string` | متن پرامپت کاربر یا پاسخ تولیدشده توسط هوش مصنوعی. |
| `SuggestedEkipIds` | `IReadOnlyCollection<Guid>` | لیست کپسوله‌شده شناسه‌های اکیپ‌های پیشنهادشده توسط AI در این پیام (`_suggestedEkipIds`). |
| `TokensUsed` | `int` | تعداد توکن مصرفی این پیام (پیش‌فرض: `0`). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت پیام (UTC). |

---

### بخش سوم: مرکز نوتیفیکیشن جامع (`Ekip.Domain.Entities.Notification`)

#### ۶. مدل نوتیفیکیشن‌ها (`Notification`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`
* **مسیر:** `Ekip.Domain.Entities.Notification`
* **اصلاح معماری:** به جای `action_payload: jsonb` بی‌ساختار، داشتن `ReferenceEntityId` (`Guid?`) و `DeepLinkUrl` (`string?` - مثلاً `ekip://ekips/{id}`) همراه با `ReadAtUtc`.

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه نوتیفیکیشن (UUIDv7). |
| `RecipientId` | `Guid` (FK) | کاربر دریافت‌کننده اعلان. |
| `ActorId` | `Guid?` (FK) | کاربر عامل ایجاد رویداد (مثلاً کسی که درخواست دوستی داده - اختیاری). |
| `Type` | `NotificationType` | نوع نوتیفیکیشن (از بین ۱۴ رویداد استاندارد سیستم). |
| `Title` | `string` | عنوان اعلان. |
| `Body` | `string` | متن اعلان. |
| `ReferenceEntityId` | `Guid?` | شناسه موجودیت هدف (مثلاً شناسه اکیپ، درخواست، یا تیکت). |
| `DeepLink` | `string?` | مسیر ناوبری در کلاینت React Native. |
| `IsRead` | `bool` | وضعیت خوانده‌شدن (پیش‌فرض: `false`). |
| `ReadAtUtc` | `DateTime?` | زمان خوانده‌شدن اعلان (UTC). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد اعلان (UTC). |

---

### بخش چهارم: گزارش تخلف، تیکت پشتیبانی و کد تخفیف (`Moderation, Support & Promo`)

#### ۷. مدل گزارش تخلف کاربران و محتوا (`AbuseReport`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.Moderation`
* **اصلاح معماری:** افزودن `ResolvedByAdminId` و `ResolvedAtUtc` جهت مشخص بودن اینکه کدام ادمین و در چه زمانی گزارش را بسته است.

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه گزارش (UUIDv7). |
| `IdempotencyKey` | `Guid` | جلوگیری از ثبت گزارش تکراری. |
| `ReporterId` | `Guid` (FK) | کاربر گزارش‌دهنده. |
| `TargetType` | `ReportTargetType` | نوع موجودیت هدف (`UserProfile`, `Ekip`, `ProfileReview`, `ChatMessage`, `NoShowAttendance`). |
| `TargetId` | `Guid` | شناسه رکورد هدف. |
| `Reason` | `ReportReasonCategory` | علت گزارش (`FakeProfile`, `Harassment`, `ScamOrSpam`, `InappropriateContent`, `NoShowAtEvent`). |
| `Description` | `string?` | توضیحات تکمیلی گزارش‌دهنده. |
| `EvidenceMediaAssetIds` | `IReadOnlyCollection<Guid>` | اسکرین‌شات یا مدارک ضمیمه‌شده توسط گزارش‌دهنده (`_evidenceMediaAssetIds`). |
| `Status` | `AbuseReportStatus` | وضعیت بررسی (`Submitted`, `UnderReview`, `ResolvedPenalized`, `Dismissed`). |
| `AdminNote` | `string?` | یادداشت ادمین بررسی‌کننده. |
| `ResolvedByAdminId` | `Guid?` | شناسه ادمینی که گزارش را تعیین تکلیف کرده است. |
| `ResolvedAtUtc` | `DateTime?` | زمان بسته شدن گزارش (UTC). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت گزارش (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر وضعیت (UTC). |

---

#### ۸. مدل تیکت‌های پشتیبانی (`SupportTicket` & `SupportTicketMessage`)
* **وراثت `SupportTicket`:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **وراثت `SupportTicketMessage`:** `BaseEntity`, `ICreatedAuditable` *(Child Entity کپسوله‌شده درون `SupportTicket` به جای JSONB کثیف!)*
* **مسیر:** `Ekip.Domain.Entities.Support`

**الف) جدول مادر `SupportTicket`:**
| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه تیکت (UUIDv7). |
| `UserId` | `Guid` (FK) | کاربر ایجادکننده تیکت. |
| `Subject` | `string` | موضوع تیکت. |
| `Category` | `TicketCategory` | دسته‌بندی مشکل (`Technical`, `Billing`, `AccountAndVerification`, `Other`). |
| `Priority` | `TicketPriority` | اولویت تیکت (`Low`, `Normal`, `High`, `Urgent`). |
| `Status` | `SupportTicketStatus` | وضعیت تیکت (`Open`, `PendingAdminReply`, `Answered`, `Closed`). |
| `AssignedAdminId` | `Guid?` | کارشناس پشتیبانی مسئول تیکت. |
| `Messages` | `IReadOnlyCollection<SupportTicketMessage>` | لیست کپسوله‌شده پیام‌های تیکت (`_messages`). |
| `ClosedAtUtc` | `DateTime?` | زمان بسته شدن تیکت (UTC). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت تیکت (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین پیام یا تغییر وضعیت (UTC). |

**ب) موجودیت فرزند `SupportTicketMessage`:**
| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه پیام تیکت (UUIDv7). |
| `SupportTicketId` | `Guid` (FK) | متصل به `SupportTicket.Id`. |
| `SenderId` | `Guid` | شناسه فرستنده (کاربر یا ادمین). |
| `IsFromAdmin` | `bool` | آیا پیام از سمت کارشناس پشتیبانی ارسال شده است؟ |
| `MessageText` | `string` | متن پیام. |
| `AttachmentAssetIds` | `IReadOnlyCollection<Guid>` | فایل‌های ضمیمه پیام (`_attachmentAssetIds`). |
| `CreatedAtUtc` | `DateTime` | زمان ارسال پیام (UTC). |

---

#### ۹. مدل کدهای تخفیف (`PromoCode`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.Subscription` (یا `Billing`)
* **اصلاح معماری:** افزودن `UsedCount` (جهت مقایسه با `MaxUses`)، `MaxDiscountAmount` (سقف مبلغ تخفیف)، و `IsActive`.

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه کد تخفیف (UUIDv7). |
| `Code` | `string` (Unique) | عبارت کد تخفیف (نرمالایزشده با حروف بزرگ، مثلاً `NOROOZ1405`). |
| `DiscountPercent` | `int` | درصد تخفیف (بین `1` تا `100`). |
| `MaxDiscountAmount` | `decimal?` | سقف مبلغ تخفیف (اختیاری - مثلاً ۲۰٪ تخفیف تا سقف ۱۰۰ هزار تومان). |
| `MaxUses` | `int` | حداکثر تعداد دفعات مجاز استفاده کل. |
| `UsedCount` | `int` | تعداد دفعات استفاده‌شده تا این لحظه (پیش‌فرض: `0`). |
| `TargetUserId` | `Guid?` (FK) | در صورتی که کد تخفیف اختصاصی برای یک کاربر خاص صادر شده باشد. |
| `ExpiresAtUtc` | `DateTime` | تاریخ انقضای کد تخفیف (UTC). |
| `IsActive` | `bool` | وضعیت فعال بودن کد (پیش‌فرض: `true`). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد کد (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین استفاده یا ویرایش (UTC). |

---

### بخش پنجم: ValueObjectهای فاز ۳ (`Ekip.Domain.ValueObjects.Messaging`)

| نام رکورد (Record) | ساختار فیلدها (Properties & Types) |
| :--- | :--- |
| **`ChatAttachment`** | `Guid AssetId`, `string FileName`, `string MimeType`, `long FileSizeBytes`, `int? DurationSeconds = null`, `int? Width = null`, `int? Height = null` |
