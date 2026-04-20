using System;
using System.Collections.Generic;
using Ekip.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ekip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRoomReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChatRoomType = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    RequestRef = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorRef = table.Column<Guid>(type: "uuid", nullable: false),
                    LastMessagePreview = table.Column<string>(type: "text", nullable: true),
                    LastMessageDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Participants = table.Column<List<Guid>>(type: "uuid[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoomReads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ChatRoomRef = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderRef = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageContent = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsEdited = table.Column<bool>(type: "boolean", nullable: false),
                    ReplyToMessageRef = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserEkipReads",
                columns: table => new
                {
                    RequestRef = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatorRef = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorAvatar = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EkipTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequestType = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequestDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequestForbidDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequiredMembers = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MemberType = table.Column<int>(type: "integer", nullable: false),
                    IsAutoAccept = table.Column<bool>(type: "boolean", nullable: false),
                    CurrentMembersCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    MaximumRequiredMembers = table.Column<int>(type: "integer", nullable: true),
                    PendingAssignments = table.Column<List<PendingAssignmentInfo>>(type: "jsonb", nullable: true),
                    AcceptedMembers = table.Column<List<EkipMember>>(type: "jsonb", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    RequiredLevel = table.Column<int>(type: "integer", nullable: true),
                    MinimumScore = table.Column<double>(type: "double precision", nullable: true),
                    TargetGender = table.Column<int>(type: "integer", nullable: false),
                    MaximumAge = table.Column<int>(type: "integer", nullable: false),
                    MinimumAge = table.Column<int>(type: "integer", nullable: false),
                    IsRepeatable = table.Column<bool>(type: "boolean", nullable: false),
                    RepeatType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEkipReads", x => x.RequestRef);
                });

            migrationBuilder.CreateTable(
                name: "UserReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileRef = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserRef = table.Column<Guid>(type: "uuid", nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    Score = table.Column<double>(type: "double precision", nullable: true),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    VerificationLevel = table.Column<int>(type: "integer", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileReads_UserReads_UserRef",
                        column: x => x.UserRef,
                        principalTable: "UserReads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorRef = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    RequiredMembers = table.Column<int>(type: "integer", nullable: false),
                    MaximumRequiredAssignments = table.Column<int>(type: "integer", nullable: true),
                    RequestDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequestForbidDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequestType = table.Column<int>(type: "integer", nullable: false),
                    MemberType = table.Column<int>(type: "integer", nullable: false),
                    IsAutoAccept = table.Column<bool>(type: "boolean", nullable: false),
                    RequestFilters = table.Column<string>(type: "jsonb", nullable: true),
                    IsRepeatable = table.Column<bool>(type: "boolean", nullable: false),
                    RepeatType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestReads_ProfileReads_CreatorRef",
                        column: x => x.CreatorRef,
                        principalTable: "ProfileReads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestAssignmentReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestRef = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SenderRef = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAssignmentReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestAssignmentReads_ProfileReads_SenderRef",
                        column: x => x.SenderRef,
                        principalTable: "ProfileReads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestAssignmentReads_RequestReads_RequestRef",
                        column: x => x.RequestRef,
                        principalTable: "RequestReads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfileReads_UserRef",
                table: "ProfileReads",
                column: "UserRef");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAssignmentReads_RequestRef",
                table: "RequestAssignmentReads",
                column: "RequestRef");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAssignmentReads_SenderRef",
                table: "RequestAssignmentReads",
                column: "SenderRef");

            migrationBuilder.CreateIndex(
                name: "IX_RequestReads_CreatorRef",
                table: "RequestReads",
                column: "CreatorRef");

            migrationBuilder.CreateIndex(
                name: "IX_UserEkipReads_Creator_Status_Deleted",
                table: "UserEkipReads",
                columns: new[] { "CreatorRef", "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_UserEkipReads_CreatorRef",
                table: "UserEkipReads",
                column: "CreatorRef");

            migrationBuilder.CreateIndex(
                name: "IX_UserEkipReads_IsDeleted",
                table: "UserEkipReads",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserEkipReads_RequestDateTime",
                table: "UserEkipReads",
                column: "RequestDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_UserEkipReads_Status",
                table: "UserEkipReads",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatRoomReads");

            migrationBuilder.DropTable(
                name: "MessageReads");

            migrationBuilder.DropTable(
                name: "RequestAssignmentReads");

            migrationBuilder.DropTable(
                name: "UserEkipReads");

            migrationBuilder.DropTable(
                name: "RequestReads");

            migrationBuilder.DropTable(
                name: "ProfileReads");

            migrationBuilder.DropTable(
                name: "UserReads");
        }
    }
}
