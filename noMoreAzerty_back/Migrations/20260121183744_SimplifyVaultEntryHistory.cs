using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace noMoreAzerty_back.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyVaultEntryHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaultEntryHistory_VaultEntry_VaultEntryId",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "CipherCommentary",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "CipherPassword",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "CipherTitle",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "CipherUrl",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "CipherUsername",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "ComentaryIV",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "ComentaryTag",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "PasswordIV",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "PasswordTag",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "TitleIV",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "TitleTag",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "UrlIV",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "UrlTag",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "UsernameIV",
                table: "VaultEntryHistory");

            migrationBuilder.DropColumn(
                name: "UsernameTag",
                table: "VaultEntryHistory");

            migrationBuilder.RenameColumn(
                name: "VaultEntryId",
                table: "VaultEntryHistory",
                newName: "EntryId");

            migrationBuilder.RenameIndex(
                name: "IX_VaultEntryHistory_VaultEntryId",
                table: "VaultEntryHistory",
                newName: "IX_VaultEntryHistory_EntryId");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordSalt",
                table: "Vault",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Vault",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HashPassword",
                table: "Vault",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VaultEntryHistory_VaultEntry_EntryId",
                table: "VaultEntryHistory",
                column: "EntryId",
                principalTable: "VaultEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaultEntryHistory_VaultEntry_EntryId",
                table: "VaultEntryHistory");

            migrationBuilder.RenameColumn(
                name: "EntryId",
                table: "VaultEntryHistory",
                newName: "VaultEntryId");

            migrationBuilder.RenameIndex(
                name: "IX_VaultEntryHistory_EntryId",
                table: "VaultEntryHistory",
                newName: "IX_VaultEntryHistory_VaultEntryId");

            migrationBuilder.AddColumn<string>(
                name: "CipherCommentary",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CipherPassword",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CipherTitle",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CipherUrl",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CipherUsername",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComentaryIV",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComentaryTag",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordIV",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordTag",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleIV",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleTag",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "VaultEntryHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlIV",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlTag",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsernameIV",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsernameTag",
                table: "VaultEntryHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordSalt",
                table: "Vault",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Vault",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HashPassword",
                table: "Vault",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "VaultEntryHistory",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "CipherCommentary", "CipherPassword", "CipherTitle", "CipherUrl", "CipherUsername", "ComentaryIV", "ComentaryTag", "PasswordIV", "PasswordTag", "TitleIV", "TitleTag", "UpdatedAt", "UrlIV", "UrlTag", "UsernameIV", "UsernameTag" },
                values: new object[] { "cipher_history_comment", "cipher_history_pass", "cipher_history_title", "cipher_history_url", "cipher_history_user", "hiv5", "htag5", "hiv3", "htag3", "hiv1", "htag1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "hiv4", "htag4", "hiv2", "htag2" });

            migrationBuilder.AddForeignKey(
                name: "FK_VaultEntryHistory_VaultEntry_VaultEntryId",
                table: "VaultEntryHistory",
                column: "VaultEntryId",
                principalTable: "VaultEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
