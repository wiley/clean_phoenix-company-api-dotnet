using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyAPI.Domain;
using CompanyAPI.Domain.Exceptions;
using CompanyAPI.Infrastructure;
using CompanyAPI.Services;
using CompanyAPI.Services.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CompanyAPI.Tests.Services
{
    [Collection("Database collection")]
    public class OrganizationUserRoleServiceTests
    {
        private readonly CompanyDbContext _context = null;
        private readonly IOrganizationUserRoleService _organizationUserRoleService;
        private readonly IKafkaService _kafkaService;

        public OrganizationUserRoleServiceTests(DatabaseFixture fixture)
        {
            _context = fixture.FixtureContext;
            _kafkaService = Substitute.For<IKafkaService>();
            _organizationUserRoleService = new OrganizationUserRoleService(_context, _kafkaService);
        }

        #region OrganizationUserRole Tests
        //V4
        [Fact]
        public async Task AddNewOrganizationUserRole_ReturnExpect()
        {
            var new_organization = await _context.Organizations.AddAsync(new Organization()
            {
                OrganizationTypeId = (int)OrganizationTypeEnum.Learner,
                CrunchbaseUuid = Guid.NewGuid(),
                OrganizationName = "Organization AddNewOrganizationUserRole_ReturnExpect",
                Permalink = "wiley",
                Domain = "wiley.com",
                HomepageUrl = "http://wiley.com",
                LogoUrl = "https://crunchbase-production-res.cloudinary.com/image/upload/c_lpad,h_120,w_120,f_jpg/v1416112216/csozpaeukqh1seoio0qx.png",
                City = "Hoboken",
                Region = "New Jersey",
                Country = "US",
                ShortDescription = "Test of AddNewOrganizationUserRole_ReturnExpect"                
            });

            var request = new OrganizationUserRoleAddRequest
            {
                OrganizationId = new_organization.Entity.OrganizationId,
                UserId = 1,
                Role = OrganizationRoleEnum.OrgAdmin
            };

            var actual = await _organizationUserRoleService.AddOrganizationUserRoleV4(request);

            await _kafkaService.Received(1).SendKafkaMessage(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<object>(), Arg.Any<string>());

            actual.Should().NotBeNull();
            Assert.IsType<OrganizationUserRoleV4>(actual);
            Assert.Equal(request.Role, actual.Role);
            Assert.Equal(request.OrganizationId, actual.OrganizationId);
            Assert.Equal(request.UserId, actual.UserId);
        }

        //V4
        [Fact]
        public async Task AddNewOrganizationUserRole_ReturnConflictException()
        {
            var exists_organizationUserRole = _context.OrganizationUserRoles.FirstOrDefault();
            var request = new OrganizationUserRoleAddRequest
            {
                OrganizationId = exists_organizationUserRole.OrganizationId,
                UserId = exists_organizationUserRole.UserId,
                Role = (OrganizationRoleEnum)exists_organizationUserRole.OrganizationRoleId
            };

            await Assert.ThrowsAsync<ConflictException>(async () =>
                await _organizationUserRoleService.AddOrganizationUserRoleV4(request) //attempt to create a OrgUserRole that already exists
            );

            await _kafkaService.DidNotReceive().SendKafkaMessage(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<object>(), Arg.Any<string>());
        }

        [Fact]
        public async Task GetOrganizationUserRole_ReturnsExpected()
        {
            var expected = _context.OrganizationUserRoles.FirstOrDefault();
            var actual = await _organizationUserRoleService.GetOrganizationUserRole(expected.Id);
            var organizationUserRoleV4 = new OrganizationUserRoleV4()
            {
                Id = expected.Id,
                UserId = expected.UserId,
                OrganizationId = expected.OrganizationId,
                Role = (OrganizationRoleEnum)expected.OrganizationRoleId,
                CreatedAt = expected.CreatedAt,
                GrantedByUserId = expected.GrantedByUserId
            };

            Assert.NotNull(actual);
            Assert.IsType<OrganizationUserRoleV4>(actual);
            actual.Should().BeEquivalentTo(organizationUserRoleV4);
        }

        [Fact]
        public async Task GetOrganizationUserRole_ReturnsNull()
        {
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _organizationUserRoleService.GetOrganizationUserRole(-1)
            );
        }

        [Fact]
        public void SearchOrganizationUserRole_ReturnsEmptyList()
        {
            var emptyList = new List<OrganizationUserRoleV4>();
            var request = new OrganizationUserRoleFind
            { offset = 0, size = 10, OrganizationId = 999 };
            var count = 0;
            var response = _organizationUserRoleService.SearchOrganizationUserRole(request, out count);

            Assert.IsAssignableFrom<IEnumerable<OrganizationUserRoleV4>>(response);
            Assert.Empty(response);
            response.Should().BeEquivalentTo(emptyList);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void SearchOrganizationUserRole_ReturnsMatch(int userId)
        {
            var expected = _context.OrganizationUserRoles.Where(x => x.UserId == userId).Select((x) =>
                OrganizationUserRoleV4.ConvertFromOrganizationUserRole(x));
            var request = new OrganizationUserRoleFind { offset = 0, size = 10, UserId = userId };
            var count = 0;
            var response = _organizationUserRoleService.SearchOrganizationUserRole(request, out count);

            Assert.IsAssignableFrom<IEnumerable<OrganizationUserRoleV4>>(response);
            Assert.NotEmpty(response);
            count.Should().BeGreaterThan(0);
            response.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void DeleteOrganizationUserRole_ReturnExpect(int id)
        {
            var actual = _organizationUserRoleService.DeleteOrganizationUserRole(id);

            actual.Should().NotBeNull();

            await _kafkaService.Received(1).SendKafkaMessage(Arg.Any<string>(), "OrganizationUserRoleDeleted", Arg.Any<object>(), Arg.Any<string>());
        }
    }
}
#endregion OrganizationUserRole Tests