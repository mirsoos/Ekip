# 🏛️ مستندات معماری کاربردی، CQRS و لایه API (Application, Messaging & Contracts)
## 🧱 گام ۱: پل `Ekip.Application` ⇄ `Ekip.Persistence`، قراردادهای خطا و تست‌های معماری

---

### ۱. پکیج‌ها و تصمیمات لایسنسی قفل‌شده

| پروژه | پکیج / ابزار | نسخه | دلیل فنی و لایسنسی |
| :--- | :--- | :--- | :--- |
| `Ekip.Application` | `MediatR` | `12.5.0` | پین‌شده در آخرین نسخه رایگان Apache 2.0 (پرهیز از لایسنس RPL-1.5/تجاری نسخه‌های ۱۳+ و پرهیز از باگ Open-Generic `static abstract` روی `IResult<TSelf>` در سورس‌ژنراتورها). ایزوله‌شده پشت قراردادهای `ICommand`/`IQuery`. |
| `Ekip.Application` | `FluentValidation.DependencyInjectionExtensions` | `12.2.0` | اعتبارسنجی ورودی‌ها تحت Apache 2.0؛ برای `Mappings/` از مپینگ دستی یا `Mapperly` استفاده می‌شود (بدون `AutoMapper`). |
| `Ekip.Application` | `Microsoft.Extensions.Logging.Abstractions` | `10.0.15` | ثبت لاگ ساختاریافته در Pipeline Behaviorها. |
| `Ekip.Persistence` | `Dapper` | `2.1.89` | اجرای کوئری‌های پرسرعت سمت Read. |
| `Ekip.Persistence` | `MassTransit.EntityFrameworkCore` | `8.5.12` | دانگریدشده از `9.2.6` به آخرین سری رایگان Apache 2.0 برای Transactional Outbox. |
| `Ekip.Api` | `Grpc.AspNetCore` | `2.85.0` | سرویس‌های gRPC و Interceptor خطاها. |
| `Ekip.ArchitectureTests` | `xunit.v3` + `TngTech.ArchUnitNET.xUnitV3` | `3.2.2` / `0.13.5` | تست‌های معماری (`<OutputType>Exe</OutputType>`). |

---

### ۲. جداسازی مسیر Write و Read (CQRS Data Access)

* **سمت Write (EF Core + Repositoryهای اختصاصی):**
  * قراردادهای Repository صرفاً روی موجودیت‌های `IAggregateRoot` در `Ekip.Application/Interfaces/` و به‌صورت **فازبه‌فاز (Just-In-Time)** تعریف و در `Ekip.Persistence/Repositories/` پیاده‌سازی می‌شوند.
  * لایه `Ekip.Application` هیچ وابستگی به `Microsoft.EntityFrameworkCore` یا SQL خام ندارد؛ عملیات حساس (`FOR UPDATE` روی `UserWallet`، آپدیت اتمیک `ExecuteUpdateAsync` روی `PromoCode`/`Ekip` و `Include` گراف فرزندها) درون `Ekip.Persistence` کپسوله می‌ماند.
* **سمت Read (Dapper + `NpgsqlDataSource` مشترک):**
  * `ISqlConnectionFactory` (در `Ekip.Application/Interfaces/`) با خروجی `ValueTask<DbConnection>` (پشتیبانی از `OpenAsync` و `await using`) در `SqlConnectionFactory` پیاده‌سازی شده و `DefaultTypeMap.MatchNamesWithUnderscores = true` را ست می‌کند.
  * یک **`NpgsqlDataSource` Singleton مشترک** (مجهز به `UseLoggerFactory`، `UseVector()` و `UseNetTopologySuite()`) بین `AddDbContextPool<EkipDbContext>` و `SqlConnectionFactory` به اشتراک گذاشته شده است تا Pool و Telemetry در Aspire یکپارچه بماند.

---

### ۳. قرارداد صفحه‌بندی (`CursorPagedResult<T>` و `CursorCodec`)

* **الگوی `pageSize + 1`:** کوئری‌های Read با `LIMIT @PageSize + 1` اجرا می‌شوند تا بدون نیاز به کوئری `COUNT(*)` وضعیت `HasMore` مشخص شود.
* **قرارداد واحد `string? Cursor` / `string? NextCursor` (الگوی هیبرید):**
  * در لیست‌های تک‌کلیدی مبتنی بر UUIDv7 (چت، نوتیفیکیشن، لجرها): رشته استاندارد `"D"` خود `Guid` بدون Base64 (`CursorCodec.EncodeId` / `TryDecodeId` با `Guid.TryParseExact(..., "D")`).
  * در فیدهای چندستونی (فاصله مکانی PostGIS، تاریخ رویداد، امتیاز هوش مصنوعی): ساختار `struct` با `System.Buffers.Text.Base64Url` و `stackalloc` همراه با سقف طول `512` کاراکتر (`CursorCodec.EncodeComposite<T>` / `TryDecodeComposite<T>`).

---

### ۴. قراردادهای CQRS، Behaviorها و `IUnitOfWork`

