using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyAPI.Migrations
{
    public partial class CreateCountriesRegionsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This showed up, might as well leave it...
            migrationBuilder.AlterColumn<string>(
                name: "HomepageUrl",
                table: "Organizations",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    GeoNameId = table.Column<int>(nullable: false),
                    CountryCode = table.Column<string>(maxLength: 5, nullable: false),
                    CountryName = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Countries", x => x.GeoNameId); });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    GeoNameId = table.Column<int>(nullable: false),
                    CountryGeoNameId = table.Column<int>(nullable: false),
                    RegionName = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Regions", x => new { x.GeoNameId, x.CountryGeoNameId }); });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_GeoNameId",
                table: "Countries",
                column: "GeoNameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_CountryGeoNameId",
                table: "Regions",
                column: "CountryGeoNameId");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_GeoNameId",
                table: "Regions",
                column: "GeoNameId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Regions");

            // This showed up, might as well leave it...
            migrationBuilder.AlterColumn<string>(
                name: "HomepageUrl",
                table: "Organizations",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 1024,
                oldNullable: true);
        }
    }
}