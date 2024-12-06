using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyAPI.Migrations
{
    public partial class UpdateWalletAccountsColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_Wallets_Id",
            //    table: "Wallets");


            migrationBuilder.AlterColumn<string>(
                name: "ProviderAccountId",
                table: "WalletsHistory",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerAccountId",
                table: "WalletsHistory",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderAccountId",
                table: "Wallets",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerAccountId",
                table: "Wallets",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");    

            //migrationBuilder.CreateIndex(
            //    name: "IX_Wallets_ConsumerAccountId",
            //    table: "Wallets",
            //    column: "ConsumerAccountId",
            //    unique: true);

          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_Wallets_ConsumerAccountId",
            //    table: "Wallets");

           migrationBuilder.AlterColumn<int>(
                name: "ProviderAccountId",
                table: "WalletsHistory",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ConsumerAccountId",
                table: "WalletsHistory",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ProviderAccountId",
                table: "Wallets",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ConsumerAccountId",
                table: "Wallets",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

           //migrationBuilder.CreateIndex(
           //     name: "IX_Wallets_Id",
           //     table: "Wallets",
           //     column: "Id",
           //     unique: true);
        }
    }
}
