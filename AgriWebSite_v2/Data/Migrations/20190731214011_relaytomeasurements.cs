using Microsoft.EntityFrameworkCore.Migrations;

namespace AgriWebSite_v2.Data.Migrations
{
    public partial class relaytomeasurements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelayId",
                table: "Measurements",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_RelayId",
                table: "Measurements",
                column: "RelayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Relays_RelayId",
                table: "Measurements",
                column: "RelayId",
                principalTable: "Relays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Relays_RelayId",
                table: "Measurements");

            migrationBuilder.DropIndex(
                name: "IX_Measurements_RelayId",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "RelayId",
                table: "Measurements");
        }
    }
}
