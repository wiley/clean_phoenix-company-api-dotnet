using System;
using System.ComponentModel.DataAnnotations;

namespace CompanyAPI.Domain
{
    public class OrganizationHistory
    {
        [Required]
        public int OrganizationId { get; set; }

        [Required]
        [StringLength(512)]
        public string OrganizationName { get; set; }

        [StringLength(512)]
        public string LogoUrl { get; set; }

        [StringLength(245)]
        public string City { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        public int UpdatedBy { get; set; }

        [Required]
        public int OrganizationTypeId { get; set; } = (int)OrganizationTypeEnum.Learner;

        public static OrganizationHistory CreateOrganizationHistory(Organization org)
        {
            //Since group history just indicates the status of a Group before it is changed, create a Group History from a Group object
            return new OrganizationHistory()
            {
                OrganizationId = org.OrganizationId,
                OrganizationName = org.OrganizationName,
                LogoUrl = org.LogoUrl,
                City = org.City,
                UpdatedAt = DateTime.Now,
                UpdatedBy = -1, //return -1 as a default for userId. When present, this will be overwritten with the userId passed in. 
                OrganizationTypeId = org.OrganizationTypeId
            };
        }
    }

   
}
