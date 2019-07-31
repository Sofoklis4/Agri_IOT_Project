using Microsoft.EntityFrameworkCore.Migrations;

namespace AgriWebSite_v2.Data.Migrations
{
    public partial class AddRules2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeasurementId",
                table: "RulesForRelays",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RulesForRelays_MeasurementId",
                table: "RulesForRelays",
                column: "MeasurementId");

            migrationBuilder.AddForeignKey(
                name: "FK_RulesForRelays_Measurements_MeasurementId",
                table: "RulesForRelays",
                column: "MeasurementId",
                principalTable: "Measurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RulesForRelays_Measurements_MeasurementId",
                table: "RulesForRelays");

            migrationBuilder.DropIndex(
                name: "IX_RulesForRelays_MeasurementId",
                table: "RulesForRelays");

            migrationBuilder.DropColumn(
                name: "MeasurementId",
                table: "RulesForRelays");
        }
    }
}
