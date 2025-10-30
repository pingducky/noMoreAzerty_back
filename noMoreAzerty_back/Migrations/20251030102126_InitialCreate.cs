using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace noMoreAzerty_back.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vault",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HashPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vault", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vault_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Share",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Share", x => new { x.UserId, x.VaultId });
                    table.ForeignKey(
                        name: "FK_Share_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Share_Vault_VaultId",
                        column: x => x.VaultId,
                        principalTable: "Vault",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VaultEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CipherTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleIV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CipherUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsernameIV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsernameTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CipherPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordIV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CipherUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlIV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CipherCommentary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComentaryIV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComentaryTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    VaultId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaultEntry_Vault_VaultId",
                        column: x => x.VaultId,
                        principalTable: "Vault",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VaultEntryHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CipherTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleIV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CipherUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsernameIV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsernameTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CipherPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordIV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CipherUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlIV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CipherCommentary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComentaryIV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComentaryTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Action = table.Column<int>(type: "int", nullable: false),
                    VaultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaultEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultEntryHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaultEntryHistory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaultEntryHistory_VaultEntry_VaultEntryId",
                        column: x => x.VaultEntryId,
                        principalTable: "VaultEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VaultEntryHistory_Vault_VaultId",
                        column: x => x.VaultId,
                        principalTable: "Vault",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "IsActive", "LastLogin" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Vault",
                columns: new[] { "Id", "CreatedAt", "HashPassword", "Name", "PasswordSalt", "UserId" },
                values: new object[] { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "hashed-password-demo", "Vault personnel", "random-salt-demo", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") });

            migrationBuilder.InsertData(
                table: "Share",
                columns: new[] { "UserId", "VaultId", "AddedAt" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "VaultEntry",
                columns: new[] { "Id", "CipherCommentary", "CipherPassword", "CipherTitle", "CipherUrl", "CipherUsername", "ComentaryIV", "ComentaryTag", "CreatedAt", "IsActive", "PasswordIV", "PasswordTag", "TitleIV", "TitleTag", "UpdatedAt", "UrlIV", "UrlTag", "UsernameIV", "UsernameTag", "VaultId" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "cipher_comment", "cipher_gmail_password", "cipher_gmail_title", "cipher_gmail_url", "cipher_gmail_user", "iv5", "tag5", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "iv3", "tag3", "iv1", "tag1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "iv4", "tag4", "iv2", "tag2", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "cipher_comment2", "cipher_github_password", "cipher_github_title", "cipher_github_url", "cipher_github_user", "iv10", "tag10", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "iv8", "tag8", "iv6", "tag6", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "iv9", "tag9", "iv7", "tag7", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") }
                });

            migrationBuilder.InsertData(
                table: "VaultEntryHistory",
                columns: new[] { "Id", "Action", "CipherCommentary", "CipherPassword", "CipherTitle", "CipherUrl", "CipherUsername", "ComentaryIV", "ComentaryTag", "CreatedAt", "PasswordIV", "PasswordTag", "TitleIV", "TitleTag", "UpdatedAt", "UrlIV", "UrlTag", "UserId", "UsernameIV", "UsernameTag", "VaultEntryId", "VaultId" },
                values: new object[] { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), 0, "cipher_history_comment", "cipher_history_pass", "cipher_history_title", "cipher_history_url", "cipher_history_user", "hiv5", "htag5", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "hiv3", "htag3", "hiv1", "htag1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "hiv4", "htag4", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "hiv2", "htag2", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.CreateIndex(
                name: "IX_Share_VaultId",
                table: "Share",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_Vault_UserId",
                table: "Vault",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultEntry_VaultId",
                table: "VaultEntry",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultEntryHistory_UserId",
                table: "VaultEntryHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultEntryHistory_VaultEntryId",
                table: "VaultEntryHistory",
                column: "VaultEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultEntryHistory_VaultId",
                table: "VaultEntryHistory",
                column: "VaultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Share");

            migrationBuilder.DropTable(
                name: "VaultEntryHistory");

            migrationBuilder.DropTable(
                name: "VaultEntry");

            migrationBuilder.DropTable(
                name: "Vault");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
