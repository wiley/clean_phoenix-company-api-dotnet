using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyAPI.Domain;

namespace CompanyAPI.Services.Interfaces
{
    public interface IOrganizationUserRoleService
    {       
        IEnumerable<OrganizationUserRoleV4> SearchOrganizationUserRole(OrganizationUserRoleFind organizationUserRoleFind, out int count);
        Task<OrganizationUserRoleV4> GetOrganizationUserRole(int organizationUserRoleId);
        Task<OrganizationUserRoleV4> AddOrganizationUserRoleV4(OrganizationUserRoleAddRequest request);
        Task<bool> DeleteOrganizationUserRole(int organizationUserRoleId);
    }
}