using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorizationSessionServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationSessionService_AuthorizationSessions_AuthorizationSessionsId",
                table: "AuthorizationSessionService");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationSessionService_Services_ServicesId",
                table: "AuthorizationSessionService");

            migrationBuilder.RenameColumn(
                name: "ServicesId",
                table: "AuthorizationSessionService",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "AuthorizationSessionsId",
                table: "AuthorizationSessionService",
                newName: "AuthorizationSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorizationSessionService_ServicesId",
                table: "AuthorizationSessionService",
                newName: "IX_AuthorizationSessionService_ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizationSessionService_AuthorizationSessions_AuthorizationSessionId",
                table: "AuthorizationSessionService",
                column: "AuthorizationSessionId",
                principalTable: "AuthorizationSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizationSessionService_Services_ServiceId",
                table: "AuthorizationSessionService",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationSessionService_AuthorizationSessions_AuthorizationSessionId",
                table: "AuthorizationSessionService");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationSessionService_Services_ServiceId",
                table: "AuthorizationSessionService");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "AuthorizationSessionService",
                newName: "ServicesId");

            migrationBuilder.RenameColumn(
                name: "AuthorizationSessionId",
                table: "AuthorizationSessionService",
                newName: "AuthorizationSessionsId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorizationSessionService_ServiceId",
                table: "AuthorizationSessionService",
                newName: "IX_AuthorizationSessionService_ServicesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizationSessionService_AuthorizationSessions_AuthorizationSessionsId",
                table: "AuthorizationSessionService",
                column: "AuthorizationSessionsId",
                principalTable: "AuthorizationSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizationSessionService_Services_ServicesId",
                table: "AuthorizationSessionService",
                column: "ServicesId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
