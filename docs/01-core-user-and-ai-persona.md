# 🏛️ مستندات معماری داده (Enterprise Data Architecture) - اپلیکیشن Ekip
## 🟢 فاز ۱: هویت، موتور اشتراک، گیمیفیکیشن مبتنی بر Ledger، امنیت محتوا و معماری Vector DB

---

### بخش اول: هسته هویت و پروفایل (Relational DB - PostgreSQL)

#### ۱. مدل حساب کاربری و امنیت (`UserAccount`)
*نگهداری اطلاعات حساس، احراز هویت‌های رسمی و وضعیت پایه اکانت.*

| نام فیلد (Field) | نوع داده (Type) | اجباری؟ | توضیحات و قوانین معماری |
| :--- | :--- | :---: | :--- |
| `id` | UUID | بله | شناسه یکتای کاربر (Primary Key - UUIDv7). |
| `username` | String (Unique) | بله | نام کاربری یکتا (B-Tree Indexed). |
| `phone_number` | String (Unique) | بله | شماره موبایل (اجباری در ثبت‌نام؛ نمایش به دیگران مشروط به رابطه `ACCEPTED` در گراف دوستی). |
| `email` | String (Unique) | خیر | ایمیل (اختیاری در ثبت‌نام). |
| `password_hash` | String | بله | هش رمز عبور (Argon2id). |
| `oauth_providers` | JSONB | خیر | اتصال به سرویس‌های OAuth (Google, Apple). |
| `two_factor_enabled` | Boolean | بله | وضعیت فعال بودن تایید دو مرحله‌ای (2FA). |
| `is_face_verified` | Boolean | بله | **تیک تایید چهره:** مقایسه تصویر زنده دوربین با آواتار تاییدشده فعلی. |
| `is_national_id_verified`| Boolean | بله | **تیک تایید کد ملی:** استعلام رسمی هویت. |
| `national_id_hash` | String (Unique) | خیر | هشِ کد ملی (HMAC-SHA256) جهت جلوگیری از ساخت اکانت‌های متعدد توسط یک شخص حقیقی. |
| `referral_code` | String (Unique) | بله | کد دعوت اختصاصی کاربر. |
| `referred_by_id` | UUID (FK) | خیر | شناسه معرف (جهت تریگر کردن رویداد اهدای XP در جدول Ledger). |
| `terms_accepted_at` | Timestamp | بله | زمان پذیرش قوانین (UTC). |
| `has_completed_tour` | Boolean | بله | وضعیت مشاهده تور اولیه اپلیکیشن. |
| `account_status` | Enum | بله | `ACTIVE`, `SUSPENDED`, `SHADOW_BANNED`, `BANNED`. |
| `created_at` | Timestamp | بله | زمان ایجاد حساب (UTC). |

---

#### ۲. مدل پروفایل عمومی و حریم خصوصی (`UserProfile`)
*اطلاعات نمایشی و تنظیمات حریم خصوصی. (درصد تکمیل پروفایل در دیتابیس ذخیره نمی‌شود و در لایه Application به صورت Computed Property محاسبه می‌گردد).*

