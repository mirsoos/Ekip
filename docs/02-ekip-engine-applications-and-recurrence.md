
# 🏛️ مستندات معماری داده و دامنه (Enterprise Domain & Data Architecture) - اپلیکیشن Ekip
## 🔵 فاز ۲: موتور اکیپ‌سازی، مختصات مکانی (`GeoLocation`)، اکیپ‌های تکرارشونده زنجیره‌ای، پروتکل همراهان، درخواست‌ها و بردار معنایی (`pgvector`)

---

### بخش اول: هسته اکیپ، الگوی تکرار و همراهان میزبان (`Ekip.Domain.Entities.Ekip`)

#### ۱. مدل اصلی اکیپ (`Ekip`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.Ekip`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه یکتای اکیپ (UUIDv7 - تولید خودکار در `BaseEntity`). |
| `IdempotencyKey` | `Guid` | کلید یکتا جهت جلوگیری از ساخت اکیپ تکراری در تلاش مجدد شبکه. |
| `CreatorId` | `Guid` (FK) | سازنده اکیپ (میزبان اصلی - متصل به `UserAccount.Id`). |
| `RecurringEkipId` | `Guid?` (FK) | اشاره به الگوی تکرار مادر در صورت تکرارشونده بودن اکیپ. |
| `Title` | `string` | عنوان اکیپ. |
| `Description` | `string` | توضیحات کامل اکیپ. |
| `Category` | `EkipCategory` | دسته اصلی (`Entertainment`, `Business`, `Education`). |
| `SubCategory` | `string` | زیردسته دقیق فعالیت (اجباری جهت شفافیت موضوع دورهمی). |
| `CityId` | `Guid` (FK) | شهر برگزاری اکیپ (متصل به `City.Id`). |
| `SponsoredPlaceId` | `Guid?` | شناسه مکان اسپانسرشده (نال‌پذیر و بدون FK در MVP؛ رزروشده برای اتصال به فیچر V2). |
| `ExactLocationPoint` | `GeoLocation` | مختصات دقیق جغرافیایی (در صورت `IsDetailsPrivate = true` فقط به اعضای تاییدشده نمایش داده می‌شود). |
| `PublicMapPoint` | `GeoLocation` | مختصات نمایش عمومی روی نقشه (با Jitter تصادفی ۳۰۰ تا ۵۰۰ متر برای اکیپ‌های خصوصی). |
| `LocationName` | `string` | نام مکان برگزاری (اجباری). |
| `AddressDetail` | `string` | آدرس متنی دقیق محل قرار (اجباری). |
| `IsDetailsPrivate` | `bool` | مخفی ماندن جزئیات مکان دقیق تا زمان تایید عضویت (پیش‌فرض: `false`). |
| `HostComposition` | `EkipComposition` | ترکیب میزبان (`Solo`, `Couple`, `Group`). |
| `MinMembers` | `int` | حداقل نفرات لازم جهت به حد نصاب رسیدن اکیپ. |
| `MaxMembers` | `int?` | حداکثر ظرفیت کل اکیپ. |
| `ApprovedMembersCount` | `int` | تعداد اعضای قطعی فعلی (پیش‌فرض: `1`). |
| `EstimatedCost` | `decimal?` | برآورد هزینه نفری. |
| `EventDateTimeUtc` | `DateTime` | تاریخ و ساعت دقیق برگزاری رویداد (UTC). |
| `JoinDeadlineUtc` | `DateTime` | مهلت پایانی عضوگیری (UTC). |
| `JoinCriteria` | `EkipJoinCriteria` | شروط ورود متقاضیان (کپسوله‌شده در Value Object). |
| `ViewsCount` | `int` | شمارنده تعداد بازدیدهای اکیپ (پیش‌فرض: `0`). |
| `ModerationStatus` | `EkipModerationStatus` | وضعیت بررسی محتوا (`PendingByAI`, `Approved`, `Rejected`, `EscalatedToAdmin`). |
| `LifecycleStatus` | `EkipLifecycleStatus` | وضعیت چرخه حیات (`WaitingHostCompanions`, `Published`, `Full`, `Completed`, `Canceled`). |
| `CanceledAtUtc` | `DateTime?` | زمان لغو شدن اکیپ (UTC). |
| `CancellationReason` | `string?` | علت لغو اکیپ. |
| `QuorumStatus` | `QuorumStatus` | وضعیت حد نصاب در ددلاین (`None`, `PendingPoll`, `ConfirmedToProceed`, `AbortedByVote`). |
| `QuorumDecisionDeadlineUtc` | `DateTime?` | مهلت تصمیم‌گیری اعضا برای ادامه اکیپ زیر حد نصاب (UTC). |
| `Tags` | `IReadOnlyCollection<string>` | لیست کپسوله‌شده تگ‌های اکیپ (`_tags`). |
| `MediaAssetIds` | `IReadOnlyCollection<Guid>` | لیست کپسوله‌شده شناسه‌های تصاویر مکان در `MediaAsset` (`_mediaAssetIds`). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت اکیپ (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین ویرایش (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم (UTC). |

---

#### ۲. مدل اکیپ‌های تکرارشونده زنجیره‌ای (`RecurringEkip`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.Ekip`
* **معماری:** تولید زنجیره‌ای و Event-Driven نوبت بعدی به محض اتمام (`Completed`) نوبت فعلی با کپی از روی اکیپ مادر (`SourceEkipId`).

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه الگوی تکرار (UUIDv7). |
| `CreatorId` | `Guid` (FK) | سازنده الگوی تکرار. |
| `SourceEkipId` | `Guid` (FK) | شناسه اکیپ مادر (الگوی مرجع جهت ساخت سری‌های بعدی). |
| `Frequency` | `RecurrenceFrequency` | دوره تکرار (`Daily`, `Weekly`, `BiWeekly`, `Monthly`). |
| `JoinDeadlineOffsetHours` | `int` | فاصله زمانی مهلت عضوگیری تا زمان شروع هر سری (به ساعت). |
| `OccurrencesCount` | `int` | تعداد دفعات تولیدشده تا این لحظه (پیش‌فرض: `1`). |
| `MaxOccurrences` | `int?` | سقف مجاز تعداد تکرار (اختیاری). |
| `LastEventDateTimeUtc` | `DateTime` | زمان برگزاری آخرین اکیپ تولیدشده در این زنجیره (UTC). |
| `EndsAtUtc` | `DateTime?` | تاریخ پایان اعتبار الگوی تکرار (UTC). |
| `IsActive` | `bool` | وضعیت فعال بودن تولید خودکار سری بعدی (پیش‌فرض: `true`). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد الگو (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین بروزرسانی (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم (UTC). |

---

#### ۳. مدل همراهان میزبان (`EkipHostCompanion`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `ICreatedAuditable`
* **مسیر:** `Ekip.Domain.Entities.Ekip`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه رکورد (UUIDv7). |
| `EkipId` | `Guid` (FK) | شناسه اکیپ مربوطه. |
| `CompanionUserId` | `Guid` (FK) | کاربر دعوت‌شده به عنوان پارتنر/همراه میزبان. |
| `CompanionStatus` | `HostCompanionStatus` | وضعیت پاسخ (`Pending`, `Accepted`, `Declined`, `ReportedAsUnwanted`). |
| `CreatedAtUtc` | `DateTime` | زمان ارسال دعوت (UTC). |
| `RespondedAtUtc` | `DateTime?` | زمان ثبت پاسخ توسط همراه (UTC). |

---

#### ۴. مدل اعضای قطعی اکیپ و حضور و غیاب مکانی (`EkipMember`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`
* **مسیر:** `Ekip.Domain.Entities.Ekip`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه عضویت (UUIDv7). |
| `EkipId` | `Guid` (FK) | شناسه اکیپ. |
| `UserId` | `Guid` (FK) | کاربر عضو. |
| `Role` | `EkipRole` | نقش در اکیپ (`Host`, `HostCompanion`, `Participant`). |
| `ApplicationId` | `Guid?` (FK) | شناسه درخواست ورود مبدا (برای میزبان و همراهانش `null` است). |
| `MembershipStatus` | `EkipMembershipStatus` | وضعیت عضویت (`Active`, `Left`, `Kicked`). |
| `AttendanceStatus` | `AttendanceStatus` | وضعیت حضور فیزیکی (`Registered`, `CheckedIn`, `NoShow`). |
| `CheckInAtUtc` | `DateTime?` | زمان ثبت حضور از طریق GPS (UTC). |
| `CheckInLocation` | `GeoLocation?` | مختصات کاربر در لحظه ثبت حضور. |
| `NoShowReportedCount` | `int` | تعداد گزارش‌های غیبت ثبت‌شده توسط سایر هم‌اکیپی‌ها (پیش‌فرض: `0`). |
| `CreatedAtUtc` | `DateTime` | زمان عضویت قطعی در اکیپ (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر وضعیت (UTC). |

---

### بخش دوم: موتور درخواست عضویت و همراهان متقاضی (`Ekip.Domain.Entities.EkipApplication`)

#### ۵. مدل درخواست ورود به اکیپ (`EkipApplication`)
* **وراثت:** `BaseEntity`, `IAggregateRoot`, `IAuditableEntity`, `ISoftDeletable`
* **مسیر:** `Ekip.Domain.Entities.EkipApplication`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه درخواست (UUIDv7). |
| `IdempotencyKey` | `Guid` | کلید یکتا جهت جلوگیری از ارسال درخواست تکراری. |
| `EkipId` | `Guid` (FK) | شناسه اکیپ هدف. |
| `ApplicantId` | `Guid` (FK) | کاربر ثبت‌کننده درخواست. |
| `JoinType` | `EkipComposition` | ترکیب ورود (`Solo`, `Couple`, `Group`). |
| `IntroNote` | `string` | یادداشت معرفی متقاضی برای میزبان. |
| `ApplicationStatus` | `EkipApplicationStatus` | وضعیت درخواست (`WaitingCompanionApproval`, `PendingHostApproval`, `Waitlisted`, `Approved`, `Rejected`, `Canceled`, `InvalidatedByCompanion`). |
| `Companions` | `IReadOnlyCollection<ApplicationCompanion>` | لیست کپسوله‌شده همراهانِ این درخواست (`_companions`). |
| `PartySize` | `int` (Computed) | تعداد کل نفرات این درخواست (`_companions.Count + 1`). |
| `CreatedAtUtc` | `DateTime` | زمان ثبت درخواست (مبنای نوبت در صف انتظار FIFO - UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین تغییر وضعیت (UTC). |
| `IsDeleted` | `bool` | فلگ حذف نرم. |
| `DeletedAtUtc` | `DateTime?` | زمان حذف نرم (UTC). |

---

#### ۶. مدل همراهانِ درخواست (`ApplicationCompanion`)
* **وراثت:** `BaseEntity`, `ICreatedAuditable` *(Child Entity زیرمجموعه `EkipApplication`)*
* **مسیر:** `Ekip.Domain.Entities.EkipApplication`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `Id` | `Guid` (PK) | شناسه رکورد (UUIDv7). |
| `EkipApplicationId` | `Guid` (FK) | متصل به `EkipApplication.Id`. |
| `CompanionUserId` | `Guid` (FK) | کاربر معرفی‌شده به عنوان همراه متقاضی. |
| `Status` | `ApplicationCompanionStatus` | وضعیت تایید همراه (`Pending`, `Accepted`, `Declined`, `ReportedAsUnwanted`). |
| `CreatedAtUtc` | `DateTime` | زمان ایجاد دعوت (UTC). |
| `RespondedAtUtc` | `DateTime?` | زمان پاسخگویی همراه (UTC). |

---

### بخش سوم: بردار معنایی اکیپ در PostgreSQL `pgvector` (`Ekip.Domain.Entities.AI`)

#### ۷. مدل بردار معنایی اکیپ (`EkipSemanticVector`)
* **وراثت:** `Entity`, `IAggregateRoot`, `IAuditableEntity` *(رابطه ۱ به ۱ با `Ekip.Id` بدون تکرار فیلدهای اضافی)*
* **مسیر:** `Ekip.Domain.Entities.AI`

| نام پراپرتی (Property) | نوع داده در C# (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `EkipId` | `Guid` (PK, FK) | شناسه یکتای اکیپ (متصل به `Ekip.Id`). |
| `ContentVector` | `float[]` | بردار معنایی ۱۵۳۶ بعدی (`vector(1536)`) حاصل از عنوان، توضیحات، زیردسته و تگ‌های اکیپ. |
| `CityId` | `Guid` | شناسه شهر جهت Pre-filtering سریع در Partial Index. |
| `IsAvailable` | `bool` | وضعیت باز و قابل عضویت بودن اکیپ جهت فیلتر سریع در جستجوی برداری (پیش‌فرض: `true`). |
| `CreatedAtUtc` | `DateTime` | زمان تولید بردار (UTC). |
| `UpdatedAtUtc` | `DateTime?` | زمان آخرین بروزرسانی بردار یا وضعیت (UTC). |

---

### بخش چهارم: کاتالوگ ValueObjectها (`sealed record`) و Enumهای فاز ۲

#### الف) ValueObjectهای فاز ۲ (`Ekip.Domain.ValueObjects.Ekip`)

| نام رکورد (Record) | ساختار فیلدها و متدهای دامنه |
| :--- | :--- |
| **`GeoLocation`** | `double Latitude`, `double Longitude` + متدهای `CreateJitteredPoint(min, max)` و `DistanceInMetersTo(other)` (فرمول Haversine). |
| **`EkipJoinCriteria`** | `EkipTargetGender TargetGender`, `IReadOnlyCollection<EkipComposition> AllowedJoinTypes`, `int? MinAge`, `int? MaxAge`, `int? MinLevel`, `decimal? MinPublicScore`, `int? MinHonor`, `bool PremiumOnly`, `bool IsAutoAccept`. |

#### ب) Enumهای فاز ۲ (`Ekip.Domain.Enums`)

* **`EkipCategory` (`Enums.Ekip`):** `Entertainment = 1`, `Business`, `Education`
* **`EkipComposition` (`Enums.Ekip`):** `Solo = 1`, `Couple`, `Group`
* **`EkipTargetGender` (`Enums.Ekip`):** `Any = 1`, `MaleOnly`, `FemaleOnly`
* **`EkipModerationStatus` (`Enums.Ekip`):** `PendingByAI = 1`, `Approved`, `Rejected`, `EscalatedToAdmin`
* **`EkipLifecycleStatus` (`Enums.Ekip`):** `WaitingHostCompanions = 1`, `Published`, `Full`, `Completed`, `Canceled`
* **`QuorumStatus` (`Enums.Ekip`):** `None = 1`, `PendingPoll`, `ConfirmedToProceed`, `AbortedByVote`
* **`RecurrenceFrequency` (`Enums.Ekip`):** `Daily = 1`, `Weekly`, `BiWeekly`, `Monthly`
* **`HostCompanionStatus` (`Enums.Ekip`):** `Pending = 1`, `Accepted`, `Declined`, `ReportedAsUnwanted`
* **`EkipRole` (`Enums.Ekip`):** `Host = 1`, `HostCompanion`, `Participant`
* **`EkipMembershipStatus` (`Enums.Ekip`):** `Active = 1`, `Left`, `Kicked`
* **`AttendanceStatus` (`Enums.Ekip`):** `Registered = 1`, `CheckedIn`, `NoShow`
* **`EkipApplicationStatus` (`Enums.EkipApplication`):** `WaitingCompanionApproval = 1`, `PendingHostApproval`, `Waitlisted`, `Approved`, `Rejected`, `Canceled`, `InvalidatedByCompanion`
* **`ApplicationCompanionStatus` (`Enums.EkipApplication`):** `Pending = 1`, `Accepted`, `Declined`, `ReportedAsUnwanted`
