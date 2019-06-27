using Microsoft.EntityFrameworkCore.Migrations;

namespace AgriWebSite_v2.Data.Migrations
{
    public partial class Init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "value",
                table: "MeasurementsValues",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DownLevel",
                table: "Measurements",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "UpLevel",
                table: "Measurements",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "value",
                table: "MeasurementsValues");

            migrationBuilder.DropColumn(
                name: "DownLevel",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "UpLevel",
                table: "Measurements");
        }
    }
}
