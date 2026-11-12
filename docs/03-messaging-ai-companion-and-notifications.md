# 🏛️ مستندات معماری داده و دامنه (Enterprise Domain & Data Architecture) - اپلیکیشن Ekip
## 🟣 فاز ۳: موتور پیام‌رسان، چت‌بات هوش مصنوعی، مرکز نوتیفیکیشن، گزارش تخلف، تیکت پشتیبانی و کد تخفیف

---

### بخش اول: موتور پیام‌رسان یکپارچه (`Ekip.Domain.Entities.Messaging`)

#### ۱. مدل اتاق گفتگو (`Conversation`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.Messaging`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای اتاق گفتگو (UUIDv7). |
| `ConversationType` | `ConversationType` | نوع گفتگو (`Direct = 1`, `EkipGroup = 2`). |
| `EkipId` | `Guid?` (FK, Unique) | شناسه اکیپ (مخصوص گپ گروهی اکیپ `EkipGroup`). |
| `ParticipantOneId` | `Guid?` (FK) | شناسه کاربر اول در چت دو نفره `Direct` (جهت جستجوی $O(1)$ و Unique Index). |
| `ParticipantTwoId` | `Guid?` (FK) | شناسه کاربر دوم در چت دو نفره `Direct`. |
| `Theme` | `ChatTheme?` | تم گرافیکی اتاق گفتگو (Value Object ذخیره به صورت `JSONB`). |
| `LastMessageId` | `Guid?` | شناسه آخرین پیام ارسال‌شده در گفتگو. |
| `LastMessagePreview` | `string?` | خلاصه متنی آخرین پیام جهت رندر سریع لیست گفتگوها. |
| `LastMessageAtUtc` | `DateTime?` | زمان ارسال آخرین پیام (UTC). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد گفتگو (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر در گفتگو (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم گفتگو. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم گفتگو (UTC). |

---

#### ۲. مدل اعضای اتاق گفتگو (`ConversationParticipant`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.Messaging`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه رکورد (UUIDv7). |
| `ConversationId` | `Guid` (FK) | متصل به `Conversation.Id`. |
| `UserId` | `Guid` (FK) | کاربر حاضر در گفتگو. |
| `LastReadMessageId` | `Guid?` | آخرین پیام مشاهده‌شده توسط کاربر (تیک خوانده‌شدن). |
| `LastReadAtUtc` | `DateTime?` | زمان آخرین مطالعه گفتگو توسط کاربر (UTC). |
| `UnreadCount` | `int` | شمارنده پیام‌های نخوانده (پیش‌فرض: `0`). |
| `IsMuted` | `bool` | وضعیت بی‌صدا بودن اعلان‌های چت (پیش‌فرض: `false`). |
| `IsActive` | `bool` | وضعیت حضور فعال کاربر در گفتگو (پیش‌فرض: `true`). |
| `CreatedAtUtc` | `DateTime` | زمان ورود به گفتگو (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر وضعیت (UTC). |

---

#### ۳. مدل پیام‌های چت (`ChatMessage`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.Messaging`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه سروری پیام (UUIDv7). |
| `IdempotencyKey` | `Guid` (Unique) | شناسه یکتای کلاینت جهت جلوگیری از ثبت پیام تکراری در تلاش مجدد شبکه. |
| `ConversationId` | `Guid` (FK) | متصل به `Conversation.Id`. |
| `SenderId` | `Guid` (FK) | فرستنده پیام. |
| `ReplyToMessageId` | `Guid?` (FK) | شناسه پیام ریپلای‌شده (اختیاری). |
| `Type` | `ChatMessageType` | نوع پیام (`Text`, `Image`, `Video`, `Voice`, `Audio`, `File`, `Location`, `Poll`, `SharedEkip`, `SystemEvent`). |
| `ContentText` | `string?` | متن پیام یا کپشن رسانه. |
| `Attachment` | `ChatAttachment?` | متادیتای فایل پیوست (Value Object ذخیره به صورت `JSONB`). |
| `Location` | `GeoLocation?` | مختصات مکانی (مخصوص پیام‌های `Location`). |
| `ReferencedEntityId` | `Guid?` | شناسه موجودیت مرجع (برای اشتراک‌گذاری اکیپ `SharedEkip` یا نظرسنجی `Poll`). |
| `IsEdited` | `bool` | وضعیت ویرایش پیام (پیش‌فرض: `false`). |
| `IsPinned` | `bool` | وضعیت پین شدن پیام در گفتگو (پیش‌فرض: `false`). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت پیام (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین ویرایش (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم پیام. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم پیام (UTC). |

---

### بخش دوم: موتور چت‌بات هوش مصنوعی (`Ekip.Domain.Entities.AI`)

#### ۴. مدل نشست گفتگوی هوش مصنوعی (`AIConversation`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.AI`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه نشست مکالمه با هوش مصنوعی (UUIDv7). |
| `UserId` | `Guid` (FK) | کاربر صاحب مکالمه. |
| `Title` | `string?` | عنوان مکالمه. |
| `AIContextMode` | `AIContextMode` | حالت مکالمه (`GeneralCompanion`, `EkipDiscovery`, `EkipCreationAssist`). |
| `TotalTokensUsed` | `int` | مجموع کل توکن‌های مصرف‌شده در این نشست (پیش‌فرض: `0`). |
| `IsMemoryExtracted` | `bool` | وضعیت استخراج آخرین پیام‌ها برای بروزرسانی پرسونای کاربر (پیش‌فرض: `false`). |
| `LastMemoryExtractedAtUtc` | `DateTime?` | زمان آخرین استخراج حافظه جهت پردازش افزایشی پیام‌های جدید (UTC). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد نشست (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تعامل (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم نشست. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم (UTC). |

---

#### ۵. مدل پیام‌های هوش مصنوعی (`AIMessage`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`
* **مسیر:** `Ekip.Domain.Entities.AI`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه پیام (UUIDv7). |
| `IdempotencyKey` | `Guid` | جلوگیری از ارسال پرامپت تکراری و کسر مضاعف سهمیه روزانه. |
| `AIConversationId` | `Guid` (FK) | متصل به `AIConversation.Id`. |
| `SentByUser` | `bool` | `true` در صورت ارسال توسط کاربر، `false` در صورت پاسخ دستیار هوش مصنوعی. |
| `Content` | `string` | متن پیام یا پاسخ هوش مصنوعی. |
| `SuggestedEkipIds` | `IReadOnlyCollection<Guid>` | لیست کپسوله‌شده شناسه‌های اکیپ‌های پیشنهادی در این پیام (`_suggestedEkipIds`). |
| `TokensUsed` | `int` | تعداد توکن مصرف‌شده در این پیام (پیش‌فرض: `0`). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت پیام (UTC). |

---

### بخش سوم: مرکز نوتیفیکیشن جامع (`Ekip.Domain.Entities.Notification`)

#### ۶. مدل نوتیفیکیشن‌ها (`Notification`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`
* **مسیر:** `Ekip.Domain.Entities.Notification`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه نوتیفیکیشن (UUIDv7). |
| `RecipientId` | `Guid` (FK) | کاربر دریافت‌کننده اعلان. |
| `ActorId` | `Guid?` (FK) | کاربر عامل رخداد (اختیاری). |
| `Type` | `NotificationType` | نوع رویداد نوتیفیکیشن (۲۰ رویداد استاندارد چرخه حیات سیستم). |
| `Title` | `string` | عنوان اعلان. |
| `Body` | `string` | متن اعلان. |
| `ReferenceEntityId` | `Guid?` | شناسه موجودیت هدف (اکیپ، درخواست، تیکت و...). |
| `DeepLink` | `string?` | لینک ناوبری مستقیم در اپلیکیشن. |
| `IsRead` | `bool` | وضعیت خوانده‌شدن (پیش‌فرض: `false`). |
| `ReadAtUtc` | `DateTime?` | زمان خوانده‌شدن اعلان (UTC). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد اعلان (UTC). |

---

### بخش چهارم: گزارش تخلف، تیکت پشتیبانی و کد تخفیف

#### ۷. مدل گزارش تخلف (`AbuseReport`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.Moderation`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه گزارش (UUIDv7). |
| `IdempotencyKey` | `Guid` | کلید یکتا جهت جلوگیری از ثبت گزارش تکراری. |
| `ReporterId` | `Guid` (FK) | کاربر گزارش‌دهنده. |
| `TargetType` | `ReportTargetType` | نوع هدف (`UserProfile`, `Ekip`, `ProfileReview`, `ChatMessage`, `NoShowAttendance`). |
| `TargetId` | `Guid` | شناسه رکورد هدف. |
| `Reason` | `ReportReasonCategory` | علت گزارش (`FakeProfile`, `Harassment`, `Scam`, `Spam`, `InappropriateContent`, `NoShowAtEvent`, `Insult`, `SexualContent`). |
| `Description` | `string?` | توضیحات تکمیلی گزارش‌دهنده. |
| `EvidenceMediaAssetIds` | `IReadOnlyCollection<Guid>` | لیست کپسوله‌شده مدارک و اسکرین‌شات‌های ضمیمه (`_evidenceMediaAssetIds`). |
| `Status` | `AbuseReportStatus` | وضعیت بررسی (`Submitted`, `UnderReview`, `ResolvedPenalized`, `Dismissed`). |
| `AdminNote` | `string?` | یادداشت ادمین بررسی‌کننده. |
| `ResolvedByAdminId` | `Guid?` | شناسه ادمین رسیدگی‌کننده. |
| `ResolvedAtUtc` | `DateTime?` | زمان تعیین تکلیف گزارش (UTC). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت گزارش (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر وضعیت (UTC). |

---

#### ۸. مدل تیکت‌های پشتیبانی (`Ticket` & `TicketMessage`)
* **وراثت `Ticket`:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **وراثت `TicketMessage`:** `BaseEntity`, `ICreatedAuditable`
* **مسیر:** `Ekip.Domain.Entities.Support`

| نام پراپرتی در `Ticket` | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه تیکت (UUIDv7). |
| `UserId` | `Guid` (FK) | کاربر ایجادکننده تیکت. |
| `Subject` | `string` | موضوع تیکت. |
| `Category` | `TicketCategory` | دسته‌بندی (`Technical`, `Billing`, `AccountAndVerification`, `FeedbackAndSuggestions`, `Other`). |
| `Priority` | `TicketPriority` | اولویت (`Low`, `Medium`, `High`, `Urgent` - پیش‌فرض: `Low`). |
| `Status` | `TicketStatus` | وضعیت (`Open`, `InProgress`, `AnsweredByAdmin`, `Resolved`, `Closed` - پیش‌فرض: `Open`). |
| `AssignedAdminId` | `Guid?` | کارشناس پشتیبانی مسئول. |
| `TicketMessages` | `IReadOnlyCollection<TicketMessage>` | لیست کپسوله‌شده پیام‌های تیکت (`_ticketMessages`). |
| `ClosedAtUtc` | `DateTime?` | زمان بسته شدن تیکت (UTC). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد تیکت (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین بروزرسانی (UTC). |

---

#### ۹. مدل کدهای تخفیف (`PromoCode`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.Discount`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه کد تخفیف (UUIDv7). |
| `Code` | `string` (Unique) | عبارت کد تخفیف. |
| `DiscountPercent` | `int` | درصد تخفیف (۱ تا ۱۰۰). |
| `MaxDiscountAmount` | `decimal?` | سقف مبلغ تخفیف (اختیاری). |
| `MaxUses` | `int` | حداکثر تعداد دفعات مجاز استفاده. |
| `UsedCount` | `int` | تعداد دفعات استفاده‌شده تا این لحظه (پیش‌فرض: `0`). |
| `TargetUserId` | `Guid?` (FK) | کاربر هدف (در صورت اختصاصی بودن کد تخفیف). |
| `IsActive` | `bool` | وضعیت فعال بودن کد (پیش‌فرض: `true`). |
| `ExpiresAtUtc` | `DateTime` | زمان انقضای کد تخفیف (UTC). |
| `IsCurrentlyValid` | `bool` (Computed) | اعتبارسنجی لحظه‌ای: `IsActive && UsedCount < MaxUses && ExpiresAtUtc > DateTime.UtcNow`. |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد کد (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر (UTC). |

---

### بخش پنجم: ValueObjectهای فاز ۳ (`Ekip.Domain.ValueObjects.Messaging`)

| نام رکورد (Record) | ساختار فیلدها |
| :--- | :--- |
| **`ChatTheme`** | `Guid BackgroundAssetId`, `DateTime SetAtUtc` |
| **`ChatAttachment`** | `Guid AssetId`, `string FileName`, `string MimeType`, `long FileSizeBytes`, `int? DurationSeconds = null`, `int? Width = null`, `int? Height = null` |