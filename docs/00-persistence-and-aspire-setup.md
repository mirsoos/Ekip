# ⚙️ مستندات زیرساخت پایگاه‌داده، EF Core و ارکستراسیون .NET Aspire

## ۱. لایه `Ekip.Persistence`
* **`EkipDbContext`:**
  * شامل ۴۱ جدول دامنه (نام‌گذاری مفرد با `snake_case`) + ۳ جدول بومی MassTransit Outbox/Inbox (`inbox_state`, `outbox_state`, `outbox_message`).
  * ثبت اکستنشن‌های PostgreSQL شامل `postgis` و `vector`.
  * کنترل همزمانی خوش‌بینانه (`Optimistic Concurrency`) با Shadow Property متصل به `xmin`.
  * فیلتر سراسری `ISoftDeletable` (`!IsDeleted`) در `OnModelCreating` و متد `PopDomainEvents()` برای استخراج اتمیک رویدادهای دامنه.
  * ثبت به صورت **`AddDbContextPool<EkipDbContext>`** (کانتکست کاملاً Stateless جهت سازگاری با Pooling و Aspire).
* **`AuditableAndSoftDeleteInterceptor`:**
  * تزریق `TimeProvider` جهت ثبت یکپارچه زمان‌های UTC (`DateTimeKind.Utc`).
  * جلوگیری از ویرایش/حذف جداول `IAppendOnlyEntity` در `ChangeTracker`.
  * تبدیل حذف فیزیکی `ISoftDeletable` به `IsDeleted = true` و `DeletedAtUtc` همراه با محافظت از Owned Entityها در برابر پاک‌شدن ناخواسته.
* **`PropertyBuilderExtensions`:**
  * **`HasJsonbConversion<T>()`:** تبدیل ساختارهای پیچیده و `IReadOnlyDictionary` (مانند `PlanEntitlements`) به `jsonb` با `System.Text.Json` و `ValueComparer<T>` اختصاصی.
  * **`HasPgVectorConversion(dimensions)`:** تبدیل `float[]` دامنه به `Pgvector.Vector` با `ValueComparer<float[]>` برای ستون‌های `vector(1536)`.
* **ایندکس‌های یکتای Soft Delete:**
  * در `UserAccount` ایندکس‌های یکتای `username`، `email` و `phone_number` دارای فیلتر `is_deleted = false` هستند؛ اما `national_id_hash` و `referral_code` سراسری یکتا می‌مانند.

---

## ۲. ارکستراسیون `.NET Aspire` (`Ekip.AppHost`)
* **پیش‌نیاز محیط توسعه:** Docker Desktop به‌روز (Linux Containers / WSL 2).
* **موتور دیتابیس:** کانتینر سفارشی `postgres:18.7` با نصب پکیج‌های رسمی `postgresql-18-postgis-3` و `postgresql-18-pgvector` در مسیر `Ekip.AppHost/docker/postgres/Dockerfile` (بدون پسوند فایل).
* **منبع Aspire:** دیتابیس با نام **`"EkipDatabase"`**، ولوم پایدار `ekip-postgres-data` و ابزار وب `pgweb` رجیستر شده است.
* **پیکربندی `Ekip.Api`:** فراخوانی `AddPersistenceServices(builder.Configuration)` و سپس `EnrichNpgsqlDbContext<EkipDbContext>()` (با حفظ Retry، HealthCheck و Tracing).

---

## ۳. قوانین Migration و اجرا
* **تولید Migration جدید (از PMC):**
  ```powershell
  Add-Migration <MigrationName> -Project Ekip.Persistence -StartupProject Ekip.Api