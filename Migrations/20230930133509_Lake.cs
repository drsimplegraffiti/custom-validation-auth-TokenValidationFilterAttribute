using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthFilterProj.Migrations
{
    /// <inheritdoc />
    public partial class Lake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Users_UserId1",
                table: "Apartments");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_UserId1",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Apartments");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Apartments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Amenities",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_UserId",
                table: "Apartments",
                column: "UserId");

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

            migrationBuilder.DropIndex(
                name: "IX_Apartments_UserId",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "Amenities",
                table: "Apartments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_UserId1",
                table: "Apartments",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Users_UserId1",
                table: "Apartments",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
