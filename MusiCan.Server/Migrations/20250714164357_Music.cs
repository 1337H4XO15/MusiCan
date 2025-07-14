using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusiCan.Server.Migrations
{
    /// <inheritdoc />
    public partial class Music : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Composer_Users_UserId",
                table: "Composer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Composer",
                table: "Composer");

            migrationBuilder.RenameTable(
                name: "Composer",
                newName: "Composers");

            migrationBuilder.RenameColumn(
                name: "Discription",
                table: "Composers",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_Composer_UserId",
                table: "Composers",
                newName: "IX_Composers_UserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Publication",
                table: "Musics",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "Musics",
                type: "TEXT",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 64);

            migrationBuilder.AddColumn<bool>(
                name: "Public",
                table: "Musics",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Musics",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Composers",
                table: "Composers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Composers_Users_UserId",
                table: "Composers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Composers_Users_UserId",
                table: "Composers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Composers",
                table: "Composers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Public",
                table: "Musics");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Musics");

            migrationBuilder.RenameTable(
                name: "Composers",
                newName: "Composer");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Composer",
                newName: "Discription");

            migrationBuilder.RenameIndex(
                name: "IX_Composers_UserId",
                table: "Composer",
                newName: "IX_Composer_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Publication",
                table: "Musics",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "Musics",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Composer",
                table: "Composer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Composer_Users_UserId",
                table: "Composer",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
