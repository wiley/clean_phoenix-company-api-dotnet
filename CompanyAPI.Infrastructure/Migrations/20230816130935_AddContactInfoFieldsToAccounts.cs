using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyAPI.Migrations
{
    public partial class AddContactInfoFieldsToAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactInfo1",
                table: "Accounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo2",
                table: "Accounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo3",
                table: "Accounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo4",
                table: "Accounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo5",
                table: "Accounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo6",
                table: "Accounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo7",
                table: "Accounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactInfo1",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ContactInfo2",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ContactInfo3",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ContactInfo4",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ContactInfo5",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ContactInfo6",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ContactInfo7",
                table: "Accounts");
        }
    }
}
