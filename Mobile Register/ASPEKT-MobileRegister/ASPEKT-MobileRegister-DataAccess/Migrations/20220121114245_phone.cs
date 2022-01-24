using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASPEKT_MobileRegister_DataAccess.Migrations
{
    public partial class phone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "UserTestMobile");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "UserTestMobile",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "UserTestMobile");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "UserTestMobile",
                type: "int",
                maxLength: 250,
                nullable: false,
                defaultValue: 0);
        }
    }
}
