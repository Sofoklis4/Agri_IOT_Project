using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AgriWebSite_v2.Data.Migrations
{
    public partial class addrasb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RaspberryId",
                table: "Relays",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RaspberryId",
                table: "Measurements",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Raspberries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raspberries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Relays_RaspberryId",
                table: "Relays",
                column: "RaspberryId");

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_RaspberryId",
                table: "Measurements",
                column: "RaspberryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Raspberries_RaspberryId",
                table: "Measurements",
                column: "RaspberryId",
                principalTable: "Raspberries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Relays_Raspberries_RaspberryId",
                table: "Relays",
                column: "RaspberryId",
                principalTable: "Raspberries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Raspberries_RaspberryId",
                table: "Measurements");

            migrationBuilder.DropForeignKey(
                name: "FK_Relays_Raspberries_RaspberryId",
                table: "Relays");

            migrationBuilder.DropTable(
                name: "Raspberries");

            migrationBuilder.DropIndex(
                name: "IX_Relays_RaspberryId",
                table: "Relays");

            migrationBuilder.DropIndex(
                name: "IX_Measurements_RaspberryId",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "RaspberryId",
                table: "Relays");

            migrationBuilder.DropColumn(
                name: "RaspberryId",
                table: "Measurements");
        }
    }
}
