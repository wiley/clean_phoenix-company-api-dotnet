using System.Linq;
using CompanyAPI.Domain;
using CompanyAPI.Domain.Exceptions;
using CompanyAPI.Infrastructure;
using CompanyAPI.Services;
using CompanyAPI.Services.Interfaces;
using FluentAssertions;
using Xunit;

namespace CompanyAPI.Tests.Services
{
    [Collection("Database collection")]
    public class OrganizationServiceTests
    {
        private readonly CompanyDbContext _context = null;
        private readonly IOrganizationService _organizationService;

        public OrganizationServiceTests(DatabaseFixture fixture)
        {
            _context = fixture.FixtureContext;
            _organizationService = new OrganizationService(_context);
        }

        [Fact]
        public void GetOrganizationV4_ReturnsExpected()
        {
            var expected = _context.Organizations.FirstOrDefault();
            var actual = _organizationService.GetOrganizationV4(expected.OrganizationId);

            Assert.NotNull(actual);
            Assert.IsType<OrganizationV4>(actual);
            actual.Should().BeEquivalentTo(OrganizationV4.ConvertFromOrganization(expected));
        }

        [Fact]
        public void GetOrganizationV4_ReturnsNull()
        {
            var actual = _organizationService.GetOrganizationV4(-1);
            Assert.Null(actual);
        }

        [Theory]
        [InlineData(OrganizationTypeEnum.Learner)]
        [InlineData(OrganizationTypeEnum.Talent)]
        public async void AddOrganizationV4_ReturnsExpected(OrganizationTypeEnum type)
        {
            var request = new OrganizationAddRequestV4
            {
                OrganizationName = $"Test Organization {type}",
                City = "City",
                LogoUrl = "https://catalyst.everythindisc.com/organization/logo/logo1.png",
                OrganizationType = type.ToString()
            };
            var expected = new OrganizationV4
            {
                OrganizationId = _context.Organizations.Count() + 1,
                OrganizationName = $"Test Organization {type}",
                City = "City",
                OrganizationType = type
            };

            var actual = await _organizationService.AddOrganizationV4(request, false);

            int countHistoryBefore =
                _context.OrganizationsHistory.Count(x => x.OrganizationId == actual.OrganizationId);

            Assert.NotNull(actual);
            Assert.IsType<OrganizationV4>(actual);
            Assert.Equal(expected.OrganizationName, actual.OrganizationName);
            Assert.Equal(expected.OrganizationType, actual.OrganizationType);
        }

        [Theory]
        [InlineData(OrganizationTypeEnum.Learner, OrganizationTypeEnum.Talent)]
        [InlineData(OrganizationTypeEnum.Talent, OrganizationTypeEnum.Learner)]
        public async void AddOrganizationV4_DuplicateOtherType_ReturnsExpected(OrganizationTypeEnum type1,
            OrganizationTypeEnum type2)
        {
            var request = new OrganizationAddRequestV4
            {
                OrganizationName = $"Organization Other Type {type1}",
                City = "TestCity",
                OrganizationType = type1.ToString()
            };

            var insert1 = await _organizationService.AddOrganizationV4(request, false);

            request.OrganizationType = type2.ToString();
            var actual = await _organizationService.AddOrganizationV4(request, false);

            Assert.NotNull(actual);
            Assert.IsType<OrganizationV4>(actual);

            Assert.True(actual.OrganizationId > 0);
            Assert.Equal(actual.OrganizationName, request.OrganizationName);
            Assert.Equal(actual.OrganizationType, type2);
        }

        [Theory]
        [InlineData(OrganizationTypeEnum.Learner)]
        [InlineData(OrganizationTypeEnum.Talent)]
        public async void AddOrganizationV4_DuplicateFails(OrganizationTypeEnum type)
        {
            var existing = _context.Organizations.FirstOrDefault(x => x.OrganizationTypeId == (int)type);
            var request = new OrganizationAddRequestV4
            {
                OrganizationName = existing.OrganizationName,
                City = existing.City,
                OrganizationType = type.ToString()
            };

            await Assert.ThrowsAsync<ConflictException>(async () =>
                    await _organizationService.AddOrganizationV4(request,
                        false) //attempt to create a group that already exists
            );
        }

        [Theory]
        [InlineData(OrganizationTypeEnum.Learner, 500)]
        [InlineData(OrganizationTypeEnum.Talent, 501)]
        public async void UpdateOrganizationV4_ReturnsExpected(OrganizationTypeEnum type, int organizationId)
        {
            // Update the last record, so as not to affect other tests, which typically use the first
            // NOTE: Only user-entered organizations can be updated
            var last = _context.Organizations.LastOrDefault(x =>
                x.OrganizationTypeId == (int)type && x.OrganizationId == organizationId);

            // Copy of object now
            var expected = new OrganizationV4
            {
                OrganizationId = last.OrganizationId,
                OrganizationName = last.OrganizationName,
                City = last.City,
                OrganizationType = type
            };

            var request = new OrganizationUpdateRequestV4
            {
                OrganizationName = "[UPDATED] " + expected.OrganizationName,
                City = "[UPDATED] " + expected.City,
                LogoUrl = ""
            };

            int countHistoryBefore =
                _context.OrganizationsHistory.Count(x => x.OrganizationId == expected.OrganizationId);

            // Object after update
            var actual = await _organizationService.UpdateOrganizationV4(request, expected.OrganizationId, false);

            Assert.NotNull(actual);
            Assert.IsType<OrganizationV4>(actual);
            Assert.Equal(request.OrganizationName, actual.OrganizationName);
            Assert.Equal(request.City, actual.City);
            Assert.NotEqual(expected.OrganizationName, actual.OrganizationName);
            Assert.NotEqual(expected.City, actual.City);
            Assert.Equal(expected.OrganizationType, actual.OrganizationType);
            Assert.Equal(countHistoryBefore + 1,
                _context.OrganizationsHistory.Count(x =>
                    x.OrganizationId == expected.OrganizationId)); //verify History written
        }


        [Fact]
        public async void UpdateOrganizationV4_InvalidOrganizationIdFails()
        {
            var request = new OrganizationUpdateRequestV4
            {
                OrganizationName = "UPDATED NAME",
                City = "Updated City"
            };

            var actual = await _organizationService.UpdateOrganizationV4(request, -1, false);

            Assert.Null(actual);
            Assert.IsNotType<Organization>(actual);
        }


        [Fact]
        public async void UpdateOrganizationV4_DuplicateFails()
        {
            // NOTE: For now only user-entered organizations can be updated	
            var org1 = _context.Organizations.FirstOrDefault();
            var org2 = _context.Organizations.Last(x => x.OrganizationTypeId == org1.OrganizationTypeId);

            var request = new OrganizationUpdateRequestV4
            {
                OrganizationName = org1.OrganizationName,
                City = org1.City
            };
            int countHistoryBefore = _context.OrganizationsHistory.Count(x => x.OrganizationId == org2.OrganizationId);

            await Assert.ThrowsAsync<ConflictException>(async () =>
                    await _organizationService.UpdateOrganizationV4(request, org2.OrganizationId,
                        false) //attempt to create a group that already exists
            );

            org1.Should().NotBeEquivalentTo(org2);
            Assert.Equal(countHistoryBefore,
                _context.OrganizationsHistory.Count(x =>
                    x.OrganizationId == org2.OrganizationId)); //verify History Not written
        }
    }
}