# 🏛️ مستندات معماری داده (Enterprise Data Architecture) - اپلیکیشن Ekip
## 🟣 فاز ۳: موتور پیام‌رسان با Client Idempotency، چت‌بات هوش مصنوعی، مرکز نوتیفیکیشن و پشتیبانی (۸ مدل)

---

### بخش اول: موتور پیام‌رسان یکپارچه کاربران و گپِ اکیپ (Real-Time Messaging Engine)
*طراحی‌شده با الگوی Zero-Aggregation Read و کلاینت Idempotency جهت جلوگیری از ارسال پیام‌های تکراری حین قطعی شبکه یا تلاش مجدد (Auto-Retry).*

#### ۱. مدل اتاق گفتگو (`Conversation`)
| نام فیلد (Field) | نوع داده (Type) | اجباری؟ | توضیحات و قوانین معماری |
| :--- | :--- | :---: | :--- |
| `id` | UUID (PK) | بله | شناسه یکتای گفتگو (UUIDv7). |
| `type` | Enum | **بله** | `DIRECT` (چت خصوصی دو نفره)، `EKIP_GROUP` (گپ گروهیِ اعضای اکیپ). |
| `ekip_id` | UUID (FK) | خیر | اتصال یک‌به‌یک با جدول `Ekip` در صورت نوع `EKIP_GROUP`. |
| `last_message_id` | UUID (FK) | خیر | اشاره به آخرین پیام ثبت‌شده. |
| `last_message_preview`| String | خیر | کش متن آخرین پیام جهت رندر سریع O(1) در لیست چت‌ها. |
| `last_message_at` | Timestamp | خیر | زمان آخرین پیام (Indexed جهت مرتب‌سازی لیست - UTC). |
| `created_at` | Timestamp | بله | زمان ایجاد اتاق گفتگو (UTC). |

---

#### ۲. مدل اعضای اتاق گفتگو (`ConversationParticipant`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه رکورد. |
| `conversation_id` | UUID (FK) | متصل به `Conversation`. |
| `user_id` | UUID (FK) | کاربر حاضر در چت (متصل به `UserAccount`). |
| `last_read_message_id`| UUID (FK) | آخرین پیام خوانده‌شده توسط کاربر (تیک خوانده‌شدن). |
| `unread_count` | Integer | شمارنده پیام‌های نخوانده (بدون نیاز به کوئری `COUNT`). |
| `is_muted` | Boolean | وضعیت بی‌صدا بودن اعلان‌های چت. |
| `joined_at` | Timestamp | زمان ورود به چت (UTC). |

---

#### ۳. مدل پیام‌های چت (`ChatMessage`)
*پشتیبانی از `client_message_id` جهت رعایت Idempotency پیام‌ها.*

| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه سروری پیام (UUIDv7). |
| `client_message_id` | String (Unique) | **شناسه یکتای کلاینت (Idempotency):** تولیدشده در React Native پیش از ارسال جهت جلوگیری از درج پیام تکراری حین Retry شبکه. |
| `conversation_id` | UUID (FK) | متصل به `Conversation`. |
| `sender_id` | UUID (FK) | فرستنده پیام (متصل به `UserAccount`). |
| `reply_to_message_id` | UUID (FK) | پیام ریپلای‌شده (Nullable). |
| `message_type` | Enum | `TEXT`, `IMAGE`, `VIDEO`, `VOICE`, `LOCATION`, `SYSTEM_EVENT`. |
| `content_text` | Text | متن پیام. |
| `media_url` | String | آدرس مستقیم رسانه ارسالی. |
| `media_metadata` | JSONB | متادیتای ابعاد، مدت زمان ویس یا حجم فایل. |
| `is_edited` | Boolean | وضعیت ویرایش پیام. |
| `deleted_at` | Timestamp | زمان حذف نرم پیام (UTC). |
| `created_at` | Timestamp | زمان ثبت در سرور (UTC). |

---

### بخش دوم: موتور چت‌بات هوش مصنوعی (AI Companion & Matchmaker Engine)

#### ۴. مدل نشست‌های گفتگوی هوش مصنوعی (`AIConversation`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه نشست مکالمه. |
| `user_id` | UUID (FK) | کاربر متصل به `UserAccount`. |
| `title` | String | عنوان استخراج‌شده مکالمه. |
| `context_mode` | Enum | `GENERAL_COMPANION`, `EKIP_DISCOVERY`, `EKIP_CREATION_ASSIST`. |
| `is_memory_extracted`| Boolean | استخراج داده‌های اپیزودیک به دیتابیس وکتوری. |
| `updated_at` | Timestamp | آخرین زمان تعامل (UTC). |

