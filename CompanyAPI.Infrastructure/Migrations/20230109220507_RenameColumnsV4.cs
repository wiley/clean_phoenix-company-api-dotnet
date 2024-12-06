using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyAPI.Migrations
{
    public partial class RenameColumnsV4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HistoryDt",
                table: "OrganizationUserRoleHistory",
                newName: "CreatedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "OrganizationUserRoleHistory",
                newName: "HistoryDt");
        }
    }
}
