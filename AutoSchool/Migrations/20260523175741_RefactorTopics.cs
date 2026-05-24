using AutoSchool.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoSchool.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTopics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE Questions SET TopicId = 1 WHERE TopicId = 5;
                UPDATE Questions SET TopicId = 1 WHERE TopicId = 6;
                UPDATE Questions SET TopicId = 1 WHERE TopicId = 7;
            """);

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Основы управления ТС и БД");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Первая помощь пострадавшим");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Устройство и техобслуживание кат. «В»");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Правовые основы дорожного движения");

            migrationBuilder.DeleteData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Topics",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 5, "Обгон и опережение" },
                    { 6, "Остановка и стоянка" },
                    { 7, "Переезды и спецсигналы" }
                });

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Общее");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Светофор и регулировщик");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Дорожные знаки");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Скорость");
        }
    }
}