#### ۵. مدل پیام‌های هوش مصنوعی و کارت‌های تعاملی (`AIMessage`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین معماری |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه پیام. |
| `ai_conversation_id`| UUID (FK) | متصل به `AIConversation`. |
| `role` | Enum | `USER`, `ASSISTANT`. |
| `content` | Text | محتوای پیام متنی. |
| `structured_payload` | JSONB | کارت‌های ساختاریافته اکیپ یا پیش‌نویس فرم جهت رندر گرافیکی در React Native. |
| `tokens_used` | Integer | حجم توکن مصرف‌شده جهت اعمال سهمیه پلن اشتراک. |
| `created_at` | Timestamp | زمان ثبت (UTC). |

---

### بخش سوم: مرکز نوتیفیکیشن جامع (Unified Notification Hub)

#### ۶. مدل نوتیفیکیشن‌ها (`Notification`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه نوتیفیکیشن. |
| `recipient_id` | UUID (FK) | دریافت‌کننده (متصل به `UserAccount`). |
| `actor_id` | UUID (FK) | کاربر مبدا رویداد (Nullable). |
| `type` | Enum | `EKIP_APPLICATION_APPROVED`, `COMPANION_HANDSHAKE_INVITE`, `NEW_EKIP_APPLICANT`, `QUORUM_POLL_STARTED`, `WAITLIST_PROMOTED`, `AI_EKIP_RECOMMENDATION`, `TRENDING_ALERT`, `FRIEND_REQUEST`, `FRIEND_ACCEPTED`, `SUBSCRIPTION_EXPIRING`, `PROMO_CODE_GIFT`, `TICKET_REPLIED`, `REPORT_STATUS_UPDATE`, `ADMIN_BROADCAST`. |
| `title` | String | عنوان اعلان. |
| `body` | Text | متن اعلان. |
| `action_payload` | JSONB | متادیتای ناوبری و Deep-Link به صفحات برنامه. |
| `is_read` | Boolean | وضعیت خوانده‌شدن. |
| `created_at` | Timestamp | زمان ارسال (UTC). |

---

### بخش چهارم: سیستم گزارش تخلفات و تیکت‌های پشتیبانی (Moderation Reports & Support Tickets)

#### ۷. مدل گزارش تخلف کاربران، اکیپ‌ها و غیبت (`AbuseReport`)
| نام فیلد (Field) | نوع داده (Type) | توضیحات و قوانین بیزینس |
| :--- | :--- | :--- |
| `id` | UUID (PK) | شناسه گزارش (UUIDv7). |
| `idempotency_key` | String (Unique) | جلوگیری از ثبت گزارش تکراری در قطعی شبکه. |
| `reporter_id` | UUID (FK) | گزارش‌دهنده (متصل به `UserAccount`). |
| `target_type` | Enum | `USER_PROFILE`, `EKIP`, `PROFILE_REVIEW`, `NO_SHOW_ATTENDANCE`. |
| `target_id` | UUID | شناسه رکورد هدف. |
| `reason_category` | Enum | `FAKE_PROFILE`, `HARASSMENT`, `SCAM_OR_SPAM`, `INAPPROPRIATE_CONTENT`, `NO_SHOW_AT_EVENT`. |
| `description` | Text | توضیحات کاربر. |
| `status` | Enum | `SUBMITTED`, `RESOLVED_PENALIZED` (ثبت رویداد کسر امتیاز در `ReputationLedger`), `DISMISSED`. |
| `admin_note` | Text | یادداشت ادمین بررسی‌کننده. |
| `created_at` | Timestamp | زمان ثبت (UTC). |

---

#### ۸. مدل تیکت‌های پشتیبانی و کدهای تخفیف (`SupportTicket` & `PromoCode`)
* **جدول `SupportTicket`:** شامل `id`, `user_id` (متصل به `UserAccount`), `subject`, `status` (`OPEN`, `ANSWERED`, `CLOSED`), `messages` JSONB و `created_at` (UTC).
* **جدول `PromoCode`:** شامل `id`, `code` (Unique), `discount_percent`, `max_uses`, `expires_at` (UTC), `target_user_id` (متصل به `UserAccount` - اختیاری).