using CompanyAPI.Domain.Pagination;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyAPI.Domain
{
    [NotMapped]
    public class OrganizationRolesResponse : PaginatableObject
    {
        public int OrganizationId { get; set; }

        public int OrganizationTypeId { get; set; }

        public string OrganizationName { get; set; }

        public string City { get; set; }

        public string LogoUrl { get; set; }

        //public IEnumerable<OrganizationRole> Roles { get; set; }
        
        public IEnumerable<string> Roles { get; set; }

        public override string GetUniqueID()
        {
            return OrganizationId.ToString();
        }

    }
}
