using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyAPI.Migrations
{
    public partial class AddRegionsForCountriesWithNoRegions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Regions (GeoNameID, CountryGeoNameID, RegionName, CreatedAt, UpdatedAt) VALUES (-3164670, 3164670, 'Vatican City', NOW(), NOW()) ON DUPLICATE KEY UPDATE GeoNameID=GeoNameID;");
            migrationBuilder.Sql("INSERT INTO Regions (GeoNameID, CountryGeoNameID, RegionName, CreatedAt, UpdatedAt) VALUES (-1880251, 1880251, 'Singapore', NOW(), NOW()) ON DUPLICATE KEY UPDATE GeoNameID=GeoNameID;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Regions WHERE GeoNameID = -3164670;");
            migrationBuilder.Sql("DELETE FROM Regions WHERE GeoNameID = -1880251;");
        }
    }
}