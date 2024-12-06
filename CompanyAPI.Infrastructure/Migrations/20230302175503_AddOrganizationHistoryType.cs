using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyAPI.Migrations
{
    public partial class AddOrganizationHistoryType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationTypeId",
                table: "OrganizationsHistory",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizationTypeId",
                table: "OrganizationsHistory");
        }
    }
}
