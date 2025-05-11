using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterAuthorizationSessionServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationSessionService_AuthorizationSessions_AuthorizationSessionId",
                table: "AuthorizationSessionService");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationSessionService_Services_ServiceId",
                table: "AuthorizationSessionService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorizationSessionService",
                table: "AuthorizationSessionService");

            migrationBuilder.RenameTable(
                name: "AuthorizationSessionService",
                newName: "AuthorizationSessionServices");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorizationSessionService_ServiceId",
                table: "AuthorizationSessionServices",
                newName: "IX_AuthorizationSessionServices_ServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorizationSessionServices",
                table: "AuthorizationSessionServices",
                columns: new[] { "AuthorizationSessionId", "ServiceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizationSessionServices_AuthorizationSessions_AuthorizationSessionId",
                table: "AuthorizationSessionServices",
                column: "AuthorizationSessionId",
                principalTable: "AuthorizationSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizationSessionServices_Services_ServiceId",
                table: "AuthorizationSessionServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationSessionServices_AuthorizationSessions_AuthorizationSessionId",
                table: "AuthorizationSessionServices");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizationSessionServices_Services_ServiceId",
                table: "AuthorizationSessionServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorizationSessionServices",
                table: "AuthorizationSessionServices");

            migrationBuilder.RenameTable(
                name: "AuthorizationSessionServices",
                newName: "AuthorizationSessionService");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorizationSessionServices_ServiceId",
                table: "AuthorizationSessionService",
                newName: "IX_AuthorizationSessionService_ServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorizationSessionService",
                table: "AuthorizationSessionService",
                columns: new[] { "AuthorizationSessionId", "ServiceId" });

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
    }
}
