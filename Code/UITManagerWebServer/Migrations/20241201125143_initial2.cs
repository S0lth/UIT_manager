using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UITManagerWebServer.Migrations
{
    /// <inheritdoc />
    public partial class initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeverityHistories_AspNetUsers_UserId",
                table: "SeverityHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_SeverityHistories_AspNetUsers_UserId",
                table: "SeverityHistories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeverityHistories_AspNetUsers_UserId",
                table: "SeverityHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_SeverityHistories_AspNetUsers_UserId",
                table: "SeverityHistories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
