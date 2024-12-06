using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace CompanyAPI.Migrations
{
    public partial class AddWalletAndWalletHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                 name: "Wallets",
                 columns: table => new
                 {
                     Id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                         .Annotation("MySql:CharSet", "utf8mb4"),
                     Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                     ProviderAccountId = table.Column<int>(type: "int", nullable: false),
                     ConsumerAccountId = table.Column<int>(type: "int", nullable: false),
                     ExternalId = table.Column<int>(type: "int", nullable: false),
                     CreatedBy = table.Column<int>(type: "int", nullable: false),
                     UpdatedBy = table.Column<int>(type: "int", nullable: false),
                     CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                     UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Wallets", x => x.Id);
                 })
                 .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_ConsumerAccountId",
                table: "Wallets",
                column: "ConsumerAccountId",
                unique: true);

            migrationBuilder.CreateTable(
                name: "WalletsHistory",
                columns: table => new
                {
                    WalletId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                         .Annotation("MySql:CharSet", "utf8mb4"),
                    WalletName = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderAccountId = table.Column<int>(type: "int", nullable: false),
                    ConsumerAccountId = table.Column<int>(type: "int", nullable: false),
                    ExternalId = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletsHistory", x => new { x.WalletId, x.UpdatedAt });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
