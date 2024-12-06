using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyAPI.Migrations
{
    public partial class RemoveCrunchbaseUuidUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organizations_CrunchbaseUuid",
                table: "Organizations");

            migrationBuilder.AlterColumn<string>(
                name: "HomepageUrl",
                table: "Organizations",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CrunchbaseUuid",
                table: "Organizations",
                column: "CrunchbaseUuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organizations_CrunchbaseUuid",
                table: "Organizations");

            migrationBuilder.AlterColumn<string>(
                name: "HomepageUrl",
                table: "Organizations",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CrunchbaseUuid",
                table: "Organizations",
                column: "CrunchbaseUuid",
                unique: true);
        }
    }
}