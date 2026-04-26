using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoSchool.Migrations
{
    /// <inheritdoc />
    public partial class AddTestingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { 1, "Билет 1" },
                    { 2, "Билет 2" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "Explanation", "Text", "TicketId" },
                values: new object[,]
                {
                    { 1, "Красный сигнал запрещает движение.", "Что означает красный сигнал светофора?", 1 },
                    { 2, "Обгон на пешеходном переходе запрещен.", "Можно ли выполнять обгон на пешеходном переходе?", 1 },
                    { 3, "Он указывает, что водитель находится на главной дороге.", "Что означает знак 'Главная дорога'?", 2 }
                });

            migrationBuilder.InsertData(
                table: "AnswerOptions",
                columns: new[] { "Id", "IsCorrect", "QuestionId", "Text" },
                values: new object[,]
                {
                    { 1, true, 1, "Движение запрещено" },
                    { 2, false, 1, "Можно ехать осторожно" },
                    { 3, false, 1, "Можно ехать только направо" },
                    { 4, false, 2, "Да, если нет пешеходов" },
                    { 5, true, 2, "Нет, запрещено" },
                    { 6, false, 2, "Да, если включен поворотник" },
                    { 7, false, 3, "Конец населенного пункта" },
                    { 8, true, 3, "Приоритет на перекрестках" },
                    { 9, false, 3, "Запрет остановки" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 3, "Teacher" },
                    { 4, "Owner" }
                });
        }
    }
}
