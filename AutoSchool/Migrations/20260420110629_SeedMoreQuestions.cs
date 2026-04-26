using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoSchool.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoreQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "Explanation", "Text", "TicketId" },
                values: new object[,]
                {
                    { 101, "Движение разрешает зелёный сигнал светофора.", "Какой сигнал светофора разрешает движение?", 1 },
                    { 102, "Жёлтый сигнал запрещает движение, кроме случаев, когда остановка может быть опасной или невозможной.", "Разрешено ли движение на жёлтый сигнал светофора?", 1 },
                    { 103, "Водитель обязан уступить дорогу транспортным средствам, движущимся по пересекаемой дороге.", "Что означает знак «Уступите дорогу»?", 1 },
                    { 104, "Если не установлены иные ограничения, в населённом пункте разрешено 60 км/ч.", "Максимальная скорость в населённом пункте, если не указано иное?", 1 },
                    { 105, "Да, если конструкцией транспортного средства предусмотрены ремни безопасности.", "Должны ли водитель и пассажиры быть пристёгнуты ремнями безопасности?", 1 },
                    { 106, "Остановка на пешеходном переходе запрещена.", "Разрешена ли остановка на пешеходном переходе?", 1 },
                    { 107, "Остановка ближе 5 метров перед переходом запрещена.", "Разрешена ли остановка ближе 5 метров перед пешеходным переходом?", 1 },
                    { 108, "Остановиться нужно перед стоп-линией, не заезжая на неё.", "Как нужно остановиться перед стоп-линией при запрещающем сигнале?", 1 },
                    { 201, "Знак запрещает въезд транспортных средств в данном направлении.", "Что означает знак «Въезд запрещён»?", 2 },
                    { 202, "Преимущество имеет транспортное средство, приближающееся справа (правило «помехи справа»).", "Кто имеет преимущество на перекрёстке равнозначных дорог?", 2 },
                    { 203, "Пользоваться телефоном без устройства hands-free запрещено.", "Разрешено ли пользоваться телефоном без гарнитуры во время движения?", 2 },
                    { 204, "Водитель обязан иметь водительское удостоверение соответствующей категории.", "Какой документ обязан иметь водитель при управлении транспортным средством?", 2 },
                    { 205, "Днём должны быть включены дневные ходовые огни или ближний свет фар.", "Что должно быть включено днём для обозначения движущегося транспортного средства?", 2 },
                    { 206, "Разворот на железнодорожном переезде запрещён.", "Разрешён ли разворот на железнодорожном переезде?", 2 },
                    { 207, "Нужно уступить дорогу и обеспечить беспрепятственный проезд.", "Как поступить при приближении автомобиля спецслужб с маячком и сиреной?", 2 },
                    { 208, "Пересечение сплошной линии 1.1 запрещено.", "Можно ли пересекать сплошную линию разметки 1.1?", 2 },
                    { 209, "Обгон в тоннеле запрещён.", "Разрешён ли обгон в тоннеле?", 2 }
                });

            migrationBuilder.InsertData(
                table: "AnswerOptions",
                columns: new[] { "Id", "IsCorrect", "QuestionId", "Text" },
                values: new object[,]
                {
                    { 1001, true, 101, "Зелёный" },
                    { 1002, false, 101, "Красный" },
                    { 1003, false, 101, "Жёлтый" },
                    { 1004, true, 102, "Запрещено, кроме случаев когда остановка невозможна" },
                    { 1005, false, 102, "Разрешено всегда" },
                    { 1006, false, 102, "Разрешено только направо" },
                    { 1007, true, 103, "Нужно уступить транспортным средствам на пересекаемой дороге" },
                    { 1008, false, 103, "Движение запрещено" },
                    { 1009, false, 103, "Вы на главной дороге" },
                    { 1010, true, 104, "60 км/ч" },
                    { 1011, false, 104, "90 км/ч" },
                    { 1012, false, 104, "40 км/ч" },
                    { 1013, true, 105, "Да, если предусмотрены конструкцией" },
                    { 1014, false, 105, "Только водитель" },
                    { 1015, false, 105, "Только на трассе" },
                    { 1016, true, 106, "Нет, запрещена" },
                    { 1017, false, 106, "Да, если нет пешеходов" },
                    { 1018, false, 106, "Да, не более 1 минуты" },
                    { 1019, true, 107, "Да, запрещена" },
                    { 1020, false, 107, "Нет, разрешена" },
                    { 1021, false, 107, "Запрещена только после перехода" },
                    { 1022, true, 108, "Перед стоп-линией" },
                    { 1023, false, 108, "На стоп-линии" },
                    { 1024, false, 108, "После стоп-линии" },
                    { 1025, true, 201, "Въезд запрещён в данном направлении" },
                    { 1026, false, 201, "Движение запрещено для всех" },
                    { 1027, false, 201, "Остановка запрещена" },
                    { 1028, true, 202, "Транспортное средство справа" },
                    { 1029, false, 202, "Транспортное средство слева" },
                    { 1030, false, 202, "Тот, кто едет быстрее" },
                    { 1031, true, 203, "Запрещено" },
                    { 1032, false, 203, "Разрешено" },
                    { 1033, false, 203, "Разрешено только в пробке" },
                    { 1034, true, 204, "Водительское удостоверение соответствующей категории" },
                    { 1035, false, 204, "Паспорт" },
                    { 1036, false, 204, "Свидетельство о рождении" },
                    { 1037, true, 205, "ДХО или ближний свет фар" },
                    { 1038, false, 205, "Только дальний свет" },
                    { 1039, false, 205, "Только противотуманные фары" },
                    { 1040, true, 206, "Нет, запрещён" },
                    { 1041, false, 206, "Да, если нет поезда" },
                    { 1042, false, 206, "Да, если включены аварийные огни" },
                    { 1043, true, 207, "Уступить дорогу и обеспечить проезд" },
                    { 1044, false, 207, "Продолжать движение без изменений" },
                    { 1045, false, 207, "Остановиться посреди полосы" },
                    { 1046, true, 208, "Нет, нельзя" },
                    { 1047, false, 208, "Да, если нет встречных" },
                    { 1048, false, 208, "Да, если спешите" },
                    { 1049, true, 209, "Нет, запрещён" },
                    { 1050, false, 209, "Да, разрешён всегда" },
                    { 1051, false, 209, "Разрешён только днём" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1006);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1007);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1008);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1009);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1010);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1011);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1012);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1013);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1014);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1015);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1016);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1017);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1018);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1019);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1020);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1021);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1022);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1023);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1024);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1025);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1026);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1027);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1028);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1029);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1030);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1031);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1032);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1033);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1034);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1035);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1036);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1037);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1038);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1039);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1040);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1041);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1042);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1043);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1044);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1045);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1046);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1047);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1048);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1049);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1050);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "Id",
                keyValue: 1051);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 209);

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
    }
}
