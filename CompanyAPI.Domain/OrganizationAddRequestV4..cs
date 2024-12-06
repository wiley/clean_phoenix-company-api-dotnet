using System.ComponentModel.DataAnnotations;

namespace CompanyAPI.Domain
{
    public class OrganizationAddRequestV4
    {
        [Required]
        [StringLength(512)]
        public string OrganizationName { get; set; }

        [Required]
        [StringLength(245)]
        public string City { get; set; }

        public string LogoUrl { get; set; }

        [Required]
        public string OrganizationType { get; set; } = OrganizationRoleEnum.Learner.ToString();
    }
}