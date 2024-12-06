using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyAPI.Migrations
{
    public partial class UpdateOrganizationUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationUserRoles",
                table: "OrganizationUserRoles");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OrganizationUserRoles",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationUserRoles",
                table: "OrganizationUserRoles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserRoles_OrganizationId_UserId_OrganizationRole~",
                table: "OrganizationUserRoles",
                columns: new[] { "OrganizationId", "UserId", "OrganizationRoleId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationUserRoles",
                table: "OrganizationUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUserRoles_OrganizationId_UserId_OrganizationRole~",
                table: "OrganizationUserRoles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrganizationUserRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationUserRoles",
                table: "OrganizationUserRoles",
                columns: new[] { "OrganizationId", "UserId", "OrganizationRoleId" });
        }
    }
}
