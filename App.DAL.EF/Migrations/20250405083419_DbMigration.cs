using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class DbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAuthTokens_Users_UserId",
                table: "UserAuthTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAuthTokens",
                table: "UserAuthTokens");

            migrationBuilder.RenameTable(
                name: "UserAuthTokens",
                newName: "UserAuth");

            migrationBuilder.RenameIndex(
                name: "IX_UserAuthTokens_UserId",
                table: "UserAuth",
                newName: "IX_UserAuth_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "StudentCode",
                table: "Users",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAuth",
                table: "UserAuth",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAuth_Users_UserId",
                table: "UserAuth",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAuth_Users_UserId",
                table: "UserAuth");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAuth",
                table: "UserAuth");

            migrationBuilder.RenameTable(
                name: "UserAuth",
                newName: "UserAuthTokens");

            migrationBuilder.RenameIndex(
                name: "IX_UserAuth_UserId",
                table: "UserAuthTokens",
                newName: "IX_UserAuthTokens_UserId");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "StudentCode",
                keyValue: null,
                column: "StudentCode",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "StudentCode",
                table: "Users",
                type: "varchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAuthTokens",
                table: "UserAuthTokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAuthTokens_Users_UserId",
                table: "UserAuthTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
