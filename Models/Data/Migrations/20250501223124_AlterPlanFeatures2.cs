using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterPlanFeatures2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanFeatures_Plans_PlanId",
                table: "PlanFeatures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlanFeatures",
                table: "PlanFeatures");

            migrationBuilder.DropIndex(
                name: "IX_PlanFeatures_Key",
                table: "PlanFeatures");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PlanFeatures");

            migrationBuilder.AlterColumn<string>(
                name: "PlanId",
                table: "PlanFeatures",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "PlanFeatures",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlanFeatures",
                table: "PlanFeatures",
                columns: new[] { "Key", "PlanId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanFeatures_Key",
                table: "PlanFeatures",
                column: "Key");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanFeatures_Plans_PlanId",
                table: "PlanFeatures",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanFeatures_Plans_PlanId",
                table: "PlanFeatures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlanFeatures",
                table: "PlanFeatures");

            migrationBuilder.DropIndex(
                name: "IX_PlanFeatures_Key",
                table: "PlanFeatures");

            migrationBuilder.AlterColumn<string>(
                name: "PlanId",
                table: "PlanFeatures",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "PlanFeatures",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PlanFeatures",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlanFeatures",
                table: "PlanFeatures",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PlanFeatures_Key",
                table: "PlanFeatures",
                column: "Key",
                unique: true,
                filter: "[Key] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanFeatures_Plans_PlanId",
                table: "PlanFeatures",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id");
        }
    }
}
