# 🏛️ مستندات معماری داده (Enterprise Data Architecture) - اپلیکیشن Ekip
## 🔴 فاز ۵: پنل ادمین ایزوله، موتور کشف ترندها، بوکمارک و تنظیمات پویا (۷ مدل)

---

### بخش اول: زیرساخت ایزوله پنل مدیریت (Isolated Back-Office & Admin Security)
*طبق معماری Zero-Trust، حساب کاربری ادمین‌ها، نشست‌ها (Sessions)، کلید رمزنگاری توکن‌ها (`ADMIN_JWT_SECRET`) و مسیر APIهای ادمین کاملاً از کاربران عادی (`UserAccount`) جداست تا امکان ارتقای سطح دسترسی غیرمجاز (Privilege Escalation) ناممکن شود.*

#### ۱. مدل حساب کاربری مدیران (`AdminAccount`)
| نام فیلد (Field) | نوع داده (Type) | اجباری؟ | توضیحات و قوانین امنیتی |
| :--- | :--- | :---: | :--- |
| `id` | UUID (PK) | بله | شناسه یکتای ادمین (UUIDv7). |
| `username` | String (Unique) | بله | نام کاربری جهت ورود به پنل مدیریت. |
| `email` | String (Unique) | بله | ایمیل سازمانی ادمین. |
| `password_hash` | String | بله | هش رمز عبور (Argon2id با پارامترهای سنگین سازمانی). |
| `role` | Enum | **بله** | `SUPER_ADMIN`, `MODERATOR`, `SUPPORT_AGENT`, `COMMERCIAL_MANAGER`. |
| `permissions` | Array[String] | خیر | دسترسی‌های ریزدانه (Granular Permissions). |
| `totp_secret` | String | بله | کلید Google Authenticator (الزام 2FA برای تمام مدیران). |
| `allowed_ips` | Array[String] | خیر | لیست سفید IPهای مجاز ورود (IP Whitelisting). |
| `is_active` | Boolean | بله | وضعیت فعال بودن حساب ادمین. |
| `created_at` | Timestamp | بله | زمان ایجاد حساب (UTC). |

---

#### ۲. مدل نشست‌ها و توکن‌های اختصاصی ادمین (`AdminSession`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین امنیتی |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه نشست ادمین. |
| `admin_id` | UUID (FK) | متصل به `AdminAccount`. |
| `refresh_token_hash` | String (Unique) | هش توکن نشست ادمین (دارای عمر کوتاه ۸ ساعته). |
| `ip_address` | String | آی‌پی متصل‌شده جهت اعتبارسنجی مداوم. |
| `user_agent` | String | مشخصات مرورگر و سیستم‌عامل ادمین. |
| `expires_at` | Timestamp | زمان انقضای نشست (UTC). |
| `revoked_at` | Timestamp | زمان ابطال دستی نشست (UTC). |

---

#### ۳. مدل لاگ امنیتی اقدامات ادمین (`AdminAuditLog`)
*جدول Append-Only جهت ثبت تاریخچه کلیه تغییرات ایجادشده توسط ادمین‌ها در سیستم.*

| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه لاگ (UUIDv7). |
| `admin_id` | UUID (FK) | ادمین انجام‌دهنده عملیات (متصل به `AdminAccount`). |
| `action_type` | String | نوع عملیات (`BAN_USER`, `APPROVE_MEDIA`, `RESOLVE_REPORT`, `UPDATE_SETTING`). |
| `target_entity` | String | جدول هدف (`UserAccount`, `Ekip`, `MediaAsset`, `SystemSetting`). |
| `target_id` | UUID | شناسه رکورد هدف. |
| `changes_diff` | JSONB | مقادیر قدیم و جدید (`{"old": ..., "new": ...}`). |
| `ip_address` | String | آی‌پی ادمین در لحظه انجام عملیات. |
| `created_at` | Timestamp | زمان دقیق رخداد (UTC). |

---

### بخش دوم: تعاملات تکمیلی اکیپ و موتور کشف ترندها (Bookmarks & Trend Engine)

#### ۴. مدل اکیپ‌های نشان‌شده / بوکمارک (`SavedEkip`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه رکورد. |
| `user_id` | UUID (FK) | کاربر ذخیره‌کننده (متصل به `UserAccount`). |
| `ekip_id` | UUID (FK) | اکیپ ذخیره‌شده (متصل به `Ekip`). *(دارای Unique Composite Index روی `(user_id, ekip_id)` جهت جلوگیری از ثبت تکراری)*. |
| `created_at` | Timestamp | زمان ذخیره اکیپ (UTC). |

---

#### ۵. مدل لاگ جستجوهای کاربران (`SearchQueryLog`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه لاگ جستجو (UUIDv7). |
| `user_id` | UUID (FK) | کاربر جستجوکننده (متصل به `UserAccount` - اختیاری). |
| `city_id` | UUID (FK) | شهر محل جستجو (متصل به `City`). |
| `normalized_query` | String (Indexed)| عبارت نرمالایزشده جستجو. |
| `applied_filters` | JSONB | فیلترهای اعمال‌شده روی فید یا نقشه. |
| `results_count` | Integer | تعداد نتایج بازگردانده‌شده. |
| `created_at` | Timestamp | زمان انجام جستجو (UTC). |

---

#### ۶. مدل ترندهای محاسبه‌شده هر شهر (`CityTrendSnapshot`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه ترند. |
| `city_id` | UUID (FK) | شهر هدف (متصل به `City`). |
| `trend_type` | Enum | `TAG_OR_TOPIC`, `LOCATION_NAME`, `SUB_CATEGORY`. |
| `keyword` | String | کلیدواژه یا مکان ترندشده. |
| `trend_score` | Float | امتیاز محاسبه‌شده نرخ رشد در ۲۴ ساعت گذشته. |
| `active_ekips_count` | Integer | تعداد اکیپ‌های باز فعال مرتبط با ترند. |
| `calculated_at` | Timestamp | زمان محاسبه ترند (UTC). |

---

### بخش سوم: تنظیمات پویا و فلگ‌های سیستم (`SystemSetting`)

#### ۷. مدل تنظیمات سراسری و پارامترهای سیستم (`SystemSetting`)
*نگهداری پارامترهای عملیاتی، Feature Flagها و ضرایب Extensionهای محاسباتی Honor/XP.*

| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `key` | String (PK) | کلید یکتا (مانند `app.force_update_version`, `feature.face_verification_enabled`, `reputation.extension_params`). |
| `value` | JSONB | مقادیر ساختاریافته تنظیمات. |
| `description` | String | شرح عملکرد تنظیم در پنل مدیریت. |
| `updated_by_admin_id` | UUID (FK) | متصل به `AdminAccount`. |
| `updated_at` | Timestamp | زمان آخرین بروزرسانی (UTC). |