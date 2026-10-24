# 🏛️ مستندات معماری داده (Enterprise Data Architecture) - اپلیکیشن Ekip
## 🔵 فاز ۲: موتور اکیپ‌سازی، جستجوی نقشه (PostGIS)، اکیپ‌های تکرارشونده، پروتکل کاپل/گروهی، صف انتظار و حضور و غیاب (۸ مدل)

---

### بخش اول: هسته اکیپ، نقشه و موتور تکرار (Relational DB - PostgreSQL + PostGIS)

#### ۱. مدل اصلی اکیپ (`Ekip`)
*جدول اصلی، مسطح (Flat) و خودکفا جهت اجرای کوئری‌های فوق‌سریع بدون Join اضافه با پشتیبانی از Idempotency، لغو، و تصمیم‌گیری حد نصاب.*

| نام فیلد (Field) | نوع داده (Type) | اجباری؟ | توضیحات و قوانین بیزینس (Business Rules) |
| :--- | :--- | :---: | :--- |
| `id` | UUID (PK) | بله | شناسه یکتای اکیپ (UUIDv7). |
| `idempotency_key` | String (Unique) | بله | **کلید یکتا:** جلوگیری از ساخت اکیپ تکراری حین قطعی شبکه یا تلاش مجدد کلاینت (Retry). |
| `creator_id` | UUID (FK) | بله | سازنده اکیپ (میزبان اصلی - متصل به `UserAccount`). |
| `recurring_ekip_id` | UUID (FK) | خیر | در صورت تولید از روی الگوی تکرار، به `RecurringEkip` اشاره می‌کند. |
| `title` | String | **بله** | عنوان اکیپ. |
| `description` | Text | **بله** | توضیحات کامل اکیپ. |
| `category` | Enum | **بله** | دسته اصلی: `ENTERTAINMENT` یا `BUSINESS`. |
| `sub_category` | String | خیر | زیردسته (کافه، سینما، کوهنوردی، بدنسازی و...). |
| `tags` | Array[String] | خیر | تگ‌های انتخابی کاربر (GIN Indexed). |
| `city_id` | UUID (FK) | **بله** | شهر برگزاری اکیپ (متصل به `City`). |
| `sponsored_place_id` | UUID (FK) | خیر | اتصال به مکان تبلیغاتی انتخاب‌شده (`SponsoredPlace`). |
| `exact_location_point`| `GEOGRAPHY(Point, 4326)`| خیر | مختصات دقیق جغرافیایی (در صورت `is_details_private = True`، فقط به اعضای `APPROVED` ارسال می‌شود). |
| `public_map_point` | `GEOGRAPHY(Point, 4326)`| خیر | **دارای Spatial GIST Index:** مختصات نمایش عمومی نقشه (برای اکیپ‌های خصوصی با Jitter تصادفی ۳۰۰ تا ۵۰۰ متر ثبت می‌شود). |
| `location_name` | String | خیر | نام مکان (مثلاً: *"کافه لمیز"*). |
| `address_detail` | Text | خیر | آدرس متنی دقیق محل قرار. |
| `is_details_private` | Boolean | بله | مخفی ماندن جزئیات دقیق و پین اصلی تا زمان عضویت قطعی. |
| `media_asset_ids` | Array[UUID] | خیر | آرایه شناسه‌های عکس مکان در جدول `MediaAsset` (تاییدشده توسط Anti-NSFW). |
| `host_composition` | Enum | **بله** | وضعیت میزبان: `SOLO`, `COUPLE`, `GROUP`. |
| `min_members` | Integer | **بله** | حداقل نفرات لازم برای تشکیل اکیپ. |
| `max_members` | Integer | خیر | حداکثر ظرفیت کل اکیپ. |
| `approved_members_count`| Integer | بله | تعداد اعضای قطعی فعلی (بررسی اتمیک ظرفیت با قانون All-or-Nothing). |
| `estimated_cost` | Integer | خیر | میانگین هزینه نفری. |
| `event_datetime` | Timestamp | **بله** | تاریخ و ساعت دقیق موعود برگزاری (UTC). |
| `join_deadline` | Timestamp | **بله** | مهلت عضوگیری (حداقل ۱۲ و حداکثر ۴۸ ساعت قبل از موعود - UTC). |
| `views_count` | Integer | بله | تعداد بازدیدهای اکیپ. |
| `moderation_status` | Enum | بله | `PENDING_SCAN`, `APPROVED`, `REJECTED_NSFW`, `ESCALATED`. |
| `lifecycle_status` | Enum | بله | `DRAFT`, `WAITING_HOST_COMPANIONS`, `PUBLISHED`, `FULL`, `COMPLETED`, `CANCELED`. |
| `canceled_at` | Timestamp | خیر | زمان لغو شدن اکیپ (UTC). |
| `cancellation_reason` | Text | خیر | دلیل لغو اکیپ. |
| `canceled_by_user_id` | UUID (FK) | خیر | لغوکننده اکیپ (در صورت لغو دیرهنگام توسط میزبان، جریمه در `ReputationLedger` ثبت می‌شود). |
| `quorum_status` | Enum | بله | وضعیت حد نصاب در زمان ددلاین: `NONE`, `PENDING_POLL` (استعلام از اعضا و میزبان), `CONFIRMED_TO_PROCEED`, `ABORTED_BY_VOTE`. |
| `quorum_decision_deadline`| Timestamp | خیر | مهلت پاسخ به استعلام ادامه دادن یا ندادن اکیپ زیر حد نصاب (UTC). |
| `created_at` | Timestamp | بله | زمان ثبت اکیپ (UTC). |

