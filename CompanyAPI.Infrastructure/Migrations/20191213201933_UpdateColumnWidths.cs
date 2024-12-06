using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyAPI.Migrations
{
    public partial class UpdateColumnWidths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Organizations",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Organizations",
                maxLength: 245,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Permalink",
                table: "Organizations",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationName",
                table: "Organizations",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "LogoUrl",
                table: "Organizations",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HomepageUrl",
                table: "Organizations",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Domain",
                table: "Organizations",
                maxLength: 245,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Organizations",
                maxLength: 245,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Organizations",
                maxLength: 245,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LocationName",
                table: "Locations",
                maxLength: 245,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "Departments",
                maxLength: 245,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Organizations",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Organizations",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 245,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Permalink",
                table: "Organizations",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationName",
                table: "Organizations",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "LogoUrl",
                table: "Organizations",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HomepageUrl",
                table: "Organizations",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Domain",
                table: "Organizations",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 245,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Organizations",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 245,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Organizations",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 245,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LocationName",
                table: "Locations",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 245);

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "Departments",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 245);
        }
    }
}