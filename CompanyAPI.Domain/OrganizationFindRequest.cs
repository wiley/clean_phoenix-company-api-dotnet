using System.ComponentModel.DataAnnotations;

namespace CompanyAPI.Domain
{
    public class OrganizationFindRequest
    {
        [Required]
        [MaxLength(512)]
        public string OrganizationName { get; set; }

        [Required]
        [MaxLength(245)]
        public string City { get; set; }

        public string OrganizationType { get; set; } = OrganizationTypeEnum.Learner.ToString();
    }
}