using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthFilterProj.Migrations
{
    /// <inheritdoc />
    public partial class Lakedsds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Users_HostId",
                table: "Apartments");

            migrationBuilder.RenameColumn(
                name: "HostId",
                table: "Apartments",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Apartments_HostId",
                table: "Apartments",
                newName: "IX_Apartments_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Users_UserId",
                table: "Apartments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Users_UserId",
                table: "Apartments");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Apartments",
                newName: "HostId");

            migrationBuilder.RenameIndex(
                name: "IX_Apartments_UserId",
                table: "Apartments",
                newName: "IX_Apartments_HostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Users_HostId",
                table: "Apartments",
                column: "HostId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
