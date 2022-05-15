using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace And9.Service.Core.Database.Migrations
{
    public partial class initSpecializationsData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Core",
                table: "Specialization",
                columns: new[] { "Id", "Direction", "Name" },
                values: new object[,]
                {
                    { 1, (short)2, "Шахтер" },
                    { 2, (short)2, "Монтажник" },
                    { 3, (short)2, "Пилот гражданских кораблей" },
                    { 4, (short)2, "Управляющий станцией" },
                    { 5, (short)2, "Комендант" },
                    { 6, (short)3, "Скриптер" },
                    { 7, (short)3, "Программист" },
                    { 8, (short)3, "Разработчик малых гр. кораблей" },
                    { 9, (short)3, "Разработчик больших гр. кораблей" },
                    { 10, (short)3, "Разработчик внутренних интерьеров" },
                    { 11, (short)3, "Дизайнер кораблей" },
                    { 12, (short)3, "Разработчик малых военных кораблей" },
                    { 13, (short)3, "Разработчик больших военных кораблей" },
                    { 14, (short)3, "Разработчик специальных систем" },
                    { 15, (short)3, "Разработчик наземной техники" },
                    { 16, (short)3, "Управляющий проектами" },
                    { 17, (short)3, "Интегратор скриптов" },
                    { 18, (short)3, "Интегратор чертежей" },
                    { 19, (short)4, "Пехотинец-диверсант" },
                    { 20, (short)4, "Наводчик турелей" },
                    { 21, (short)4, "Оператор специальных систем" },
                    { 22, (short)4, "Пилот истребителя / перехватчика / разрушителя" },
                    { 23, (short)4, "Пилот снайпера / САУ" },
                    { 24, (short)4, "Пилот катера / самбуки" },
                    { 25, (short)4, "Пилот штурмовика / монитора" },
                    { 26, (short)4, "Пилот канонерки" },
                    { 27, (short)4, "Пилот торпедоносца / эсминца" },
                    { 28, (short)4, "Пилот фрегата" },
                    { 29, (short)4, "Пилот корвета" },
                    { 30, (short)4, "Пилот броненосца / линкора" },
                    { 31, (short)4, "Командир звена" },
                    { 32, (short)4, "Командир флотилии" },
                    { 33, (short)5, "Дипломат" },
                    { 34, (short)5, "Агитатор" },
                    { 35, (short)5, "Монтажёр" },
                    { 36, (short)5, "Репортёр" },
                    { 37, (short)5, "Стример" },
                    { 38, (short)1, "Наставник" },
                    { 39, (short)1, "Ментор" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specialization",
                keyColumn: "Id",
                keyValue: 39);
        }
    }
}