| نام فیلد (Field) | نوع داده (Type) | اجباری در ثبت‌نام؟ | توضیحات و قوانین معماری |
| :--- | :--- | :---: | :--- |
| `user_id` | UUID (PK, FK) | بله | ارتباط ۱ به ۱ با `UserAccount`. |
| `first_name` | String | **بله** | نام. |
| `last_name` | String | **بله** | نام خانوادگی. |
| `gender` | Enum | **بله** | `MALE`, `FEMALE` (غیرقابل تغییر توسط کاربر پس از ثبت‌نام/احراز هویت). |
| `birth_date` | Date | **بله** | تاریخ تولد (مبنای محاسبه سن). |
| `city_id` | UUID (FK) | **خیر** | شهر محل سکونت (**اختیاری در ثبت‌نام**، قابل تکمیل در پروفایل - متصل به `City`). |
| `avatar_asset_id` | UUID (FK) | خیر | اشاره به رکورد تاییدشده در جدول `MediaAsset` (با تغییر عکس، `is_face_verified` ریست می‌شود). |
| `banner_asset_id` | UUID (FK) | خیر | اشاره به بنر تاییدشده در جدول `MediaAsset`. |
| `intro_video_asset_id`| UUID (FK) | خیر | اشاره به ویدیوی معرفی چند ثانیه‌ای در جدول `MediaAsset`. |
| `bio` | Text | خیر | بیوگرافی کوتاه. |
| `job_title` | String | خیر | عنوان شغلی. |
| `personality_type` | String | خیر | تیپ شخصیتی (مثلاً حاصل از تست کوتاه اولیه یا ورود دستی). |
| `interests` | Array[String] | خیر | تگ‌های علاقه‌مندی (GIN Indexed برای کوئری‌های سریع). |
| `social_links` | JSONB | خیر | لینک شبکه‌های اجتماعی (اینستاگرام، فیسبوک و...). |
| `mood_widget` | JSONB | خیر | ویجت مود کاربر (ایموجی، متن، استایل). |
| `theme_id` | String | خیر | شناسه تم پروفایل (**قابل استفاده برای تمام کاربران** بدون محدودیت اشتراک). |
| `is_searchable` | Boolean | بله | **حریم خصوصی سرچ:** اگر `False` باشد، نام کاربری و پروفایل در نتایج جستجو برگردانده نمی‌شود. |
| `show_contacts` | Boolean | بله | **حریم خصوصی دوستان:** مجوز نمایش لیست دوستان (`Contacts`) به دیگران. |
| `show_last_seen` | Boolean | بله | **حریم خصوصی آنلاین بودن:** مجوز نمایش وضعیت آنلاین و آخرین بازدید. |
| `last_seen_at` | Timestamp | خیر | زمان آخرین فعالیت (UTC). وضعیت آنلاین لحظه‌ای در Redis نگهداری شده و هنگام قطع سوکت در این فیلد سینک می‌شود. |
| `text_moderation_status`| Enum | بله | وضعیت سلامت متون پروفایل: `APPROVED`, `PENDING_AI`, `REJECTED_NSFW`. |

---

### بخش دوم: موتور اشتراک چندسطحی و مدیریت سهمیه (Multi-Tier Subscription & Entitlement Engine)

#### ۳. مدل تعریف پلن‌های اشتراک (`SubscriptionPlan`)
*جدول داینامیک برای تعریف انواع اشتراک‌ها بدون نیاز به تغییر کد.*

| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه پلن. |
| `code` | String (Unique) | کد سیستمی پلن (مثلاً: `FREE_TIER`, `PLUS_MONTHLY`, `PRO_VIP`, `BUSINESS_HOST`). |
| `title` | String | نام نمایشی پلن. |
| `price` | Decimal | قیمت پلن. |
| `duration_days` | Integer | مدت اعتبار به روز (مثلاً ۳۰، ۹۰، ۳۶۵). |
| `entitlements` | JSONB | قوانین و سهمیه‌های پلن (سقف اکیپ فعال، پیام AI، امکان ساخت اکیپ تکرارشونده و...). |
| `is_active` | Boolean | وضعیت فعال بودن پلن جهت خرید. |

#### ۴. مدل اشتراک‌های کاربر و مصرف سهمیه (`UserSubscription`)
*نگهداری وضعیت اشتراک فعلی کاربر و ردیابی میزان مصرف سهمیه با رعایت Idempotency.*

| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه اشتراک کاربر. |
| `idempotency_key` | String (Unique) | **کلید یکتا:** جلوگیری از فعال‌سازی مجدد یا تکراری اشتراک در صورت تکرار درخواست (Timeout/Network Retry). |
| `user_id` | UUID (FK) | شناسه کاربر (متصل به `UserAccount`). |
| `plan_id` | UUID (FK) | متصل به `SubscriptionPlan`. |
| `status` | Enum | `ACTIVE`, `EXPIRED`, `CANCELED`, `UPGRADED`. |
| `starts_at` | Timestamp | زمان شروع اشتراک (UTC). |
| `expires_at` | Timestamp | زمان انقضای اشتراک (UTC). |
| `quota_usage` | JSONB | میزان مصرف‌شده از سهمیه دوره فعلی. |