* **قراردادها (`Ekip.Application/Contracts/`):** `ICommand`, `ICommand<TValue>`, `IQuery<TValue>` به همراه مارکرهای `IBaseCommand` و `IBaseQuery` همگی به خروجی `Result` / `Result<TValue>` مقید هستند.
* **پایپ‌لاین (`Ekip.Application/Behaviors/`):**
  1. **`LoggingBehavior` (خارجی‌ترین لایه):** سنجش زمان با `Stopwatch.GetTimestamp()`، عدم لاگ کردن بدنه `request` جهت حفظ امنیت PII/توکن‌ها، تفکیک `ErrorType.Failure` (`LogError`) از ردشدن‌های عادی بیزینس (`LogInformation`)، هشدار کندی بالای `500ms` (`LogWarning`)، و مستثنی کردن `OperationCanceledException` ناشی از کنسل شدن کلاینت (`LogDebug`).
  2. **`ValidationBehavior`:** مقید به `where TResponse : struct, IResult<TResponse>` (فراخوانی `TResponse.Failure(...)` بدون Reflection/Boxing)، پیمایش ترتیبی `foreach` روی Validatorها (ایمن در برابر همزمانی `DbContext`)، استخراج `field` در `Metadata` و محافظت از فرمت `MODULE.ERROR_CODE` (فالبک به `GENERAL.VALIDATION_ERROR` در صورت فراموشی `.WithErrorCode()`).
* **`IUnitOfWork` (مسیر Outbox-First):**
  * `SaveChangesAsync`: پیش از `DbContext.SaveChangesAsync()`، رویدادهای `GetDomainEvents()` را به `IPublishEndpoint?` (با تایپ واقعی رویداد) تحویل می‌دهد و پس از ذخیره موفق `ClearDomainEvents()` را فراخوانی می‌کند *(تنظیم `o.UseBusOutbox()` در زمان پیکربندی MassTransit در گام ۲ الزامی است؛ هیچ رویدادی داخل Singleton `SaveChangesInterceptor` پابلیش نمی‌شود)*.
  * `ExecuteInTransactionAsync`: اجرای کل چرخه Load/Mutate/Save تراکنش‌های قفل‌دار (`FOR UPDATE` / `ExecuteUpdateAsync`) درون `CreateExecutionStrategy().ExecuteAsync(...)`؛ با `ChangeTracker.Clear()` خودکار در Retry و Rollback خودکار در صورت بازگشت `IResult { IsFailure: true }`.

---

### ۵. مپینگ خطاها در `Ekip.Api` (REST RFC 9457 و gRPC)

* **REST (`ResultExtensions` + `GlobalExceptionHandler`):**
  * نگاشت وضعیت‌ها: `Validation -> 400`, `Unauthorized -> 401`, `Forbidden -> 403`, `NotFound -> 404`, `Conflict -> 409`, `Failure -> 500` با فرمت `application/problem+json` شامل `errorCode`، `traceId`، `metadata` و آرایه `errors` (با حذف `metadata: null`).
  * `GlobalExceptionHandler`: نگاشت `OperationCanceledException` (`RequestAborted`) به `499` با `LogDebug`، نگاشت `BadHttpRequestException` به `400`، و عدم افشای Stack Trace در خطاهای `500` (`GENERAL.UNHANDLED_ERROR`).
* **gRPC (`GrpcResultExtensions` + `GrpcExceptionInterceptor`):**
  * نگاشت `ErrorType` به `StatusCode` (`InvalidArgument`, `NotFound`, `FailedPrecondition`, `Unauthenticated`, `PermissionDenied`, `Internal`) + ارسال `x-error-code`، `x-error-metadata` و `x-errors-json` در `Trailers` با `JsonSerializerDefaults.Web` (تضمین ASCII-Safe).
  * پوشش کامل متدهای Unary و Streaming در `GrpcExceptionInterceptor`.
  * `UseHttpsRedirection()` محدود به محیط `Development` جهت عدم تداخل با ترافیک داخلی `h2c` پشت Reverse Proxy.

---

### ۶. تست‌های معماری (`Ekip.ArchitectureTests`)

۱۰ تست خودکار در ۳ کلاس (`LayerDependencyTests`, `DomainRulesTests`, `CqrsRulesTests`):
* صفر بودن وابستگی NuGet و پروژه‌ای در `Ekip.Domain` و ایزولاسیون کامل `Ekip.Application` از `EF Core` / `Persistence` / `Dapper` / `Api`.
* کنترل `IAppendOnlyEntity` (حتماً `IAggregateRoot`؛ بدون `ISoftDeletable` و `IAuditableEntity`) و کنترل یکتایی و الگوی `^[A-Z0-9_]+\.[A-Z0-9_]+$` روی تمام `Error`های دامنه.
* کنترل `sealed` بودن Handlerها و Validatorها، ممنوعیت وابستگی `IQueryHandler` به `IUnitOfWork`، و الزام استفاده از `IBaseCommand`/`IBaseQuery` برای تمام Requestها.