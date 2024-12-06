using System.ComponentModel.DataAnnotations;

namespace CompanyAPI.Domain
{
    public class OrganizationV4Find
    {
        public int offset;
        public int size;
        public string OrganizationName;
        public string City;
        public OrganizationTypeEnum OrganizationType;
    }
}