---

### بخش سوم: موتور گیمیفیکیشن، Honor و دفتر کل رویدادها (Event-Sourced Reputation Engine)

#### ۵. مدل دفتر کل رویدادهای اعتبار و تجربه (`ReputationLedger`)
*جدول Append-Only جهت ثبت تاریخچه خام تمامی تراکنش‌های امتیازی و اعتباری با قابلیت باز‌محاسبه.*

| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه تراکنش اعتباری (UUIDv7). |
| `user_id` | UUID (FK) | کاربر هدف (متصل به `UserAccount`). |
| `event_type` | Enum | نوع رویداد: `EKIP_COMPLETED`, `USER_RATED`, `LEFT_EKIP`, `KICKED_FROM_EKIP`, `HOST_CANCELED_LATE`, `NO_SHOW_PENALTY`, `BLOCKED_BY_USER`, `REPORT_CONFIRMED`, `NSFW_VIOLATION`, `UNWANTED_INVITE_SPAM`, `MISSION_COMPLETED`, `REFERRAL_BONUS`. |
| `xp_delta` | Integer | میزان تغییر XP (مثبت یا صفر). |
| `honor_delta` | Float | میزان تاثیر روی Honor مخفی (مثبت یا منفی). |
| `reference_entity_type`| String | موجودیت مرتبط (`EKIP`, `EKIP_MEMBER`, `REVIEW`, `APPLICATION`, `MISSION`, `MEDIA`). |
| `reference_entity_id` | UUID | شناسه رکورد مرجع. |
| `created_at` | Timestamp | زمان وقوع رویداد (UTC). |

#### ۶. مدل خلاصه وضعیت گیمیفیکیشن (`UserGamificationSummary`)
*نمای محاسبه‌شده (Materialized State) از روی Ledger جهت کوئری‌های O(1).*

| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `user_id` | UUID (PK, FK) | شناسه کاربر (متصل به `UserAccount`). |
| `total_xp` | Integer | مجموع XP کسب‌شده. |
| `current_level` | Integer | سطح فعلی کاربر (محاسبه‌شده از روی `total_xp`). |
| `public_score` | Float | میانگین امتیازات ۱ تا ۵ دریافتی از اعضا در اکیپ‌ها. |
| `ratings_count` | Integer | تعداد کل امتیازدهندگان. |
| `hidden_honor` | Float | امتیاز مخفی رفتار کاربر (مبنای فیلتر ورود به اکیپ‌ها و شرط کاربران کاملاً پایه). |
| `aggregated_counters` | JSONB | شمارنده‌های تجمیعی (`left_count`, `kicked_count`, `no_show_count`, `blocked_count`, `reported_count`). |
| `updated_at` | Timestamp | زمان آخرین بروزرسانی (UTC). |

---

### بخش چهارم: امنیت محتوا، ضد فحاشی و گراف ارتباطی (Trust, Safety & Social Graph)

#### ۷. مدل مدیریت رسانه و دروازه ضد محتوای جنسی (`MediaAsset`)
*خط لوله پردازش تمام تصاویر و ویدیوها قبل از نمایش عمومی.*

| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین ضد محتوای جنسی (Anti-NSFW) |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه یکتای فایل. |
| `uploader_id` | UUID (FK) | آپلودکننده (متصل به `UserAccount`). |
| `usage_context` | Enum | `USER_AVATAR`, `USER_BANNER`, `USER_INTRO_VIDEO`, `EKIP_COVER`, `SPONSORED_COVER`. |
| `storage_key` | String | مسیر فایل در Object Storage (S3/MinIO). |
| `nsfw_probability` | Float | نمره احتمال برهنگی/محتوای جنسی (`0.0` تا `1.0`). |
| `moderation_status` | Enum | `PENDING_SCAN`, `APPROVED`, `REJECTED_NSFW`, `ESCALATED_TO_ADMIN`. |
| `scanned_at` | Timestamp | زمان اتمام اسکن (UTC). |
| `created_at` | Timestamp | زمان آپلود (UTC). |

