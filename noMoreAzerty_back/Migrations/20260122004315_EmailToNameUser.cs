using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace noMoreAzerty_back.Migrations
{
    /// <inheritdoc />
    public partial class EmailToNameUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "Email");
        }
    }
}
