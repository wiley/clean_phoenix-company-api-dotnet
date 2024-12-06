using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CompanyAPI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CompanyAPI.Infrastructure.Interface
{
    public enum SaveOrganizationResult
    {
        Success = 1,
        Unchanged = 0,
        ErrorUnknown = -1,
        ErrorDuplicate = -2,
        ErrorDatabase = -3,
        ErrorNotFound = -4
    }
    public interface ICompanyDbContext
    {
        ChangeTracker ChangeTracker { get; }
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        DbSet<Organization> Organizations { get; set; }
        DbSet<OrganizationHistory> OrganizationsHistory { get; set; }
        DbSet<OrganizationRole> OrganizationRoles { get; set; }
        DbSet<OrganizationUserRole> OrganizationUserRoles { get; set; }
        DbSet<OrganizationUserRoleHistory> OrganizationUserRoleHistory { get; set; }
        Task<SaveOrganizationResult> AddOrganization(Organization organization, bool checkIfExists = true, bool useFullText = true);
        bool DeleteOrganization(int organizationID, int updatedBy = -1, bool fromMerge = false);
        Organization FindOrganization(OrganizationFindRequest request, bool useFullText = true);
        (int,IEnumerable<OrganizationRolesResponse>) FindUserOrganizationsAndRoles(int userId, int? roleId, string keyword, 
            string include, bool useFullText, int size, int offset, int? typeId = 1);
        Task<Organization> GetOrganization(int organizationId);
        Task<bool> AddOrganizationUserRole(OrganizationUserRole userRole, bool checkIfExists = true);
        // Task<OrganizationRole> GetOrganizationRole(string roleName);
        void Initialize();
        void SeedOrganizations();
        void SeedOrganizationRoles();
        IEnumerable<Organization> FindOrganizationV4(OrganizationV4Find request, bool useFullText, out int count);
        Task<SaveOrganizationResult> AddOrganizationV4(Organization organization, bool checkIfExists = true, bool useFullText = true);
        bool DeleteOrganizationV4(int organizationID, int updatedBy);
        IEnumerable<OrganizationUserRoleV4> SearchOrganizationUserRole(OrganizationUserRoleFind organizationUserRoleFind, out int count);
        Task<OrganizationUserRoleV4> GetOrganizationUserRole(int organizationUserRoleId);
        bool ExistsOrganizationUserRoleV4(OrganizationUserRole organizationUserRole);
    }
}