using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace And9.Service.Award.Database.Migrations;

[DbContext(typeof(AwardDataContext))]
[Migration("20220509185000_Awards.Quartz")]
public class QuartzMigration : Migration
{
    private const string _PATH_TO_UP_SCRIPT = "Migrations/Scripts/qrtz_tables_postgres_up.sql";
    private const string _PATH_TO_DOWN_SCRIPT = "Migrations/Scripts/qrtz_tables_postgres_down.sql";
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(File.ReadAllText(_PATH_TO_UP_SCRIPT));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(File.ReadAllText(_PATH_TO_DOWN_SCRIPT));
    }
}