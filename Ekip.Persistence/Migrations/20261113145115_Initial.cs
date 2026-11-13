using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pgvector;

#nullable disable

namespace Ekip.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "AdminAccount",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    role = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    totp_secret = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    is_two_factor_activated = table.Column<bool>(type: "boolean", nullable: false),
                    failed_login_attempts = table.Column<int>(type: "integer", nullable: false),
                    locked_until_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_login_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    allowed_ips = table.Column<string[]>(type: "varchar(64)[]", nullable: false),
                    permissions = table.Column<string[]>(type: "varchar(100)[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admin_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    native_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    english_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    iso_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    phone_prefix = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    default_time_zone_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_country", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inbox_state",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consumer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lock_id = table.Column<Guid>(type: "uuid", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    received = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    receive_count = table.Column<int>(type: "integer", nullable: false),
                    expiration_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    consumed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_sequence_number = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox_state", x => x.id);
                    table.UniqueConstraint("ak_inbox_state_message_id_consumer_id", x => new { x.message_id, x.consumer_id });
                });

            migrationBuilder.CreateTable(
                name: "outbox_state",
                columns: table => new
                {
                    outbox_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lock_id = table.Column<Guid>(type: "uuid", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_sequence_number = table.Column<long>(type: "bigint", nullable: true),
                    bus_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_state", x => x.outbox_id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlan",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    entitlements = table.Column<string>(type: "jsonb", nullable: false),
                    duration_days = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    price_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    price_currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_plan", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccount",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    account_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_face_verified = table.Column<bool>(type: "boolean", nullable: false),
                    is_national_id_verified = table.Column<bool>(type: "boolean", nullable: false),
                    national_id_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    referral_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    referred_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    has_completed_tour = table.Column<bool>(type: "boolean", nullable: false),
                    terms_accepted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    oauth_providers = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_account", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_account_user_account_referred_by_id",
                        column: x => x.referred_by_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdminAuditLog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    admin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    target_entity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    old_value_json = table.Column<string>(type: "jsonb", nullable: true),
                    new_value_json = table.Column<string>(type: "jsonb", nullable: false),
                    reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    admin_ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admin_audit_log", x => x.id);
                    table.ForeignKey(
                        name: "fk_admin_audit_log_admin_account_admin_id",
                        column: x => x.admin_id,
                        principalTable: "AdminAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdminSession",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    admin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    refresh_token_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    token_version = table.Column<int>(type: "integer", nullable: false),
                    ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false),
                    revoked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admin_session", x => x.id);
                    table.ForeignKey(
                        name: "fk_admin_session_admin_account_admin_id",
                        column: x => x.admin_id,
                        principalTable: "AdminAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemSetting",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    value_json = table.Column<string>(type: "jsonb", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_admin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by_admin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_setting", x => x.id);
                    table.ForeignKey(
                        name: "fk_system_setting_admin_account_created_by_admin_id",
                        column: x => x.created_by_admin_id,
                        principalTable: "AdminAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_system_setting_admin_account_updated_by_admin_id",
                        column: x => x.updated_by_admin_id,
                        principalTable: "AdminAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Province",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    country_id = table.Column<Guid>(type: "uuid", nullable: false),
                    native_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    english_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_province", x => x.id);
                    table.ForeignKey(
                        name: "fk_province_country_country_id",
                        column: x => x.country_id,
                        principalTable: "Country",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "outbox_message",
                columns: table => new
                {
                    sequence_number = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    enqueue_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sent_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    headers = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    inbox_message_id = table.Column<Guid>(type: "uuid", nullable: true),
                    inbox_consumer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    outbox_id = table.Column<Guid>(type: "uuid", nullable: true),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    message_type = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    initiator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    destination_address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    response_address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    fault_address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    expiration_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_message", x => x.sequence_number);
                    table.ForeignKey(
                        name: "fk_outbox_message_inbox_state_inbox_message_id_inbox_consumer_",
                        columns: x => new { x.inbox_message_id, x.inbox_consumer_id },
                        principalTable: "inbox_state",
                        principalColumns: new[] { "message_id", "consumer_id" });
                    table.ForeignKey(
                        name: "fk_outbox_message_outbox_state_outbox_id",
                        column: x => x.outbox_id,
                        principalTable: "outbox_state",
                        principalColumn: "outbox_id");
                });

            migrationBuilder.CreateTable(
                name: "AbuseReport",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    idempotency_key = table.Column<Guid>(type: "uuid", nullable: false),
                    reporter_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reason = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    target_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    admin_note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    resolved_by_admin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resolved_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    evidence_media_asset_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abuse_report", x => x.id);
                    table.ForeignKey(
                        name: "fk_abuse_report_admin_account_resolved_by_admin_id",
                        column: x => x.resolved_by_admin_id,
                        principalTable: "AdminAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_abuse_report_user_accounts_reporter_id",
                        column: x => x.reporter_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AIConversation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    context_mode = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    total_tokens_used = table.Column<int>(type: "integer", nullable: false),
                    is_memory_extracted = table.Column<bool>(type: "boolean", nullable: false),
                    last_memory_extracted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_conversation", x => x.id);
                    table.ForeignKey(
                        name: "fk_ai_conversation_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaAsset",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    uploader_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usage_context = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    storage_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    nsfw_probability = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    moderation_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    error = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    scanned_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media_asset", x => x.id);
                    table.ForeignKey(
                        name: "fk_media_asset_user_accounts_uploader_id",
                        column: x => x.uploader_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    actor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    reference_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deep_link = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    read_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_user_accounts_actor_id",
                        column: x => x.actor_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_notification_user_accounts_recipient_id",
                        column: x => x.recipient_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromoCode",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    discount_percent = table.Column<int>(type: "integer", nullable: false),
                    max_discount_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    max_uses = table.Column<int>(type: "integer", nullable: false),
                    used_count = table.Column<int>(type: "integer", nullable: false),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promo_code", x => x.id);
                    table.ForeignKey(
                        name: "fk_promo_code_user_accounts_target_user_id",
                        column: x => x.target_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReputationLedger",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    actor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    event_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    xp_delta = table.Column<int>(type: "integer", nullable: false),
                    xp_balance_after = table.Column<int>(type: "integer", nullable: false),
                    honor_delta = table.Column<int>(type: "integer", nullable: false),
                    honor_balance_after = table.Column<int>(type: "integer", nullable: false),
                    reference_entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    reference_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    idempotency_key = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reputation_ledger", x => x.id);
                    table.ForeignKey(
                        name: "fk_reputation_ledger_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    category = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    assigned_admin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    closed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_admin_account_assigned_admin_id",
                        column: x => x.assigned_admin_id,
                        principalTable: "AdminAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_ticket_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserDeviceSession",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    device_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    device_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    app_version = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    fcm_push_token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    refresh_token_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    token_version = table.Column<int>(type: "integer", nullable: false),
                    ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    last_active_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false),
                    revoked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_reason = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_device_session", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_device_session_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGamificationSummary",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_xp = table.Column<int>(type: "integer", nullable: false),
                    current_level = table.Column<int>(type: "integer", nullable: false),
                    public_score = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    rating_scores_sum = table.Column<int>(type: "integer", nullable: false),
                    next_level_xp = table.Column<int>(type: "integer", nullable: false),
                    ratings_count = table.Column<int>(type: "integer", nullable: false),
                    total_honor = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    aggregated_counters = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_gamification_summary", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_gamification_summary_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserIdentityVector",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    personality_vector = table.Column<Vector>(type: "vector(1536)", nullable: false),
                    ai_persona_summary = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    city_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    birth_year = table.Column<int>(type: "integer", nullable: false),
                    current_level = table.Column<int>(type: "integer", nullable: false),
                    total_honor = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_identity_vector", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_identity_vector_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRelation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    requester_id = table.Column<Guid>(type: "uuid", nullable: false),
                    receiver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_relation", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_relation_user_account_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_relation_user_account_requester_id",
                        column: x => x.requester_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscription",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    starts_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    idempotency_key = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    quota_usage = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_subscription", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_subscription_subscription_plan_subscription_plan_id",
                        column: x => x.subscription_plan_id,
                        principalTable: "SubscriptionPlan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_subscription_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWallet",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    locked_balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_wallet", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_wallet_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletLedger",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    idempotency_key = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    amount_delta = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    reference_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wallet_ledger", x => x.id);
                    table.ForeignKey(
                        name: "fk_wallet_ledger_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    country_id = table.Column<Guid>(type: "uuid", nullable: false),
                    province_id = table.Column<Guid>(type: "uuid", nullable: false),
                    native_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    english_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    center_point = table.Column<Point>(type: "geography(Point, 4326)", nullable: false),
                    default_zoom_level = table.Column<double>(type: "double precision", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_city", x => x.id);
                    table.ForeignKey(
                        name: "fk_city_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "Country",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_city_provinces_province_id",
                        column: x => x.province_id,
                        principalTable: "Province",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AIMessage",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                    sent_by_user = table.Column<bool>(type: "boolean", nullable: false),
                    ai_conversation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tokens_used = table.Column<int>(type: "integer", nullable: false),
                    idempotency_key = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    suggested_ekip_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_message", x => x.id);
                    table.ForeignKey(
                        name: "fk_ai_message_ai_conversation_ai_conversation_id",
                        column: x => x.ai_conversation_id,
                        principalTable: "AIConversation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransaction",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    idempotency_key = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purpose = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    reference_entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    promo_code_id = table.Column<Guid>(type: "uuid", nullable: true),
                    base_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    wallet_amount_used = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    gateway_payable_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    gateway = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    authority_or_token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    reference_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    failure_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    paid_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    refunded_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_transaction", x => x.id);
                    table.ForeignKey(
                        name: "fk_payment_transaction_promo_codes_promo_code_id",
                        column: x => x.promo_code_id,
                        principalTable: "PromoCode",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_payment_transaction_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketMessage",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticket_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_from_admin = table.Column<bool>(type: "boolean", nullable: false),
                    message_text = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    attachment_asset_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_message", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_message_ticket_ticket_id",
                        column: x => x.ticket_id,
                        principalTable: "Ticket",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityTrendSnapshot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    city_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    keyword = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    trend_score = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    active_ekips_count = table.Column<int>(type: "integer", nullable: false),
                    rank = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_city_trend_snapshot", x => x.id);
                    table.ForeignKey(
                        name: "fk_city_trend_snapshot_city_city_id",
                        column: x => x.city_id,
                        principalTable: "City",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchQueryLog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    city_id = table.Column<Guid>(type: "uuid", nullable: false),
                    normalized_query = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    results_count = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_search_query_log", x => x.id);
                    table.ForeignKey(
                        name: "fk_search_query_log_city_city_id",
                        column: x => x.city_id,
                        principalTable: "City",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_search_query_log_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    user_gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    city_id = table.Column<Guid>(type: "uuid", nullable: true),
                    avatar_asset_id = table.Column<Guid>(type: "uuid", nullable: true),
                    banner_asset_id = table.Column<Guid>(type: "uuid", nullable: true),
                    bio = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    job_title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    mbti_types = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_searchable = table.Column<bool>(type: "boolean", nullable: false),
                    show_contacts = table.Column<bool>(type: "boolean", nullable: false),
                    show_last_seen = table.Column<bool>(type: "boolean", nullable: false),
                    last_seen_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    interests = table.Column<string[]>(type: "varchar(100)[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    mood_widget = table.Column<string>(type: "jsonb", nullable: true),
                    social_links = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_profile", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_profile_city_city_id",
                        column: x => x.city_id,
                        principalTable: "City",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_profile_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationCompanion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ekip_application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    companion_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    responded_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_companion", x => x.id);
                    table.ForeignKey(
                        name: "fk_application_companion_user_accounts_companion_user_id",
                        column: x => x.companion_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reply_to_message_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    content_text = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    location = table.Column<Point>(type: "geography(Point, 4326)", nullable: true),
                    referenced_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_edited = table.Column<bool>(type: "boolean", nullable: false),
                    is_pinned = table.Column<bool>(type: "boolean", nullable: false),
                    idempotency_key = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    attachment = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_message", x => x.id);
                    table.ForeignKey(
                        name: "fk_chat_message_chat_message_reply_to_message_id",
                        column: x => x.reply_to_message_id,
                        principalTable: "ChatMessage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_chat_message_user_accounts_sender_id",
                        column: x => x.sender_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conversation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversation_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ekip_id = table.Column<Guid>(type: "uuid", nullable: true),
                    participant_one_id = table.Column<Guid>(type: "uuid", nullable: true),
                    participant_two_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_message_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_message_preview = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    last_message_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    theme = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conversation", x => x.id);
                    table.ForeignKey(
                        name: "fk_conversation_user_accounts_participant_one_id",
                        column: x => x.participant_one_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_conversation_user_accounts_participant_two_id",
                        column: x => x.participant_two_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConversationParticipant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_read_message_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_read_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    unread_count = table.Column<int>(type: "integer", nullable: false),
                    is_muted = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conversation_participant", x => x.id);
                    table.ForeignKey(
                        name: "fk_conversation_participant_chat_message_last_read_message_id",
                        column: x => x.last_read_message_id,
                        principalTable: "ChatMessage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_conversation_participant_conversation_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "Conversation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_conversation_participant_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ekip",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    idempotency_key = table.Column<Guid>(type: "uuid", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recurring_ekip_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    sub_category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    city_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sponsored_place_id = table.Column<Guid>(type: "uuid", nullable: true),
                    exact_location_point = table.Column<Point>(type: "geography(Point, 4326)", nullable: false),
                    public_map_point = table.Column<Point>(type: "geography(Point, 4326)", nullable: false),
                    location_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address_detail = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_details_private = table.Column<bool>(type: "boolean", nullable: false),
                    host_composition = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    min_members = table.Column<int>(type: "integer", nullable: false),
                    max_members = table.Column<int>(type: "integer", nullable: true),
                    approved_members_count = table.Column<int>(type: "integer", nullable: false),
                    estimated_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    event_date_time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    join_deadline_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    views_count = table.Column<int>(type: "integer", nullable: false),
                    moderation_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    lifecycle_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    canceled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    quorum_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    quorum_decision_deadline_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tags = table.Column<string[]>(type: "varchar(100)[]", nullable: false),
                    media_asset_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    join_criteria = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ekip", x => x.id);
                    table.ForeignKey(
                        name: "fk_ekip_cities_city_id",
                        column: x => x.city_id,
                        principalTable: "City",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ekip_user_accounts_creator_id",
                        column: x => x.creator_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EkipApplication",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    idempotency_key = table.Column<Guid>(type: "uuid", nullable: false),
                    ekip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    applicant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    join_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    intro_note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    application_status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ekip_application", x => x.id);
                    table.ForeignKey(
                        name: "fk_ekip_application_ekip_ekip_id",
                        column: x => x.ekip_id,
                        principalTable: "Ekip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ekip_application_user_accounts_applicant_id",
                        column: x => x.applicant_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EkipHostCompanion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ekip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    companion_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    companion_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    responded_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ekip_host_companion", x => x.id);
                    table.ForeignKey(
                        name: "fk_ekip_host_companion_ekip_ekip_id",
                        column: x => x.ekip_id,
                        principalTable: "Ekip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ekip_host_companion_user_accounts_companion_user_id",
                        column: x => x.companion_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EkipSemanticVector",
                columns: table => new
                {
                    ekip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_vector = table.Column<Vector>(type: "vector(1536)", nullable: false),
                    city_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ekip_semantic_vector", x => x.ekip_id);
                    table.ForeignKey(
                        name: "fk_ekip_semantic_vector_cities_city_id",
                        column: x => x.city_id,
                        principalTable: "City",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ekip_semantic_vector_ekips_ekip_id",
                        column: x => x.ekip_id,
                        principalTable: "Ekip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileReview",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ekip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false),
                    comment_text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    toxicity_score = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    moderation_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    idempotency_key = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profile_review", x => x.id);
                    table.ForeignKey(
                        name: "fk_profile_review_ekip_ekip_id",
                        column: x => x.ekip_id,
                        principalTable: "Ekip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_profile_review_user_accounts_author_user_id",
                        column: x => x.author_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_profile_review_user_accounts_target_user_id",
                        column: x => x.target_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurringEkip",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_ekip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    frequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    join_deadline_offset_hours = table.Column<int>(type: "integer", nullable: false),
                    occurrences_count = table.Column<int>(type: "integer", nullable: false),
                    max_occurrences = table.Column<int>(type: "integer", nullable: true),
                    last_event_date_time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ends_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recurring_ekip", x => x.id);
                    table.ForeignKey(
                        name: "fk_recurring_ekip_ekip_source_ekip_id",
                        column: x => x.source_ekip_id,
                        principalTable: "Ekip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_recurring_ekip_user_accounts_creator_id",
                        column: x => x.creator_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavedEkip",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ekip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saved_ekip", x => x.id);
                    table.ForeignKey(
                        name: "fk_saved_ekip_ekip_ekip_id",
                        column: x => x.ekip_id,
                        principalTable: "Ekip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_saved_ekip_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EkipMember",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ekip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    membership_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    attendance_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    check_in_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    check_in_location = table.Column<Point>(type: "geography(Point, 4326)", nullable: true),
                    no_show_reported_count = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ekip_member", x => x.id);
                    table.ForeignKey(
                        name: "fk_ekip_member_ekip_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "EkipApplication",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_ekip_member_ekip_ekip_id",
                        column: x => x.ekip_id,
                        principalTable: "Ekip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ekip_member_user_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "UserAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_abuse_report_idempotency_key",
                table: "AbuseReport",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_abuse_report_reporter_id",
                table: "AbuseReport",
                column: "reporter_id");

            migrationBuilder.CreateIndex(
                name: "ix_abuse_report_resolved_by_admin_id",
                table: "AbuseReport",
                column: "resolved_by_admin_id");

            migrationBuilder.CreateIndex(
                name: "ix_abuse_report_status_created_at_utc",
                table: "AbuseReport",
                columns: new[] { "status", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_abuse_report_target_type_target_id",
                table: "AbuseReport",
                columns: new[] { "target_type", "target_id" });

            migrationBuilder.CreateIndex(
                name: "ix_admin_account_email",
                table: "AdminAccount",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_admin_account_user_name",
                table: "AdminAccount",
                column: "user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_admin_audit_log_admin_id_created_at_utc",
                table: "AdminAuditLog",
                columns: new[] { "admin_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_admin_audit_log_target_entity_target_id",
                table: "AdminAuditLog",
                columns: new[] { "target_entity", "target_id" });

            migrationBuilder.CreateIndex(
                name: "ix_admin_session_admin_id_is_revoked",
                table: "AdminSession",
                columns: new[] { "admin_id", "is_revoked" });

            migrationBuilder.CreateIndex(
                name: "ix_admin_session_refresh_token_hash",
                table: "AdminSession",
                column: "refresh_token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ai_conversation_is_memory_extracted",
                table: "AIConversation",
                column: "is_memory_extracted",
                filter: "is_memory_extracted = false");

            migrationBuilder.CreateIndex(
                name: "ix_ai_conversation_user_id_updated_at_utc",
                table: "AIConversation",
                columns: new[] { "user_id", "updated_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_ai_message_ai_conversation_id_created_at_utc",
                table: "AIMessage",
                columns: new[] { "ai_conversation_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_ai_message_idempotency_key",
                table: "AIMessage",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_application_companion_companion_user_id_status",
                table: "ApplicationCompanion",
                columns: new[] { "companion_user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_application_companion_ekip_application_id_companion_user_id",
                table: "ApplicationCompanion",
                columns: new[] { "ekip_application_id", "companion_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_chat_message_conversation_id_created_at_utc",
                table: "ChatMessage",
                columns: new[] { "conversation_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_chat_message_conversation_id_is_pinned",
                table: "ChatMessage",
                columns: new[] { "conversation_id", "is_pinned" },
                filter: "is_pinned = true");

            migrationBuilder.CreateIndex(
                name: "ix_chat_message_idempotency_key",
                table: "ChatMessage",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_chat_message_reply_to_message_id",
                table: "ChatMessage",
                column: "reply_to_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_chat_message_sender_id",
                table: "ChatMessage",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "ix_city_center_point",
                table: "City",
                column: "center_point")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "ix_city_country_id_is_active",
                table: "City",
                columns: new[] { "country_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_city_province_id_english_name",
                table: "City",
                columns: new[] { "province_id", "english_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_city_province_id_is_active_display_order",
                table: "City",
                columns: new[] { "province_id", "is_active", "display_order" });

            migrationBuilder.CreateIndex(
                name: "ix_city_trend_snapshot_city_id_created_at_utc_rank",
                table: "CityTrendSnapshot",
                columns: new[] { "city_id", "created_at_utc", "rank" });

            migrationBuilder.CreateIndex(
                name: "ix_conversation_ekip_id",
                table: "Conversation",
                column: "ekip_id",
                unique: true,
                filter: "ekip_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_conversation_last_message_at_utc",
                table: "Conversation",
                column: "last_message_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_conversation_participant_one_id_participant_two_id",
                table: "Conversation",
                columns: new[] { "participant_one_id", "participant_two_id" },
                unique: true,
                filter: "participant_one_id IS NOT NULL AND participant_two_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_conversation_participant_two_id",
                table: "Conversation",
                column: "participant_two_id");

            migrationBuilder.CreateIndex(
                name: "ix_conversation_participant_conversation_id_user_id",
                table: "ConversationParticipant",
                columns: new[] { "conversation_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_conversation_participant_last_read_message_id",
                table: "ConversationParticipant",
                column: "last_read_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_conversation_participant_user_id_is_active",
                table: "ConversationParticipant",
                columns: new[] { "user_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_country_is_active_display_order",
                table: "Country",
                columns: new[] { "is_active", "display_order" });

            migrationBuilder.CreateIndex(
                name: "ix_country_iso_code",
                table: "Country",
                column: "iso_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ekip_city_id_lifecycle_status_moderation_status_event_date_",
                table: "Ekip",
                columns: new[] { "city_id", "lifecycle_status", "moderation_status", "event_date_time_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_ekip_creator_id",
                table: "Ekip",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_ekip_idempotency_key",
                table: "Ekip",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ekip_public_map_point",
                table: "Ekip",
                column: "public_map_point")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "ix_ekip_recurring_ekip_id",
                table: "Ekip",
                column: "recurring_ekip_id");

            migrationBuilder.CreateIndex(
                name: "ix_ekip_tags",
                table: "Ekip",
                column: "tags")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "ix_ekip_application_applicant_id_ekip_id",
                table: "EkipApplication",
                columns: new[] { "applicant_id", "ekip_id" });

            migrationBuilder.CreateIndex(
                name: "ix_ekip_application_ekip_id_application_status_created_at_utc",
                table: "EkipApplication",
                columns: new[] { "ekip_id", "application_status", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_ekip_application_idempotency_key",
                table: "EkipApplication",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ekip_host_companion_companion_user_id",
                table: "EkipHostCompanion",
                column: "companion_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_ekip_host_companion_ekip_id_companion_user_id",
                table: "EkipHostCompanion",
                columns: new[] { "ekip_id", "companion_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ekip_member_application_id",
                table: "EkipMember",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_ekip_member_ekip_id_user_id",
                table: "EkipMember",
                columns: new[] { "ekip_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ekip_member_user_id_membership_status",
                table: "EkipMember",
                columns: new[] { "user_id", "membership_status" });

            migrationBuilder.CreateIndex(
                name: "ix_ekip_semantic_vector_city_id_is_available",
                table: "EkipSemanticVector",
                columns: new[] { "city_id", "is_available" },
                filter: "is_available = true");

            migrationBuilder.CreateIndex(
                name: "ix_ekip_semantic_vector_content_vector",
                table: "EkipSemanticVector",
                column: "content_vector",
                filter: "is_available = true")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_inbox_state_delivered",
                table: "inbox_state",
                column: "delivered");

            migrationBuilder.CreateIndex(
                name: "ix_media_asset_storage_key",
                table: "MediaAsset",
                column: "storage_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_media_asset_uploader_id_moderation_status",
                table: "MediaAsset",
                columns: new[] { "uploader_id", "moderation_status" });

            migrationBuilder.CreateIndex(
                name: "ix_notification_actor_id",
                table: "Notification",
                column: "actor_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_recipient_id_is_read_created_at_utc",
                table: "Notification",
                columns: new[] { "recipient_id", "is_read", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_inbox_message_id_inbox_consumer_id_sequence_",
                table: "outbox_message",
                columns: new[] { "inbox_message_id", "inbox_consumer_id", "sequence_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_outbox_id_sequence_number",
                table: "outbox_message",
                columns: new[] { "outbox_id", "sequence_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbox_state_bus_name_created",
                table: "outbox_state",
                columns: new[] { "bus_name", "created" });

            migrationBuilder.CreateIndex(
                name: "ix_payment_transaction_authority_or_token",
                table: "PaymentTransaction",
                column: "authority_or_token",
                filter: "authority_or_token IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_payment_transaction_idempotency_key",
                table: "PaymentTransaction",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_payment_transaction_promo_code_id",
                table: "PaymentTransaction",
                column: "promo_code_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_transaction_reference_id",
                table: "PaymentTransaction",
                column: "reference_id",
                unique: true,
                filter: "reference_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_payment_transaction_user_id_created_at_utc",
                table: "PaymentTransaction",
                columns: new[] { "user_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_profile_review_author_user_id",
                table: "ProfileReview",
                column: "author_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_profile_review_ekip_id_author_user_id_target_user_id",
                table: "ProfileReview",
                columns: new[] { "ekip_id", "author_user_id", "target_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_profile_review_idempotency_key",
                table: "ProfileReview",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_profile_review_target_user_id_moderation_status",
                table: "ProfileReview",
                columns: new[] { "target_user_id", "moderation_status" });

            migrationBuilder.CreateIndex(
                name: "ix_promo_code_code",
                table: "PromoCode",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_promo_code_target_user_id_is_active",
                table: "PromoCode",
                columns: new[] { "target_user_id", "is_active" },
                filter: "target_user_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_province_country_id_english_name",
                table: "Province",
                columns: new[] { "country_id", "english_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_province_country_id_is_active_display_order",
                table: "Province",
                columns: new[] { "country_id", "is_active", "display_order" });

            migrationBuilder.CreateIndex(
                name: "ix_recurring_ekip_creator_id_is_active",
                table: "RecurringEkip",
                columns: new[] { "creator_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_recurring_ekip_source_ekip_id",
                table: "RecurringEkip",
                column: "source_ekip_id");

            migrationBuilder.CreateIndex(
                name: "ix_reputation_ledger_idempotency_key",
                table: "ReputationLedger",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reputation_ledger_user_id_created_at_utc",
                table: "ReputationLedger",
                columns: new[] { "user_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_saved_ekip_ekip_id",
                table: "SavedEkip",
                column: "ekip_id");

            migrationBuilder.CreateIndex(
                name: "ix_saved_ekip_user_id_ekip_id",
                table: "SavedEkip",
                columns: new[] { "user_id", "ekip_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_search_query_log_city_id_normalized_query_created_at_utc",
                table: "SearchQueryLog",
                columns: new[] { "city_id", "normalized_query", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_search_query_log_user_id",
                table: "SearchQueryLog",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_plan_code",
                table: "SubscriptionPlan",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_system_setting_created_by_admin_id",
                table: "SystemSetting",
                column: "created_by_admin_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_setting_is_public",
                table: "SystemSetting",
                column: "is_public",
                filter: "is_public = true");

            migrationBuilder.CreateIndex(
                name: "ix_system_setting_key",
                table: "SystemSetting",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_system_setting_updated_by_admin_id",
                table: "SystemSetting",
                column: "updated_by_admin_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_assigned_admin_id",
                table: "Ticket",
                column: "assigned_admin_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_status_priority_updated_at_utc",
                table: "Ticket",
                columns: new[] { "status", "priority", "updated_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_ticket_user_id_created_at_utc",
                table: "Ticket",
                columns: new[] { "user_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_ticket_message_ticket_id_created_at_utc",
                table: "TicketMessage",
                columns: new[] { "ticket_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_user_account_email",
                table: "UserAccount",
                column: "email",
                unique: true,
                filter: "email IS NOT NULL AND is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_user_account_national_id_hash",
                table: "UserAccount",
                column: "national_id_hash",
                unique: true,
                filter: "national_id_hash IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_user_account_phone_number",
                table: "UserAccount",
                column: "phone_number",
                unique: true,
                filter: "phone_number IS NOT NULL AND is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_user_account_referral_code",
                table: "UserAccount",
                column: "referral_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_account_referred_by_id",
                table: "UserAccount",
                column: "referred_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_account_username",
                table: "UserAccount",
                column: "username",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_user_device_session_fcm_push_token",
                table: "UserDeviceSession",
                column: "fcm_push_token",
                filter: "fcm_push_token IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_user_device_session_refresh_token_hash",
                table: "UserDeviceSession",
                column: "refresh_token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_device_session_user_id_device_id",
                table: "UserDeviceSession",
                columns: new[] { "user_id", "device_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_identity_vector_city_id_user_gender_birth_year",
                table: "UserIdentityVector",
                columns: new[] { "city_id", "user_gender", "birth_year" });

            migrationBuilder.CreateIndex(
                name: "ix_user_identity_vector_personality_vector",
                table: "UserIdentityVector",
                column: "personality_vector")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_user_profile_city_id",
                table: "UserProfile",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_profile_interests",
                table: "UserProfile",
                column: "interests")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "ix_user_relation_receiver_id_status",
                table: "UserRelation",
                columns: new[] { "receiver_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_user_relation_requester_id_receiver_id",
                table: "UserRelation",
                columns: new[] { "requester_id", "receiver_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_subscription_expires_at_utc",
                table: "UserSubscription",
                column: "expires_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_user_subscription_idempotency_key",
                table: "UserSubscription",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_subscription_subscription_plan_id",
                table: "UserSubscription",
                column: "subscription_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_wallet_ledger_idempotency_key",
                table: "WalletLedger",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_wallet_ledger_user_id_created_at_utc",
                table: "WalletLedger",
                columns: new[] { "user_id", "created_at_utc" });

            migrationBuilder.AddForeignKey(
                name: "fk_application_companion_ekip_applications_ekip_application_id",
                table: "ApplicationCompanion",
                column: "ekip_application_id",
                principalTable: "EkipApplication",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_chat_message_conversations_conversation_id",
                table: "ChatMessage",
                column: "conversation_id",
                principalTable: "Conversation",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_conversation_ekip_ekip_id",
                table: "Conversation",
                column: "ekip_id",
                principalTable: "Ekip",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_ekip_recurring_ekips_recurring_ekip_id",
                table: "Ekip",
                column: "recurring_ekip_id",
                principalTable: "RecurringEkip",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ekip_user_accounts_creator_id",
                table: "Ekip");

            migrationBuilder.DropForeignKey(
                name: "fk_recurring_ekip_user_accounts_creator_id",
                table: "RecurringEkip");

            migrationBuilder.DropForeignKey(
                name: "fk_city_countries_country_id",
                table: "City");

            migrationBuilder.DropForeignKey(
                name: "fk_province_country_country_id",
                table: "Province");

            migrationBuilder.DropForeignKey(
                name: "fk_city_provinces_province_id",
                table: "City");

            migrationBuilder.DropForeignKey(
                name: "fk_ekip_cities_city_id",
                table: "Ekip");

            migrationBuilder.DropForeignKey(
                name: "fk_recurring_ekip_ekip_source_ekip_id",
                table: "RecurringEkip");

            migrationBuilder.DropTable(
                name: "AbuseReport");

            migrationBuilder.DropTable(
                name: "AdminAuditLog");

            migrationBuilder.DropTable(
                name: "AdminSession");

            migrationBuilder.DropTable(
                name: "AIMessage");

            migrationBuilder.DropTable(
                name: "ApplicationCompanion");

            migrationBuilder.DropTable(
                name: "CityTrendSnapshot");

            migrationBuilder.DropTable(
                name: "ConversationParticipant");

            migrationBuilder.DropTable(
                name: "EkipHostCompanion");

            migrationBuilder.DropTable(
                name: "EkipMember");

            migrationBuilder.DropTable(
                name: "EkipSemanticVector");

            migrationBuilder.DropTable(
                name: "MediaAsset");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "outbox_message");

            migrationBuilder.DropTable(
                name: "PaymentTransaction");

            migrationBuilder.DropTable(
                name: "ProfileReview");

            migrationBuilder.DropTable(
                name: "ReputationLedger");

            migrationBuilder.DropTable(
                name: "SavedEkip");

            migrationBuilder.DropTable(
                name: "SearchQueryLog");

            migrationBuilder.DropTable(
                name: "SystemSetting");

            migrationBuilder.DropTable(
                name: "TicketMessage");

            migrationBuilder.DropTable(
                name: "UserDeviceSession");

            migrationBuilder.DropTable(
                name: "UserGamificationSummary");

            migrationBuilder.DropTable(
                name: "UserIdentityVector");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "UserRelation");

            migrationBuilder.DropTable(
                name: "UserSubscription");

            migrationBuilder.DropTable(
                name: "UserWallet");

            migrationBuilder.DropTable(
                name: "WalletLedger");

            migrationBuilder.DropTable(
                name: "AIConversation");

            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "EkipApplication");

            migrationBuilder.DropTable(
                name: "inbox_state");

            migrationBuilder.DropTable(
                name: "outbox_state");

            migrationBuilder.DropTable(
                name: "PromoCode");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "SubscriptionPlan");

            migrationBuilder.DropTable(
                name: "Conversation");

            migrationBuilder.DropTable(
                name: "AdminAccount");

            migrationBuilder.DropTable(
                name: "UserAccount");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Province");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Ekip");

            migrationBuilder.DropTable(
                name: "RecurringEkip");
        }
    }
}