#### فیلدهای شروط ورود متقاضیان (درون جدول `Ekip`):
| نام فیلد شرط | نوع داده | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `target_gender` | Enum | `ANY`, `MALE_ONLY`, `FEMALE_ONLY` (کاربر آقا مجاز به ساخت `FEMALE_ONLY` نیست و بالعکس). |
| `allowed_join_types` | Array[Enum] | ترکیبی از `['SOLO', 'COUPLE', 'GROUP']`. |
| `min_age` & `max_age` | Integer | بازه سنی مجاز. |
| `min_level` | Integer | حداقل لول پروفایل. |
| `min_public_score` | Float | حداقل امتیاز عمومی پروفایل (۱ تا ۵). |
| `min_honor` | Float | حداقل امتیاز Honor مخفی. |
| `premium_only` | Boolean | محدود به کاربران دارای اشتراک پریمیوم. |
| `is_auto_accept` | Boolean | تایید خودکار متقاضیان واجد شرایط بدون دخالت دستی میزبان. |
| `auto_accept_strict_honor`| Boolean | شرط تایید خودکار فقط برای کاربران کاملاً پایه (Honor بالا و کنسلی صفر). |

---

#### ۲. مدل اکیپ‌های تکرارشونده (`RecurringEkip`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه الگوی تکرار. |
| `creator_id` | UUID (FK) | سازنده اکیپ (متصل به `UserAccount`). |
| `rrule_string` | String | رشته استاندارد RFC 5545 تکرار زمانی. |
| `template_payload` | JSONB | قالب اطلاعات پایه، شروط و لوکیشن جهت تولید خودکار نمونه‌ها در `Ekip`. |
| `generated_until_date` | Timestamp | تاریخ انتهای پیش‌تولید نمونه‌ها در جدول `Ekip` (UTC). |
| `is_active` | Boolean | وضعیت فعال بودن تولید خودکار نمونه‌ها. |
| `created_at` | Timestamp | زمان ایجاد الگو (UTC). |

---

#### ۳. مدل همراهان خودِ میزبان (`EkipHostCompanion`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه رکورد. |
| `ekip_id` | UUID (FK) | متصل به `Ekip`. |
| `companion_user_id` | UUID (FK) | پارتنر یا عضو گروه معرفی‌شده توسط میزبان (متصل به `UserAccount`). |
| `status` | Enum | `PENDING`, `ACCEPTED`, `DECLINED`. تا تایید همه، وضعیت اکیپ `WAITING_HOST_COMPANIONS` می‌ماند. |
| `responded_at` | Timestamp | زمان پاسخ همراه (UTC). |

---

### بخش دوم: موتور درخواست عضویت، صف انتظار و حضور و غیاب (Applications, Waitlist & Check-in)

#### ۴. مدل درخواست ورود به اکیپ (`EkipApplication`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه درخواست (UUIDv7). |
| `idempotency_key` | String (Unique) | **کلید یکتا:** جلوگیری از ثبت درخواست تکراری در شرایط قطعی شبکه. |
| `ekip_id` | UUID (FK) | متصل به `Ekip`. |
| `leader_user_id` | UUID (FK) | کاربر ثبت‌کننده درخواست (متصل به `UserAccount`). |
| `join_type` | Enum | `SOLO`, `COUPLE`, `GROUP`. |
| `party_size` | Integer | تعداد نفرات درخواست (قانون All-or-Nothing برای ظرفیت). |
| `intro_note` | Text | پیام معرفی برای میزبان. |
| `status` | Enum | `WAITING_COMPANIONS` (میزبان نمی‌بیند), `PENDING_HOST`, `WAITLISTED` (صف انتظار), `APPROVED`, `REJECTED`, `CANCELED`, `INVALIDATED_BY_COMPANION`. |
| `created_at` | Timestamp | زمان ثبت درخواست (مبنای تقدم در صف FIFO لیست انتظار - UTC). |

