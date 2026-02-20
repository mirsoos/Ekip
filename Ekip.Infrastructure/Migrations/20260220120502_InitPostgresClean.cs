using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ekip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitPostgresClean : Migration
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
                    Gender = table.Column<bool>(type: "boolean", nullable: false),
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
                    VerificationLevel = table.Column<int>(type: "integer", nullable: false)
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
                    MaximumRequiredAssignmnets = table.Column<int>(type: "integer", nullable: true),
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
                name: "RequestReads");

            migrationBuilder.DropTable(
                name: "ProfileReads");

            migrationBuilder.DropTable(
                name: "UserReads");
        }
    }
}
