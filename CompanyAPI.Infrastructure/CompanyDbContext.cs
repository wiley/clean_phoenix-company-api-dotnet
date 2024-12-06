using CompanyAPI.Domain;
using CompanyAPI.Domain.Exceptions;
using CompanyAPI.Infrastructure.Interface;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompanyAPI.Infrastructure
{
    public class CompanyDbContext : DbContext, ICompanyDbContext
    {
        public CompanyDbContext() { }

        public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options) { }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<OrganizationHistory> OrganizationsHistory { get; set; }

        public DbSet<OrganizationRole> OrganizationRoles { get; set; }

        public DbSet<OrganizationUserRole> OrganizationUserRoles { get; set; }

        public DbSet<OrganizationUserRoleHistory> OrganizationUserRoleHistory { get; set; }


        // This is a query only model
        public DbSet<OrganizationRolesResponse> UserOrganizations { get; set; }
        
        public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<CompanyDbContext>
        {
            // Solution for "InvalidOperationException: No database provider has been configured for this DbContext."
            // https://stackoverflow.com/questions/38338475/
            public CompanyDbContext CreateDbContext(string[] args)
            {
                //Enter the connection string below.
                var connectionString = "server=localhost;port=3306;user id=root;password=passion4excellence;database=CompanyAPI"; //Environment.GetEnvironmentVariable("COMPANYAPI_CONNECTION_STRING");

                var optionsBuilder = new DbContextOptionsBuilder<CompanyDbContext>()
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                return new CompanyDbContext(optionsBuilder.Options);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // See https://stackoverflow.com/questions/38338475
            // Set Auto Incremental https://stackoverflow.com/questions/49592274
            // Set default datetime value = "CURRENT_TIMESTAMP" (doesn't work?!)
            //	https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/35
            //	https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/958
            base.OnModelCreating(modelBuilder);

            // Organizations table
            modelBuilder.Entity<Organization>()
                .HasKey(o => new { o.OrganizationId });
            modelBuilder.Entity<Organization>()
                .HasIndex(o => new { o.OrganizationId })
                .IsUnique();
            modelBuilder.Entity<Organization>()
                .Property(o => o.OrganizationId)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Organization>()
                .Property(o => o.OrganizationTypeId)
                .HasDefaultValueSql("1");
            modelBuilder.Entity<Organization>()
                .HasIndex(o => new { o.CrunchbaseUuid });

            // OrganizationsHistory table
            modelBuilder.Entity<OrganizationHistory>()
                .HasKey(o => new { o.OrganizationId, o.UpdatedAt });

            // OrganizationRoles table
            modelBuilder.Entity<OrganizationRole>()
                .HasKey(o => new { o.OrganizationRoleId });
            modelBuilder.Entity<OrganizationRole>()
                .HasIndex(o => new { o.OrganizationRoleId })
                .IsUnique();
            modelBuilder.Entity<OrganizationRole>()
                .Property(o => o.OrganizationRoleId)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<OrganizationRole>()
                .Property(o => o.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            // OrganizationUserRoles table
            modelBuilder.Entity<OrganizationUserRole>()
                .HasKey(o => new { o.Id });
            modelBuilder.Entity<OrganizationUserRole>()
                .HasIndex(o => new { o.OrganizationId, o.UserId, o.OrganizationRoleId })
                .IsUnique();
            modelBuilder.Entity<OrganizationUserRole>()
                .HasIndex(o => new { o.OrganizationId });
            modelBuilder.Entity<OrganizationUserRole>()
                .HasIndex(o => new { o.UserId });
            modelBuilder.Entity<OrganizationUserRole>()
                .Property(o => o.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            // OrganizationUserRoleHistory table
            modelBuilder.Entity<OrganizationUserRoleHistory>()
                .HasKey(o => new { o.OrganizationUserRoleHistoryId });
            modelBuilder.Entity<OrganizationUserRoleHistory>()
                .HasIndex(o => new { o.OrganizationUserRoleHistoryId })
                .IsUnique();
            modelBuilder.Entity<OrganizationUserRoleHistory>()
                .HasIndex(o => new { o.OrganizationId });
            modelBuilder.Entity<OrganizationUserRoleHistory>()
                .HasIndex(o => new { o.UserId });
            modelBuilder.Entity<OrganizationUserRoleHistory>()
                .Property(o => o.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<OrganizationUserRoleHistory>()
                .Property(o => o.WasDeleted)
                .HasColumnType("bit");

            modelBuilder.Entity<OrganizationRolesResponse>()
                .HasNoKey()
                .Ignore(t => t.Roles)
                .ToTable("UserOrganizations", t => t.ExcludeFromMigrations(true));
        }

        public void Initialize()
        {
            Database.Migrate();
		}

        #region Migration Functions

        private bool MigrationHasRun(string migrationId)
        {
            try
            {
                Database.OpenConnection();
                using var cmd = Database.GetDbConnection().CreateCommand();
                cmd.CommandText = @$"SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '{migrationId}';";
                object value = cmd.ExecuteScalar();
                return value != null && (long)value == 1;
            }
            finally
            {
                Database.CloseConnection();
            }
        }

        private int DropFullTextIndex(string tableName, string indexName)
        {
            string sql = $"ALTER TABLE `{tableName}` DROP INDEX `{indexName}`;";
            return Database.ExecuteSqlRaw(sql);
        }

        private int AddFullTextIndex(string tableName, string indexName, string columnName)
        {
            string sql = $"CREATE FULLTEXT INDEX `{indexName}` ON {tableName}(`{columnName}`);";
            return Database.ExecuteSqlRaw(sql);
        }

        #endregion

        #region Seed Data

        public async void SeedOrganizations()
        {
            // Add Organizations
            #region WLS (Learner) Organizations

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                CrunchbaseUuid = Guid.Parse("caa9fae4-492a-d3c2-0aed-351ded4213bd"),
                OrganizationName = "Organization Updated 500",
                Permalink = "wiley",
                Domain = "wiley.com",
                HomepageUrl = "http://wiley.com",
                LogoUrl = "https://crunchbase-production-res.cloudinary.com/image/upload/c_lpad,h_120,w_120,f_jpg/v1416112216/csozpaeukqh1seoio0qx.png",
                City = "Hoboken",
                Region = "New Jersey",
                Country = "US",
                ShortDescription = "We are the EMEA Global Education division of global publishers John Wiley and Sons.",
                OrganizationId = 500
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Talent,
                CrunchbaseUuid = Guid.Parse("caa9fae4-492a-d3c2-0aed-351ded4213bd"),
                OrganizationName = "Organization Updated 501",
                Permalink = "wiley",
                Domain = "wiley.com",
                HomepageUrl = "http://wiley.com",
                LogoUrl = "https://crunchbase-production-res.cloudinary.com/image/upload/c_lpad,h_120,w_120,f_jpg/v1416112216/csozpaeukqh1seoio0qx.png",
                City = "Hoboken",
                Region = "New Jersey",
                Country = "US",
                ShortDescription = "We are the EMEA Global Education division of global publishers John Wiley and Sons.",
                OrganizationId = 501
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Talent,
                CrunchbaseUuid = Guid.NewGuid(),
                OrganizationName = "AccontOrg Test 1",
                Permalink = "",
                Domain = "",
                HomepageUrl = "",
                LogoUrl = "LogoUrl",
                City = "A City",
                Region = "",
                Country = "",
                ShortDescription = "",
                OrganizationId = 502
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                CrunchbaseUuid = Guid.Parse("caa9fae4-492a-d3c2-0aed-351ded4213bd"),
                OrganizationName = "John Wiley & Sons",
                Permalink = "wiley",
                Domain = "wiley.com",
                HomepageUrl = "http://wiley.com",
                LogoUrl = "https://crunchbase-production-res.cloudinary.com/image/upload/c_lpad,h_120,w_120,f_jpg/v1416112216/csozpaeukqh1seoio0qx.png",
                City = "Hoboken",
                Region = "New Jersey",
                Country = "US",
                ShortDescription = "We are the EMEA Global Education division of global publishers John Wiley and Sons."
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                CrunchbaseUuid = Guid.Parse("e96d3a67-d899-612d-0047-63a005425e1f"),
                OrganizationName = "Williams-Sonoma",
                Permalink = "williams-sonoma",
                Domain = "williams-sonomainc.com",
                HomepageUrl = "http://www.williams-sonomainc.com/",
                LogoUrl = "https://crunchbase-production-res.cloudinary.com/image/upload/c_lpad,h_120,w_120,f_jpg/v1410848162/d8eaqsodrnm8d7xvldns.jpg",
                City = "San Francisco",
                Region = "California",
                Country = "US",
                ShortDescription = "Williams-Sonoma is a retailer of home furnishings and gourmet cookware that offers products for every room in the house.",
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                CrunchbaseUuid = Guid.Parse("5dd4a432-bc19-49ea-a61f-fe78e250cec1"),
                OrganizationName = "J.D. Williams and Company",
                Permalink = "j-d-williams-and-company",
                Domain = "jdwilliams.co.uk",
                HomepageUrl = "http://www.jdwilliams.co.uk",
                LogoUrl = "https://crunchbase-production-res.cloudinary.com/image/upload/c_lpad,h_120,w_120,f_jpg/ncw8gwjkuzvctxzkrzdy",
                City = "Manchester",
                Region = "Manchester",
                Country = "GB",
                ShortDescription = "J.D. Williams retails apparel, footwear, and accessories for men, women, and children in the United Kingdom.",
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                CrunchbaseUuid = Guid.Parse("6acfa7da-1dbd-936e-d985-cf07a1b27711"),
                OrganizationName = "Google",
                Permalink = "google",
                Domain = "google.com",
                HomepageUrl = "http://www.google.com/",
                LogoUrl = "https://crunchbase-production-res.cloudinary.com/image/upload/c_lpad,h_120,w_120,f_jpg/fa8nmvofinznny6rkwvf",
                City = "Mountain View",
                Region = "California",
                Country = "US",
                ShortDescription = "Google is a multinational corporation that specializes in Internet-related services and products."
            }, true, false);

            //Custom Test Organization
            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                CrunchbaseUuid = Guid.Parse("00000000-0000-0000-0000-000000000000"), //CrunchbasUuid empty for all custom test organizations
                OrganizationName = "Test Company",
                Permalink = "", //Remaining content is going to be empty for now
                Domain = "testcompany.com",
                HomepageUrl = "", //Remaining content is going to be empty for now
                LogoUrl = "", //Remaining content is going to be empty for now
                City = "", //Remaining content is going to be empty for now
                Region = "", //Remaining content is going to be empty for now
                Country = "", //Remaining content is going to be empty for now
                ShortDescription = "" //Remaining content is going to be empty for now
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                OrganizationName = "Root Beer Example Organization",
                Domain = "rootbeer.example.com"
            }, true, false);

            for (int i = 0; i < 255; i++)
            {
                //Add a bunch of organizations so that can verify only 250 of the 255 are returned. 
                await AddOrganization(new Organization()
                {
                    OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                    CrunchbaseUuid = Guid.Parse("00000000-0000-0000-0000-000000000000"), //CrunchbasUuid empty for all custom test organizations
                    OrganizationName = "Test Count " + i,
                    Permalink = "", //Remaining content is going to be empty for now
                    Domain = null, // No longer updating Domain
                    HomepageUrl = "", //Remaining content is going to be empty for now
                    LogoUrl = "", //Remaining content is going to be empty for now
                    City = "Anytown",
                    Region = "", //Remaining content is going to be empty for now
                    Country = "", //Remaining content is going to be empty for now
                    ShortDescription = "" //Remaining content is going to be empty for now
                }, true, false);
            }

            //This Org should have no users
            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                CrunchbaseUuid = Guid.Parse("5dd4a432-bc19-49ea-a61f-fe78e250cec2"),
                OrganizationName = "NoUsers",
                Permalink = "",
                Domain = "",
                HomepageUrl = "",
                LogoUrl = "LogoUrl",
                City = "A City",
                Region = "",
                Country = "",
                ShortDescription = ""
            }, true, false);

            //This Org used for the Merge
            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                CrunchbaseUuid = Guid.NewGuid(),
                OrganizationName = "MergeOrg1",
                Permalink = "",
                Domain = "",
                HomepageUrl = "",
                LogoUrl = "LogoUrl",
                City = "A City",
                Region = "",
                Country = "",
                ShortDescription = ""
            }, true, false);

            //This Org should have no users
            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                CrunchbaseUuid = Guid.NewGuid(),
                OrganizationName = "MergeOrg2",
                Permalink = "",
                Domain = "",
                HomepageUrl = "",
                LogoUrl = "LogoUrl",
                City = "A City",
                Region = "",
                Country = "",
                ShortDescription = ""
            }, true, false);

            //This Org should have no users
            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Talent,
                CrunchbaseUuid = Guid.NewGuid(),
                OrganizationName = "MergeOrg3",
                Permalink = "",
                Domain = "",
                HomepageUrl = "",
                LogoUrl = "LogoUrl",
                City = "A City",
                Region = "",
                Country = "",
                ShortDescription = ""
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Talent,
                CrunchbaseUuid = Guid.NewGuid(),
                OrganizationName = "AccontOrgV4 Test 1",
                Permalink = "",
                Domain = "",
                HomepageUrl = "",
                LogoUrl = "LogoUrl",
                City = "A City",
                Region = "",
                Country = "",
                ShortDescription = ""
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Talent,
                CrunchbaseUuid = Guid.NewGuid(),
                OrganizationName = "AccontOrgV4 Test 2",
                Permalink = "",
                Domain = "",
                HomepageUrl = "",
                LogoUrl = "LogoUrl",
                City = "A City",
                Region = "",
                Country = "",
                ShortDescription = ""
            }, true, false);
            #endregion

            #region CK (Talent) Organizations

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Talent,
                OrganizationName = "CK Test Organization 1",
                LogoUrl = "https://crunchbase-production-res.cloudinary.com/image/upload/c_lpad,h_120,w_120,f_jpg/axdcjjmvhcnesadsjr6f",
                City = "Suresnes",
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Talent,
                OrganizationName = "CK Test Organization 2",
                LogoUrl = "https://www.crossknowledge.com/wp-content/uploads/cache/2021/11/cropped-ck-ellipsis/261722991.png",
                City = "Nice",
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Talent,
                OrganizationName = "CK Test Organization 3",
                LogoUrl = "https://www.crossknowledge.com/wp-content/uploads/cache/2021/11/cropped-ck-ellipsis/2274907095.png",
                City = "Normandy",
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Talent,
                OrganizationName = "CK Test Organization 4",
                LogoUrl = "https://www.crossknowledge.com/wp-content/uploads/cache/2021/11/cropped-ck-ellipsis/2274907095.png",
                City = "Paris",
            }, true, false);

            await AddOrganization(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Talent,
                OrganizationName = "CK Test Organization 5",
                LogoUrl = "",
                City = "Paris",
            }, true, false);

            #endregion

            SaveChanges();
        }

        public async void SeedOrganizationRoles()
        {
            await AddOrganizationRole(new OrganizationRole
            {
                OrganizationRoleId = 1,
                Name = "org-admin",
                CreatedAt = new DateTime(2022, 1, 1, 0, 0, 0),
                UpdatedAt = new DateTime(2022, 1, 1, 0, 0, 0)
            });

            await AddOrganizationRole(new OrganizationRole
            {
                OrganizationRoleId = 2,
                Name = "Learner",
                CreatedAt = new DateTime(2022, 1, 1, 0, 0, 0),
                UpdatedAt = new DateTime(2022, 1, 1, 0, 0, 0)
            });

            SaveChanges();
        }

        public async void SeedOrganizationUserRoles()
        {
            #region WLS (Learner) Organization User Roles

            await AddOrganizationUserRole(new OrganizationUserRole
            {
                OrganizationId = 1,
                UserId = 1,
                OrganizationRoleId = 1,
                GrantedByUserId = 500,
                CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
            });

            await AddOrganizationUserRole(new OrganizationUserRole
            {
                OrganizationId = 1,
                UserId = 1,
                OrganizationRoleId = 2,
                GrantedByUserId = 500,
                CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
            });

            await AddOrganizationUserRole(new OrganizationUserRole
            {
                OrganizationId = 1,
                UserId = 2,
                OrganizationRoleId = 2,
                GrantedByUserId = 500,
                CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
            });

            await AddOrganizationUserRole(new OrganizationUserRole
            {
                OrganizationId = 3,
                UserId = 3,
                OrganizationRoleId = 1,
                GrantedByUserId = 500,
                CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
            });

            await AddOrganizationUserRole(new OrganizationUserRole
            {
                OrganizationId = 4,
                UserId = 3,
                OrganizationRoleId = 1,
                GrantedByUserId = 500,
                CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
            });

            //Adding a bunch of organization for user Id 4
            for (int i = 0; i < 255; i++)
            {
                await AddOrganizationUserRole(new OrganizationUserRole
                {
                    OrganizationId = i + 1,
                    UserId = 4,
                    OrganizationRoleId = 1,
                    GrantedByUserId = 500,
                    CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
                });
            }

            var orgmerge1 = Organizations.FirstOrDefault(x => x.OrganizationName == "MergeOrg1");
            await AddOrganizationUserRole(new OrganizationUserRole
            {
                OrganizationId = orgmerge1.OrganizationId,
                UserId = 1100,
                OrganizationRoleId = 1,
                GrantedByUserId = 500,
                CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
            });

            var orgmerge2 = Organizations.FirstOrDefault(x => x.OrganizationName == "MergeOrg2");
            await AddOrganizationUserRole(new OrganizationUserRole
            {
                OrganizationId = orgmerge2.OrganizationId,
                UserId = 1100,
                OrganizationRoleId = 1,
                GrantedByUserId = 500,
                CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
            });

            var orgmerge3 = Organizations.FirstOrDefault(x => x.OrganizationName == "MergeOrg3");
            await AddOrganizationUserRole(new OrganizationUserRole
            {
                OrganizationId = orgmerge3.OrganizationId,
                UserId = 1100,
                OrganizationRoleId = 1,
                GrantedByUserId = 500,
                CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
            });

            #endregion

            #region CK (Talent) Organization User Roles

            await AddOrganizationUserRole(new OrganizationUserRole
            {
                OrganizationId = Organizations.FirstOrDefault(o => o.OrganizationTypeId == (int)OrganizationTypeEnum.Talent
                    && o.OrganizationName == "CK Test Organization 1").OrganizationId,
                UserId = 2500,
                OrganizationRoleId = (int)OrganizationRoleEnum.OrgAdmin,
                GrantedByUserId = 500,
                CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
            });

            await AddOrganizationUserRole(new OrganizationUserRole
            {
                OrganizationId = Organizations.FirstOrDefault(o => o.OrganizationTypeId == (int)OrganizationTypeEnum.Talent
                    && o.OrganizationName == "CK Test Organization 2").OrganizationId,
                UserId = 2500,
                OrganizationRoleId = (int)OrganizationRoleEnum.Facilitator,
                GrantedByUserId = 500,
                CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
            });

            await AddOrganizationUserRole(new OrganizationUserRole
            {
                OrganizationId = Organizations.FirstOrDefault(o => o.OrganizationTypeId == (int)OrganizationTypeEnum.Talent
                    && o.OrganizationName == "CK Test Organization 3").OrganizationId,
                UserId = 2500,
                OrganizationRoleId = (int)OrganizationRoleEnum.Learner,
                GrantedByUserId = 500,
                CreatedAt = new DateTime(2022, 2, 1, 0, 0, 0)
            });

            #endregion

            SaveChanges();
        }
        #endregion

        #region Organizations

        public async Task<SaveOrganizationResult> AddOrganization(Organization organization, bool checkIfExists = true, bool useFullText = true)
        {
            try
            {
                if (!Enum.IsDefined(typeof(OrganizationTypeEnum), organization.OrganizationTypeId))
                {
                    organization.OrganizationTypeId = (int)OrganizationTypeEnum.Learner;
                }

                if (checkIfExists) // Only false when importing from CSV and the table is empty
                {
                    var request = new OrganizationFindRequest
                    {
                        OrganizationName = organization.OrganizationName,
                        City = organization.City,
                        OrganizationType = organization.OrganizationTypeId.ToString()
                    };

                    // Prevent duplicates (matching Name and City)
                    var duplicate = FindOrganization(request, useFullText);

                    if (duplicate == null)
                    {
                        if (organization.CrunchbaseUuid != Guid.Empty) // Crunchbase record - verify UUID is unique
                            if (await Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.CrunchbaseUuid.Equals(organization.CrunchbaseUuid)) != null)
                                return SaveOrganizationResult.ErrorDuplicate;
                    }
                    else
                    {
                        // DEBUGGING ONLY - Log duplicates as tab-separated values
                        //if (System.Diagnostics.Debugger.IsAttached &&
                        //	((organization.Domain != null && organization.Domain.Length > 0 && organization.Domain == duplicate.Domain) ||
                        //	(organization.HomepageUrl != null && organization.HomepageUrl.Length > 0 && organization.HomepageUrl == duplicate.HomepageUrl)))
                        //{
                        //	LogHelper.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11:yyyy-MM-dd hh:mm:ss}\t{12:yyyy-MM-dd hh:mm:ss}",
                        //		duplicate.OrganizationId, duplicate.CrunchbaseUuid, duplicate.OrganizationName, duplicate.Permalink,
                        //		duplicate.ShortDescription.Replace("\n", " ").Replace("\t", " "), duplicate.LogoUrl, duplicate.Domain, duplicate.HomepageUrl,
                        //		duplicate.City, duplicate.Region, duplicate.Country, duplicate.CreatedAt, duplicate.UpdatedAt), LogHelper.TimeFormat.None);
                        //	LogHelper.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11:yyyy-MM-dd hh:mm:ss}\t{12:yyyy-MM-dd hh:mm:ss}",
                        //		organization.OrganizationId, organization.CrunchbaseUuid, organization.OrganizationName, organization.Permalink,
                        //		organization.ShortDescription.Replace("\n", " ").Replace("\t", " "), organization.LogoUrl, organization.Domain, organization.HomepageUrl,
                        //		organization.City, organization.Region, organization.Country, organization.CreatedAt, organization.UpdatedAt), LogHelper.TimeFormat.None);
                        //}
                        return SaveOrganizationResult.ErrorDuplicate;
                    }
                }

                try
                {
                    await Organizations.AddAsync(organization);
                    return SaveOrganizationResult.Success;
                }
                catch
                {
                    return SaveOrganizationResult.ErrorDatabase;
                }
            }
            catch
            {
                return SaveOrganizationResult.ErrorUnknown;
            }
        }

        public Organization FindOrganization(OrganizationFindRequest request, bool useFullText = true)
        {
            Organization organization;

            int type = 1;
            if (request.OrganizationType != null)
            {
                if (Enum.TryParse<OrganizationTypeEnum>(request.OrganizationType, true, out OrganizationTypeEnum organizationTypeEnum))
                    type = Convert.ToInt32(organizationTypeEnum);
                else
                    return null;
            }

            try
            {
                if (useFullText)
                {
                    organization = Organizations.FromSqlRaw(
                            $"SELECT * FROM Organizations " +
                            $"WHERE OrganizationTypeId = {type} " +
                            $"AND MATCH(OrganizationName) AGAINST (@p0 IN BOOLEAN MODE) " +
                            $"AND LCASE(OrganizationName) = CONVERT(LCASE(@p1) USING utf8mb4) COLLATE utf8mb4_bin " +
                            $"AND LCASE(IFNULL(City, '')) = CONVERT(LCASE(@p2) USING utf8mb4) COLLATE utf8mb4_bin",
                            "\"" + request.OrganizationName + "\"", request.OrganizationName, request.City ?? "")
                        .AsNoTracking().FirstOrDefault();
                }
                else
                {
                    // Unit tests cannot use "FromSql" against an in-memory database

                    // Why was this broken into two queries?
                    //var organizations = Organizations.AsNoTracking().Where(
                    //	o => o.OrganizationTypeId.Equals((int)request.OrganizationType) && o.OrganizationName.Equals(request.OrganizationName) &&
                    //		 o.City.Equals(request.City));
                    //organization = organizations.FirstOrDefault(o => o.OrganizationName.Equals(request.OrganizationName, StringComparison.OrdinalIgnoreCase) &&
                    //	o.City.Equals(request.City, StringComparison.OrdinalIgnoreCase));

                    // Can be simplified as...
                    organization = Organizations.AsNoTracking().FirstOrDefault(
                        o => o.OrganizationTypeId.Equals(type) &&
                             o.OrganizationName.Equals(request.OrganizationName, StringComparison.OrdinalIgnoreCase) &&
                             o.City.Equals(request.City, StringComparison.OrdinalIgnoreCase));
                }

                return organization;
            }
            catch (Exception ex)
            {
                if (ex is NotSupportedException && useFullText && ex.ToString().Contains("is currently not supported."))
                    return FindOrganization(request, false); // Try with useFullText=false
                else
                    return null;
            }
        }

        private static string FormatKeywords(string value)
        {
            // MySQL expects one or more keywords separted by spaces
            // + before a keyword means it's required (AND)
            // nothing before a keyword means optional (OR) but improves the score of the response
            // * after a keyword means a wildcard
            // Other characters before a keyword we're not using:
            // - NOT including this word
            // < reduce contribution to score
            // > increase contribution to score
            // ~ Negative contribution to score (possibly used for noise words)
            // "word1 word2" used to mark a phrase
            // "word1 word2" @8 the two words are within 8 words of each other
            // (word1 word2) use for grouping.  (An example I found said "(word1 word2) (word1* word2*)" so it would give higher relevance to exact matches on words because it would double count the match, but it would still allow a partial match

            // NASA-80 - Remove user input of any special characters we can't search for
            // Get rid of chars that will interfere with SQL
            // NASA-1248 - Replace "&" with " & ", and turn off Stopwords (my.ini: innodb_ft_enable_stopword = f)
            string temp = value.Replace("+", "").Replace("*", "").Replace("-", " ").Replace("<", "").Replace(">", "")
                .Replace("~", "").Replace("\"", "").Replace("(", "").Replace(")", "").Replace("&", " & ");
            temp = Regex.Replace(temp, @"\@\d+", "").Replace("@", "");

            // Remove multiple spaces and replace with one
            temp = temp.Trim(); //leading and trailing spaces are not helpful
            temp = Regex.Replace(temp, @"\s+", " ");

            //Convert into words
            string[] words = temp.Split(" ");

            for (int index = 0; index < words.Length; index++)
            {
                words[index] = "+" + words[index] + "*"; //require each word and allow for partial matches
            }

            string formatted = string.Join(' ', words);

            return formatted;
        }

        public async Task<Organization> GetOrganization(int organizationId)
        {
            return await Organizations.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == organizationId);
        }

        public bool DeleteOrganization(int organizationID, int updatedBy = -1, bool fromMerge = false)
        {

            //If UpdatedBy is passed, verify that user is org admin and can delete organizations
            if (updatedBy > 0 && !OrganizationUserRoles.Any(x => x.OrganizationId == organizationID && x.OrganizationRoleId == (int)OrganizationRoleEnum.OrgAdmin && x.UserId == updatedBy))
            {
                throw new ForbiddenException($"User {updatedBy} does not have permission to delete organization {organizationID}");
            }
            //remove any OrganizationUserRole records and write history for them before deleting the organization. 
            var updateTime = DateTime.Now;
            var userRoles = OrganizationUserRoles.Where(x => x.OrganizationId == organizationID).ToList();

            // (NASA-1809) Allow deleting custom organizations
            // TODO: Per Wiley/HTTP specs API routes should be idempotent, i.e. identical requests should have identical results.
            var organization = Organizations.FirstOrDefault(x => x.OrganizationId == organizationID && (fromMerge || x.CrunchbaseUuid == Guid.Empty));
            if (organization == null)
                throw new NotFoundException();

            userRoles.ForEach(userRole =>
            {
                //save organizationUserRole history record
                OrganizationUserRoleHistory orgRolesHistory = Domain.OrganizationUserRoleHistory.CreateOrganizationUserRoleHistory(userRole, true);
                orgRolesHistory.ChangedByUserId = updatedBy;
                OrganizationUserRoleHistory.Add(orgRolesHistory);

                //delete the role
                OrganizationUserRoles.Remove(userRole);
            });

            //save organization history record
            OrganizationHistory organizationHistory = OrganizationHistory.CreateOrganizationHistory(organization);
            organizationHistory.OrganizationName = "Deleted";
            organizationHistory.UpdatedBy = updatedBy; //pull from delete request when updated
            OrganizationsHistory.Add(organizationHistory);

            //Remove the organization
            Organizations.Remove(organization);
            SaveChanges();


            //TODO: We need to come up with a Kafka messaging process to let other systems know of a delete. 
            return true;
        }

        #endregion

        #region OrganizationV4

        public IEnumerable<Organization> FindOrganizationV4(OrganizationV4Find request, bool useFullText, out int count)
        {
            count = 0;
            try
            {
                IQueryable<Organization> organizations;
                IList<Organization> organizationsList = null;

                if (useFullText && !string.IsNullOrEmpty(request.OrganizationName))
                {
                    string where = "";

                    if (request.City != null) where += " AND o.City = @p3 ";
                    if (request.OrganizationType > 0) where += " AND o.OrganizationTypeId = @p4 ";

                    count = Organizations.FromSqlRaw(
                        $"SELECT * " +
                        $"FROM Organizations as o " +
                        $"WHERE MATCH(o.OrganizationName) AGAINST (@p0 IN BOOLEAN MODE)" +
                        where +
                        $"ORDER BY MATCH(o.OrganizationName) AGAINST (@p0) DESC ",
                        request.OrganizationName, request.size, request.offset, request.City, (int)request.OrganizationType).Count();

                    organizations = Organizations.FromSqlRaw(
                        $"SELECT * " +
                        $"FROM Organizations as o " +
                        $"WHERE MATCH(o.OrganizationName) AGAINST (@p0 IN BOOLEAN MODE)" +
                        where +
                        $"ORDER BY MATCH(o.OrganizationName) AGAINST (@p0) DESC " +
                        $"Limit @p1 Offset @p2;",
                        request.OrganizationName, request.size, request.offset, request.City, (int)request.OrganizationType);

                    organizationsList = organizations.AsNoTracking().ToList();
                }
                else
                {
                    var query = Organizations.AsQueryable();
                    if (request.OrganizationName != null) query = query.Where(t => t.OrganizationName.Contains(request.OrganizationName));
                    if (request.City != null) query = query.Where(t => t.City.Contains(request.City));
                    if (request.OrganizationType > 0) query = query.Where(t => t.OrganizationTypeId == (int)request.OrganizationType);

                    count = query.Count();

                    organizationsList = query.Skip(request.offset).Take(request.size).ToList();
                }

                return organizationsList;

            }
            catch (Exception ex)
            {
                if (ex is NotSupportedException && useFullText && ex.ToString().Contains("is currently not supported."))
                    return FindOrganizationV4(request, false, out count); // Try with useFullText=false
                else
                    throw; //Throw an exception. Will result in 500 return type, but will log the error in controller
            }
        }

        public async Task<SaveOrganizationResult> AddOrganizationV4(Organization organization, bool checkIfExists = true, bool useFullText = true)
        {
            try
            {
                if (!Enum.IsDefined(typeof(OrganizationTypeEnum), organization.OrganizationTypeId))
                {
                    organization.OrganizationTypeId = (int)OrganizationTypeEnum.Learner;
                }

                if (checkIfExists) // Only false when importing from CSV and the table is empty
                {
                    var request = new OrganizationFindRequest
                    {
                        OrganizationName = organization.OrganizationName,
                        City = organization.City,
                        OrganizationType = organization.OrganizationTypeId.ToString(),
                    };

                    // Prevent duplicates (matching Name and City)
                    var duplicate = FindOrganization(request, useFullText);

                    if (duplicate is not null)
                    {
                        // DEBUGGING ONLY - Log duplicates as tab-separated values
                        //if (System.Diagnostics.Debugger.IsAttached &&
                        //	((organization.Domain != null && organization.Domain.Length > 0 && organization.Domain == duplicate.Domain) ||
                        //	(organization.HomepageUrl != null && organization.HomepageUrl.Length > 0 && organization.HomepageUrl == duplicate.HomepageUrl)))
                        //{
                        //	LogHelper.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11:yyyy-MM-dd hh:mm:ss}\t{12:yyyy-MM-dd hh:mm:ss}",
                        //		duplicate.OrganizationId, duplicate.CrunchbaseUuid, duplicate.OrganizationName, duplicate.Permalink,
                        //		duplicate.ShortDescription.Replace("\n", " ").Replace("\t", " "), duplicate.LogoUrl, duplicate.Domain, duplicate.HomepageUrl,
                        //		duplicate.City, duplicate.Region, duplicate.Country, duplicate.CreatedAt, duplicate.UpdatedAt), LogHelper.TimeFormat.None);
                        //	LogHelper.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11:yyyy-MM-dd hh:mm:ss}\t{12:yyyy-MM-dd hh:mm:ss}",
                        //		organization.OrganizationId, organization.CrunchbaseUuid, organization.OrganizationName, organization.Permalink,
                        //		organization.ShortDescription.Replace("\n", " ").Replace("\t", " "), organization.LogoUrl, organization.Domain, organization.HomepageUrl,
                        //		organization.City, organization.Region, organization.Country, organization.CreatedAt, organization.UpdatedAt), LogHelper.TimeFormat.None);
                        //}
                        return SaveOrganizationResult.ErrorDuplicate;
                    }
                }

                await Organizations.AddAsync(organization);
                return SaveOrganizationResult.Success;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool DeleteOrganizationV4(int organizationID, int updatedBy)
        {
            try
            {
                //If UpdatedBy is passed, verify that user is org admin and can delete organizations
                if (updatedBy > 0 && !OrganizationUserRoles.Any(x => x.OrganizationId == organizationID && x.OrganizationRoleId == (int)OrganizationRoleEnum.OrgAdmin && x.UserId == updatedBy))
                {
                    throw new ForbiddenException($"User {updatedBy} does not have permission to delete organization {organizationID}");
                }
                //remove any OrganizationUserRole records and write history for them before deleting the organization. 
                var updateTime = DateTime.Now;
                var userRoles = OrganizationUserRoles.Where(x => x.OrganizationId == organizationID).ToList();

                // (NASA-1809) Allow deleting custom organizations
                // TODO: Per Wiley/HTTP specs API routes should be idempotent, i.e. identical requests should have identical results.
                var organization = Organizations.FirstOrDefault(x => x.OrganizationId == organizationID && (x.CrunchbaseUuid == Guid.Empty));
                if (organization == null)
                    return false;

                userRoles.ForEach(userRole =>
                {
                    //save organizationUserRole history record
                    OrganizationUserRoleHistory orgRolesHistory = Domain.OrganizationUserRoleHistory.CreateOrganizationUserRoleHistory(userRole, true);
                    orgRolesHistory.ChangedByUserId = updatedBy;
                    OrganizationUserRoleHistory.Add(orgRolesHistory);

                    //delete the role
                    OrganizationUserRoles.Remove(userRole);
                });

                //save organization history record
                OrganizationHistory organizationHistory = OrganizationHistory.CreateOrganizationHistory(organization);
                organizationHistory.OrganizationName = "Deleted";
                organizationHistory.UpdatedBy = updatedBy; //pull from delete request when updated
                OrganizationsHistory.Add(organizationHistory);

                //Remove the organization
                Organizations.Remove(organization);
                SaveChanges();

                //TODO: We need to come up with a Kafka messaging process to let other systems know of a delete. 
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
        #region Organization Roles

        //public async Task<OrganizationRole> GetOrganizationRole(string roleName)
        //{
        //    return await OrganizationRoles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == roleName);
        //}

        //public async Task<OrganizationUserRole> GetOrganizationUserRole(int organizationId, int userId, int organizationRoleId)
        //{
        //    return await OrganizationUserRoles.AsNoTracking().FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserId == userId  && x.OrganizationRoleId == organizationRoleId);
        //}
        public async Task<bool> AddOrganizationRole(OrganizationRole role, bool checkIfExists = true)
        {
            if (checkIfExists && OrganizationRoles.AsNoTracking().Any(x => x.OrganizationRoleId == role.OrganizationRoleId && x.Name == role.Name))
            {
                return false;
            }
            else
            {
                await OrganizationRoles.AddAsync(role);
                return true;
            }
        }
        #endregion

        #region Organization User Roles
        public async Task<bool> AddOrganizationUserRole(OrganizationUserRole userRole, bool checkIfExists = true)
        {
            if (checkIfExists && OrganizationUserRoles.AsNoTracking().Any(x => x.OrganizationId == userRole.OrganizationId
            && x.OrganizationRoleId == userRole.OrganizationRoleId
            && x.UserId == userRole.UserId))
            {
                return false;
            }
            else
            {
                await OrganizationUserRoles.AddAsync(userRole);
                return true;
            }
        }

        public (int, IEnumerable<OrganizationRolesResponse>) FindUserOrganizationsAndRoles(int userId, int? roleId,
            string keyword, string include, bool useFullText, int size, int offset, int? typeId = 1)
        {
            //Count is needed tobe returned for pagination since we are retrieving the query already paginated
            int count = 0;

            List<OrganizationRolesResponse> userOrganizations = null;
            try
            {
                if (keyword != null && keyword.Length > 0 && useFullText)
                {
                    string keywords = FormatKeywords(keyword);

                    string andSql = $" AND o.OrganizationTypeId = {typeId} ";
                    if (roleId != null)
                        andSql += $" AND our.OrganizationRoleId = {roleId} ";

                    count = UserOrganizations.FromSqlRaw(
                        $"SELECT DISTINCT o.OrganizationId, o.OrganizationName, o.City, o.LogoUrl, o.OrganizationTypeId " +
                        $"FROM Organizations as o " +
                        $"INNER JOIN OrganizationUserRoles as our on o.OrganizationId = our.OrganizationId and our.UserId = @p1 " +
                        $"WHERE MATCH(o.OrganizationName) AGAINST (@p0 IN BOOLEAN MODE) {andSql}",
                        keywords, userId).ToList().Count;

                    if (count > 0)
                    {
                        userOrganizations = UserOrganizations.FromSqlRaw(
                            $"SELECT DISTINCT o.OrganizationId, o.OrganizationName, o.City, o.LogoUrl, o.OrganizationTypeId " +
                            $"FROM Organizations as o " +
                            $"INNER JOIN OrganizationUserRoles as our on o.OrganizationId = our.OrganizationId and our.UserId = @p2 " +
                            $"WHERE MATCH(o.OrganizationName) AGAINST (@p0 IN BOOLEAN MODE) {andSql}" +
                            $"ORDER BY MATCH(o.OrganizationName) AGAINST (@p1) DESC " +
                            $"Limit @p3 Offset @p4;",
                            keywords, keywords, userId, size, offset).ToList();
                    }
                }
                else
                {
                    count = (from o in Organizations
                             join our in OrganizationUserRoles on o.OrganizationId equals our.OrganizationId
                             where our.UserId == userId && (keyword == null || o.OrganizationName.Contains(keyword))
                                && (roleId == null || our.OrganizationRoleId == roleId) && o.OrganizationTypeId == typeId
                             orderby o.OrganizationName
                             select o
                                     ).Distinct().Count();
                    if (count > 0)
                    {
                        userOrganizations = (from o in Organizations
                                             join our in OrganizationUserRoles on o.OrganizationId equals our.OrganizationId
                                             where our.UserId == userId && (keyword == null || o.OrganizationName.Contains(keyword))
                                                && (roleId == null || our.OrganizationRoleId == roleId) && o.OrganizationTypeId == typeId
                                             orderby o.OrganizationName
                                             select new OrganizationRolesResponse
                                             {
                                                 City = o.City,
                                                 LogoUrl = o.LogoUrl,
                                                 OrganizationId = o.OrganizationId,
                                                 OrganizationName = o.OrganizationName,
                                                 OrganizationTypeId = o.OrganizationTypeId
                                             }
                                         ).Distinct().Skip(offset).Take(size).ToList();
                    }
                }

                if (userOrganizations == null || userOrganizations.Count == 0)
                    return (0, null);

                var enumDict = OrganizationRoleDict.EnumToDictByKey();

                //Find user organization roles
                if (include != null && include.Length > 0 && include.ToLower().Contains("role"))
                {
                    foreach (OrganizationRolesResponse uo in userOrganizations)
                    {
                        var organizationRoles = new List<string>();

                        var roles = (from our in OrganizationUserRoles
                                     where our.OrganizationId == uo.OrganizationId && our.UserId == userId
                                     orderby our.OrganizationRoleId
                                     select our.OrganizationRoleId
                                        ).Distinct().ToList();

                        foreach (var r in roles)
                        {
                            if (enumDict.TryGetValue(r, out string role))
                                organizationRoles.Add(role);
                        }

                        uo.Roles = organizationRoles.AsEnumerable();
                    }
                }

                return (count, userOrganizations);
            }
            catch (Exception ex)
            {
                if (ex is NotSupportedException && useFullText && ex.ToString().Contains("is currently not supported."))
                    return FindUserOrganizationsAndRoles(userId, roleId, keyword, include, false, size, offset); // Try with useFullText=false
                else
                    return (0, null);
            }
        }

        public IEnumerable<OrganizationUserRoleV4> SearchOrganizationUserRole(OrganizationUserRoleFind organizationUserRoleFind, out int count)
        {
            try
            {
                var query = OrganizationUserRoles.AsQueryable();
                if (organizationUserRoleFind.UserId > 0) query = query.Where(t => t.UserId == organizationUserRoleFind.UserId);
                if (organizationUserRoleFind.OrganizationRoleId > 0) query = query.Where(t => t.OrganizationRoleId == (int)organizationUserRoleFind.OrganizationRoleId);
                if (organizationUserRoleFind.OrganizationId > 0) query = query.Where(t => t.OrganizationId == organizationUserRoleFind.OrganizationId);
                if (organizationUserRoleFind.GrantedByUserId > 0) query = query.Where(t => t.GrantedByUserId == organizationUserRoleFind.GrantedByUserId);

                count = query.Count();

                List<OrganizationUserRoleV4> organizationUserRoleV4List = query.Select((x) =>
                OrganizationUserRoleV4.ConvertFromOrganizationUserRole(x))
                    .Skip(organizationUserRoleFind.offset)
                    .Take(organizationUserRoleFind.size)
                    .ToList();

                return organizationUserRoleV4List;
            }
            catch
            {
                throw;
            }
        }

        public async Task<OrganizationUserRoleV4> GetOrganizationUserRole(int OrganizationUserRoleId)
        {

            var organizationUserRole = await OrganizationUserRoles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == OrganizationUserRoleId);

            if (organizationUserRole == null)
                throw new NotFoundException();

            return OrganizationUserRoleV4.ConvertFromOrganizationUserRole(organizationUserRole);

        }
        public bool ExistsOrganizationUserRoleV4(OrganizationUserRole organizationUserRole)
        {
            try
            {
                var query = OrganizationUserRoles.AsQueryable();
                query = query.Where(t => t.UserId == organizationUserRole.UserId);
                query = query.Where(t => t.OrganizationRoleId == (int)organizationUserRole.OrganizationRoleId);
                query = query.Where(t => t.OrganizationId == organizationUserRole.OrganizationId);

                if (query.Count() > 0)
                {
                    return true;
                }

                return false;

            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}