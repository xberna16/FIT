using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamCommunication.DAL.Migrations
{
    public partial class Login : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "salt",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "salt",
                table: "Users");
        }
    }
}