---

#### ۵. مدل تاییدیه همراهانِ درخواست (`ApplicationCompanion`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه رکورد. |
| `application_id` | UUID (FK) | متصل به `EkipApplication`. |
| `companion_user_id` | UUID (FK) | پارتنر/هم‌گروهی معرفی‌شده (متصل به `UserAccount`). |
| `status` | Enum | `INVITED`, `ACCEPTED`, `DECLINED`. |
| `flagged_as_unwanted` | Boolean | در صورت زدن گزینه مزاحمت: ابطال کل درخواست + کسر Honor ثبت‌کننده در `ReputationLedger`. |
| `responded_at` | Timestamp | زمان پاسخ همراه (UTC). |

---

#### ۶. مدل اعضای نهایی اکیپ و حضور و غیاب مکانی (`EkipMember`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه عضویت. |
| `ekip_id` | UUID (FK) | متصل به `Ekip`. |
| `user_id` | UUID (FK) | کاربر عضو (متصل به `UserAccount`). |
| `role` | Enum | `HOST`, `HOST_COMPANION`, `MEMBER`. |
| `application_id` | UUID (FK) | متصل به `EkipApplication` مبدا. |
| `membership_status` | Enum | `ACTIVE`, `LEFT` (آزاد شدن ظرفیت و فراخوانی صف انتظار + کسر Honor), `KICKED`. |
| `attendance_status` | Enum | `REGISTERED`, `CHECKED_IN`, `NO_SHOW`. |
| `check_in_at` | Timestamp | زمان ثبت حضور از طریق GPS (UTC). |
| `check_in_location` | `GEOGRAPHY(Point, 4326)` | مختصات GPS گوشی کاربر در لحظه ثبت حضور (باید در شعاع مجاز رویداد باشد). |
| `no_show_reported_count`| Integer | تعداد گزارش‌های ثبت‌شده توسط هم‌اکیپی‌ها مبنی بر غیبت این فرد (کسر سنگین Honor در صورت احراز). |
| `joined_at` | Timestamp | زمان عضویت قطعی (UTC). |

---

### بخش سوم: مکان‌های تبلیغاتی / اسپانسری (`SponsoredPlace`)

#### ۷. مدل مکان‌های تبلیغاتی (`SponsoredPlace`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه مکان. |
| `city_id` | UUID (FK) | شهر نمایش تبلیغ (متصل به `City`). |
| `target_categories` | Array[String] | دسته‌بندی‌های اکیپ مرتبط (کافه، رستوران، ورزش و...). |
| `title` | String | نام مکان/مجموعه. |
| `description` | String | آفر یا توضیحات مکان. |
| `location_point` | `GEOGRAPHY(Point, 4326)`| مختصات دقیق روی نقشه. |
| `address_text` | String | آدرس متنی. |
| `cover_asset_id` | UUID (FK) | متصل به `MediaAsset`. |
| `priority_weight` | Integer | وزن اولویت نمایش در لیست. |
| `expires_at` | Timestamp | انقضای کمپین تبلیغاتی (UTC). |
| `is_active` | Boolean | وضعیت فعال بودن تبلیغ. |

---

### بخش چهارم: دیتابیس وکتوری برای اکیپ‌ها (Vector DB Schema - Qdrant / Milvus)

#### ۸. کالکشن بردارهای معنایی و مکانی اکیپ (`Ekip_Semantic_Vectors`)
| بخش در Vector DB | نام فیلد | نوع داده | توضیحات معماری |
| :--- | :--- | :--- | :--- |
| **Point ID** | `ekip_id` | UUID | شناسه یکتای اکیپ (منطبق با `Ekip.id`). |
| **Vector** | `content_vector` | Dense Vector (1536d) | بردار معنایی حاصل از `title`, `description`, `sub_category`, `tags`. |
| **Metadata Payload** | `city_id` | UUID (Indexed) | فیلتر قطعی شهر. |
| **Metadata Payload** | `geo_point` | GeoPoint (`lat`, `lon`)| ایندکس جغرافیایی جهت سرچ شعاع مکانی توسط هوش مصنوعی. |
| **Metadata Payload** | `category` | Keyword | دسته تفریحی یا بیزینس. |
| **Metadata Payload** | `target_gender` | Keyword | جنسیت مجاز جهت Pre-filtering. |
| **Metadata Payload** | `event_timestamp` | Integer (Unix) | زمان برگزاری رویداد. |
| **Metadata Payload** | `lifecycle_status` | Keyword | وضعیت چرخه حیات (جهت خروج آنی اکیپ‌های پر یا لغو شده از جستجو). |