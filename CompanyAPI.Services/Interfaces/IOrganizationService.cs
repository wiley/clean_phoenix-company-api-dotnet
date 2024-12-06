using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyAPI.Domain;

namespace CompanyAPI.Services.Interfaces
{
    public interface IOrganizationService
    {
        //V4
        IEnumerable<OrganizationV4> GetOrganizationList(OrganizationV4Find request, out int count);
        OrganizationV4 GetOrganizationV4(int organizationId);
        Task<OrganizationV4> AddOrganizationV4(OrganizationAddRequestV4 request, bool useFullText = true);
        Task<OrganizationV4> UpdateOrganizationV4(OrganizationUpdateRequestV4 request, int organizationId, bool useFullText = true);
        bool DeleteOrganizationV4(int organizationId, int userId);
        Task GenerateKafkaEvents();
    }
}
