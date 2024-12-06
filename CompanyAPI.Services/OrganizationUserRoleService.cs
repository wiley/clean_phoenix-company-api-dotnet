using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyAPI.Domain;
using CompanyAPI.Domain.Exceptions;
using CompanyAPI.Infrastructure.Interface;
using CompanyAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompanyAPI.Services
{
    public class OrganizationUserRoleService : IOrganizationUserRoleService
    {
        private readonly ICompanyDbContext _iCompanyDbContext;
        private readonly IKafkaService _kafkaService;

        public OrganizationUserRoleService(ICompanyDbContext companyDbContext, IKafkaService kafkaService)
        {
            _iCompanyDbContext = companyDbContext;
            _kafkaService = kafkaService;
        }

        public IEnumerable<OrganizationUserRoleV4> SearchOrganizationUserRole(OrganizationUserRoleFind organizationUserRoleFind, out int count)
        {                  
            return _iCompanyDbContext.SearchOrganizationUserRole(organizationUserRoleFind, out count);
        }

        public Task<OrganizationUserRoleV4> GetOrganizationUserRole(int organizationUserRoleId)
        {
            return _iCompanyDbContext.GetOrganizationUserRole(organizationUserRoleId);
        }

        public async Task<OrganizationUserRoleV4> AddOrganizationUserRoleV4(OrganizationUserRoleAddRequest request) 
        {
            var organizationUserRole = new OrganizationUserRole
            {
                UserId = request.UserId,
                OrganizationId = request.OrganizationId,
                OrganizationRoleId = (int)request.Role,
                GrantedByUserId = request.GrantedByUserId,
            };

            var result = _iCompanyDbContext.ExistsOrganizationUserRoleV4(organizationUserRole);
            
            if (!result)
            {
                await _iCompanyDbContext.OrganizationUserRoles.AddAsync(organizationUserRole);
                await _iCompanyDbContext.SaveChangesAsync();

                await _kafkaService.SendKafkaMessage(organizationUserRole.Id.ToString(), "OrganizationUserRoleCreated", organizationUserRole, "wly.glb.pl.organization");

                return OrganizationUserRoleV4.ConvertFromOrganizationUserRole(organizationUserRole);

            } else { 
                // Return object without saving
                throw new ConflictException($"OrganizationUserRole '{request.UserId} - {request.OrganizationId} - {request.Role}' already exists.");
            }
        }

        public async Task<bool> DeleteOrganizationUserRole(int organizationUserRoleId)
        {
            var organizationUserRole = await _iCompanyDbContext.OrganizationUserRoles.FirstOrDefaultAsync(t => t.Id == organizationUserRoleId);
            if (organizationUserRole == null)
                throw new NotFoundException();

            _iCompanyDbContext.OrganizationUserRoles.Remove(organizationUserRole);
            await _iCompanyDbContext.SaveChangesAsync();

            await _kafkaService.SendKafkaMessage(organizationUserRoleId.ToString(), "OrganizationUserRoleDeleted", organizationUserRole, "wly.glb.pl.organization");
            return true;
        }
        
    }
}