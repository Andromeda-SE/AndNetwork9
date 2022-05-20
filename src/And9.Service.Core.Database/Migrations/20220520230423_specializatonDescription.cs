using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace And9.Service.Core.Database.Migrations
{
    public partial class specializatonDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MustApproved",
                schema: "Core",
                table: "Specializations");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Core",
                table: "Specializations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Core",
                table: "CandidateRequests",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Шахтеры занимаются разведкой и разработкой рудных жил. Для исполнения задач они используют ручные буры и специальные корабли.");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Задача монтажников — сборка блоков и базовая настройка. Также они используют свои навыки для извлечения компонентов со старых, поврежденных или трофейных блоков. Их типичные инструменты — сварочные аппараты и УШМ, установленные на корабле или выполненые в ручном варианте.");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Данная специализация объединяет пилотов кораблей, не предназначенных для прямого боестолкновения.");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Водители — операторы назменой техники. В их задачи входит поиск оптимальных маршрутов и учет оссобенностей каждого типа техники", "Водитель" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Управляющие следят за состоянием и коплектацией вереных станций, кораблей и иных машин.", "Управляющий пунктом базирования" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Direction", "Name" },
                values: new object[] { "Коменданты ведут учет пунктов базирования и флота. Даная специализация предполагает глубокие познания в ведении тылового обеспечения воюющей группы.", (short)2, "Комендант" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Скриптеры осуществляют автоматизацию объектов, создавая новые или модифицируя существующие скрипты, а также ведут документацию по созданным решениям.", "Скриптер" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Данная специализация предполагает участие в создании и совершенствовании инфраструктуры клана. В отличие от скриптеров, программисты обладают широкими знаниями в технологиях разработки ПО вне игры.", "Программист" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Разработчик малых гр. кораблей" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Разработчик больших гр. кораблей" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Разработчик внутренних интерьеров" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Дизайнер кораблей" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Разработчик малых военных кораблей" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Разработчик больших военных кораблей" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Разработчик специальных систем" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Разработчик наземной техники" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Управляющий проектами" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Интегратор скриптов" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Description", "Direction", "Name" },
                values: new object[] { "", (short)3, "Интегратор чертежей" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Пехотинец-диверсант" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Наводчик турелей" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Оператор специальных систем" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Пилот истребителя / перехватчика / разрушителя" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Пилот снайпера / САУ" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Пилот катера / самбуки" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Пилот штурмовика / монитора" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Пилот канонерки" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Пилот торпедоносца / эсминца" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Пилот фрегата" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Пилот корвета" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Пилот броненосца / линкора" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Description", "Name" },
                values: new object[] { "", "Командир звена" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Description", "Direction", "Name" },
                values: new object[] { "", (short)4, "Командир флотилии" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Дипломаты занимаются контактами с другими кланами, отдельными представителями сообщества и разработчиками игры", "Дипломат" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Основной задачей агитаторов является поиск игроков на общедоступных серверах и плоаках сообщества. Также они осущетвляют поддержку дипломатам в посике объеденений игроков.", "Агитатор" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Монтаж видео — процесс создания из исходных записей цельного видео. Монтажеры кроме обработки видео создают спецэффекты.", "Монтажёр" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Репортеры собирают информацию о просиходящем внутри клана и в сообществе игры, распростаняют информацию среди участников клана и иных заинтересованых лиц.", "Репортёр" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Description", "Direction", "Name" },
                values: new object[] { "Стримеры осуществляют прямые включения с мест событий, а также подготовку к ним", (short)5, "Стример" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "Description", "Direction", "Name" },
                values: new object[] { "Операторы видеозаписи записывают исходные видео дл дальнейшей обработки монтажером", (short)5, "Оператор видеозаписи" });

            migrationBuilder.InsertData(
                schema: "Core",
                table: "Specializations",
                columns: new[] { "Id", "Description", "Direction", "Name" },
                values: new object[,]
                {
                    { 40, "Мастера графики создают плакаты, заставки, обрабатывают скриншоты и подготавливают их для публикации", (short)5, "Мастер графики" },
                    { 41, "", (short)1, "Наставник" },
                    { 42, "", (short)1, "Ментор" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Core",
                table: "Specializations");

            migrationBuilder.AddColumn<bool>(
                name: "MustApproved",
                schema: "Core",
                table: "Specializations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Core",
                table: "CandidateRequests",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Управляющий станцией");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Комендант" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Direction", "Name" },
                values: new object[] { (short)3, "Скриптер" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Программист" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 8,
                column: "Name",
                value: "Разработчик малых гр. кораблей");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "Разработчик больших гр. кораблей");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 10,
                column: "Name",
                value: "Разработчик внутренних интерьеров");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Дизайнер кораблей" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Разработчик малых военных кораблей" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Разработчик больших военных кораблей" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Разработчик специальных систем" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 15,
                column: "Name",
                value: "Разработчик наземной техники");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Управляющий проектами" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 17,
                column: "Name",
                value: "Интегратор скриптов");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 18,
                column: "Name",
                value: "Интегратор чертежей");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Direction", "MustApproved", "Name" },
                values: new object[] { (short)4, true, "Пехотинец-диверсант" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 20,
                column: "Name",
                value: "Наводчик турелей");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Оператор специальных систем" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 22,
                column: "Name",
                value: "Пилот истребителя / перехватчика / разрушителя");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Пилот снайпера / САУ" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Пилот катера / самбуки" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Пилот штурмовика / монитора" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Пилот канонерки" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Пилот торпедоносца / эсминца" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Пилот фрегата" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 29,
                column: "Name",
                value: "Пилот корвета");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Пилот броненосца / линкора" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Командир звена" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Командир флотилии" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Direction", "MustApproved", "Name" },
                values: new object[] { (short)5, true, "Дипломат" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Агитатор" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "MustApproved", "Name" },
                values: new object[] { true, "Монтажёр" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 36,
                column: "Name",
                value: "Репортёр");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 37,
                column: "Name",
                value: "Стример");

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Direction", "MustApproved", "Name" },
                values: new object[] { (short)1, true, "Наставник" });

            migrationBuilder.UpdateData(
                schema: "Core",
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "Direction", "Name" },
                values: new object[] { (short)1, "Ментор" });
        }
    }
}
