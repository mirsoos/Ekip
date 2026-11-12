namespace Ekip.Domain.Enums.Notification;

public enum NotificationType
{
    // ۱. رویدادهای چرخه حیات اکیپ و همراهان
    CompanionInviteReceived = 1,      // دعوت به عنوان پارتنر/همراه (توسط میزبان یا متقاضی)
    CompanionInviteResponded,         // پاسخ همراه به دعوت (قبول یا رد کردن)
    NewEkipApplicationReceived,       // ثبت درخواست عضویت جدید (اعلان برای میزبان)
    EkipApplicationApproved,          // تایید شدن درخواست عضویت متقاضی توسط میزبان
    EkipApplicationRejected,          // رد شدن درخواست عضویت
    WaitlistPromoted,                 // باز شدن ظرفیت و ارتقا از صف انتظار به لیست اصلی
    QuorumPollStarted,                // شروع رای‌گیری حد نصاب در زمان ددلاین
    EkipCanceled,                     // لغو شدن اکیپ (اعلان برای اعضای تاییدشده)
    EkipUpcomingReminder,             // یادآوری نزدیک شدن به ساعت شروع اکیپ و ثبت حضور (Check-In)
    PostEkipReviewReminder,           // یادآوری امتیازدهی به هم‌اکیپی‌ها پس از اتمام اکیپ

    // ۲. رویدادهای شبکه اجتماعی و تعاملات پروفایل
    FollowRequestReceived,            // دریافت درخواست دنبال کردن / دوستی
    FollowRequestAccepted,            // قبول شدن درخواست دوستی
    NewProfileReviewReceived,         // ثبت نظر و امتیاز جدید روی پروفایل کاربر

    // ۳. رویدادهای هوش مصنوعی و کشف ترندها
    AiEkipRecommendation,             // پیشنهاد هوشمند اکیپ جدید متناسب با سلیقه کاربر
    CityTrendingAlert,                // داغ شدن یک موضوع یا مکان در شهر کاربر

    // ۴. رویدادهای مالی، اشتراک و پشتیبانی
    SubscriptionExpiringSoon,         // هشدار نزدیک شدن به انقضای اشتراک پریمیوم
    PromoCodeGiftReceived,            // دریافت کد تخفیف هدیه یا اختصاصی
    SupportTicketReplied,             // پاسخ کارشناس پشتیبانی به تیکت کاربر
    AbuseReportStatusUpdated,         // مشخص شدن نتیجه گزارش تخلف ثبت‌شده
    SystemBroadcast                   // اطلاعیه سراسری مدیریت سیستم
}