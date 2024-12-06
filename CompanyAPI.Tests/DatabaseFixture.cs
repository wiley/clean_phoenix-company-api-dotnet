using System;
using CompanyAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CompanyAPI.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public CompanyDbContext FixtureContext { get; private set; }

        public DatabaseFixture()
        {
            // Create an in-memory copy of the database context, and initialize it with some known values
            DbContextOptions<CompanyDbContext> options;
            var builder = new DbContextOptionsBuilder<CompanyDbContext>();
            builder.UseInMemoryDatabase("CompanyApiTest");
            options = builder.Options;

            FixtureContext = new CompanyDbContext(options);
            FixtureContext.SeedOrganizations();
            FixtureContext.SeedOrganizationRoles();
            FixtureContext.SeedOrganizationUserRoles();
        }

        public void Dispose()
        {
            FixtureContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}