#### ۸. مدل کامنت‌های پروفایل و امتیازدهی (`ProfileReview`)
*ثبت امتیاز فعالیت‌ها همراه با فیلتر ضد فحاشی و Idempotency.*

| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین ضد فحاشی و تکرار |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه نظر. |
| `idempotency_key` | String (Unique) | **کلید یکتا:** جلوگیری از درج نظر تکراری حین قطعی شبکه یا دابل‌کلیک. |
| `target_user_id` | UUID (FK) | صاحب پروفایل (متصل به `UserAccount`). |
| `author_user_id` | UUID (FK) | نویسنده کامنت (متصل به `UserAccount`). **شرط:** حضور کاربر در اکیپ مربوطه باید `CHECKED_IN` باشد. |
| `ekip_id` | UUID (FK) | اکیپ مرتبط (متصل به `Ekip`). |
| `score` | Integer | امتیاز ۱ تا ۵. |
| `comment_text` | Text | متن کامنت. |
| `toxicity_score` | Float | نمره سمّی بودن/فحاشی متن (`0.0` تا `1.0`). |
| `moderation_status` | Enum | `PENDING_AI`, `PUBLISHED`, `REJECTED_PROFANITY`, `DISPUTED`. |
| `created_at` | Timestamp | زمان ثبت (UTC). |

#### ۹. مدل گراف ارتباطی (`UserRelation`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه رابطه. |
| `requester_id` | UUID (FK) | درخواست‌دهنده (متصل به `UserAccount`). |
| `receiver_id` | UUID (FK) | دریافت‌کننده (متصل به `UserAccount`). |
| `status` | Enum | `PENDING`, `ACCEPTED` (اضافه شدن به Contacts و باز شدن دسترسی به شماره تماس), `BLOCKED` (ثبت رویداد منفی در Ledger برای فرد مسدودشده). |
| `created_at` | Timestamp | زمان ایجاد رابطه (UTC). |

---

### بخش پنجم: معماری دیتابیس وکتوری مجزا (Vector DB Schema - Qdrant / Milvus)

#### ۱۰. کالکشن بردار هویت و علایق نیمه‌ایستا (`User_Identity_Vectors`)
| بخش در Vector DB | نام فیلد | نوع داده | توضیحات معماری |
| :--- | :--- | :--- | :--- |
| **Point ID** | `user_id` | UUID | شناسه یکتای کاربر (منطبق با `UserAccount.id`). |
| **Named Vectors** | `interest_bio_vector`| Dense Vector (1536d) | بردار معنایی حاصل از `interests`, `bio`, `personality_type`. |
| **Metadata Payload** | `city_id` | UUID (Indexed) | شناسه شهر جهت فیلتر سریع (بدون نیاز به Re-embedding). |
| **Metadata Payload** | `gender` | Keyword | جنسیت جهت Pre-filtering. |
| **Metadata Payload** | `birth_year` | Integer | سال تولد جهت فیلتر بازه سنی. |
| **Metadata Payload** | `current_level` | Integer | لول کاربر. |
| **Metadata Payload** | `hidden_honor` | Float | امتیاز رفتار کاربر. |

#### ۱۱. کالکشن حافظه اپیزودیک هوش مصنوعی (`User_Episodic_Memory`)
| بخش در Vector DB | نام فیلد | نوع داده | توضیحات معماری |
| :--- | :--- | :--- | :--- |
| **Point ID** | `memory_chunk_id` | UUID | شناسه یکتای تکه خاطره. |
| **Vector** | `memory_embedding` | Dense Vector | بردار معنایی خلاصه تعامل یا مکالمه. |
| **Metadata Payload** | `user_id` | UUID (Indexed) | شناسه کاربر مالک خاطره. |
| **Metadata Payload** | `raw_summary_text` | Text | متن خلاصه خاطره تزریقی به System Prompt. |
| **Metadata Payload** | `memory_type` | Keyword | `CHAT_INSIGHT`, `PAST_EKIP_FEEDBACK`. |
| **Metadata Payload** | `created_at_ts` | Integer (Unix) | زمان ثبت جهت محاسبه ضریب فراموشی زمانی (Time-Decay). |