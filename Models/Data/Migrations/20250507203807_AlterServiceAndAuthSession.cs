using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterServiceAndAuthSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserServices");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UserServices",
                newName: "AssignedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UserModelAis",
                newName: "AssignedAt");

            migrationBuilder.AddColumn<string>(
                name: "BillingPeriod",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AuthorizationSessionService",
                columns: table => new
                {
                    AuthorizationSessionsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServicesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationSessionService", x => new { x.AuthorizationSessionsId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_AuthorizationSessionService_AuthorizationSessions_AuthorizationSessionsId",
                        column: x => x.AuthorizationSessionsId,
                        principalTable: "AuthorizationSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorizationSessionService_Services_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationSessionService_ServicesId",
                table: "AuthorizationSessionService",
                column: "ServicesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizationSessionService");

            migrationBuilder.DropColumn(
                name: "BillingPeriod",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                table: "UserServices",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                table: "UserModelAis",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserServices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
