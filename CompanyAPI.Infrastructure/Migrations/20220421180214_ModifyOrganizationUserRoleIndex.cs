using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyAPI.Migrations
{
    public partial class ModifyOrganizationUserRoleIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationUserRoles_OrganizationId",
                table: "OrganizationUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUserRoles_UserId",
                table: "OrganizationUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUserRoleHistory_OrganizationId",
                table: "OrganizationUserRoleHistory");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUserRoleHistory_UserId",
                table: "OrganizationUserRoleHistory");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserRoles_OrganizationId",
                table: "OrganizationUserRoles",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserRoles_UserId",
                table: "OrganizationUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserRoleHistory_OrganizationId",
                table: "OrganizationUserRoleHistory",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserRoleHistory_UserId",
                table: "OrganizationUserRoleHistory",
                column: "UserId");

            migrationBuilder.Sql("Update OrganizationRoles Set Name = 'org-admin' where OrganizationRoleId = 1;");
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationUserRoles_OrganizationId",
                table: "OrganizationUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUserRoles_UserId",
                table: "OrganizationUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUserRoleHistory_OrganizationId",
                table: "OrganizationUserRoleHistory");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUserRoleHistory_UserId",
                table: "OrganizationUserRoleHistory");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserRoles_OrganizationId",
                table: "OrganizationUserRoles",
                column: "OrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserRoles_UserId",
                table: "OrganizationUserRoles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserRoleHistory_OrganizationId",
                table: "OrganizationUserRoleHistory",
                column: "OrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserRoleHistory_UserId",
                table: "OrganizationUserRoleHistory",
                column: "UserId",
                unique: true);

            migrationBuilder.Sql("Update OrganizationRoles Set Name = 'OrganizationAdmin' where OrganizationRoleId = 1;");
        }
    }
}
