using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnCurrencyToPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementTab_Advertisements_AdvertisementId",
                table: "AdvertisementTab");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdvertisementTab",
                table: "AdvertisementTab");

            migrationBuilder.RenameTable(
                name: "AdvertisementTab",
                newName: "AdvertisementTabs");

            migrationBuilder.RenameIndex(
                name: "IX_AdvertisementTab_AdvertisementId",
                table: "AdvertisementTabs",
                newName: "IX_AdvertisementTabs_AdvertisementId");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Plans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdvertisementTabs",
                table: "AdvertisementTabs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementTabs_Advertisements_AdvertisementId",
                table: "AdvertisementTabs",
                column: "AdvertisementId",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementTabs_Advertisements_AdvertisementId",
                table: "AdvertisementTabs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdvertisementTabs",
                table: "AdvertisementTabs");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Plans");

            migrationBuilder.RenameTable(
                name: "AdvertisementTabs",
                newName: "AdvertisementTab");

            migrationBuilder.RenameIndex(
                name: "IX_AdvertisementTabs_AdvertisementId",
                table: "AdvertisementTab",
                newName: "IX_AdvertisementTab_AdvertisementId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdvertisementTab",
                table: "AdvertisementTab",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementTab_Advertisements_AdvertisementId",
                table: "AdvertisementTab",
                column: "AdvertisementId",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
