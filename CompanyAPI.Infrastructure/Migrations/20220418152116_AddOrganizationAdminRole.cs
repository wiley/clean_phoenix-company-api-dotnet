using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyAPI.Migrations
{
    public partial class AddOrganizationAdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO OrganizationRoles(OrganizationRoleId, Name, CreatedAt, UpdatedAt) Values(1, 'OrganizationAdmin', now(), now());");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM OrganizationRoles WHERE OrganizationRoleId = 1;");
        }
    }
}
