using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HashPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VaultId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CipherTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleIV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CipherUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsernameIV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsernameTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CipherPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordIV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CipherUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlIV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CipherCommentary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComentaryIV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComentaryTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    VaultId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CipherTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleIV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CipherUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsernameIV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsernameTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CipherPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordIV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CipherUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlIV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CipherCommentary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComentaryIV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComentaryTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaultId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VaultEntryId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
