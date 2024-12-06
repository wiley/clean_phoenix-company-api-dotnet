using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyAPI.Domain.Pagination;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CompanyAPI.Domain
{
    /* MySQL Notes
        VARCHAR max length for version <= 5.0.3 is 245, > 5.0.3 is 65535
        https://stackoverflow.com/questions/21156301
        https://stackoverflow.com/questions/13506832
        Best practices for SQL varchar column length
        https://stackoverflow.com/questions/8295131
        
        FULLTEXT indexes on VARCHAR(245) require twice the space of a VARCHAR(254)
        https://www.vbulletin.com/forum/project.php?issueid=32655
    */
    public class OrganizationV4 : PaginatableObject
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrganizationId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrganizationTypeEnum OrganizationType { get; set; }

        [Required]
        [StringLength(512)]
        public string OrganizationName { get; set; }

        [StringLength(512)]
        public string LogoUrl { get; set; }

        [StringLength(245)]
        public string City { get; set; }

        // Note: API returns UNIX timestamps - use UnixTimeHelper to convert
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public override string GetUniqueID()
        {
            return OrganizationId.ToString();

        }

        public static OrganizationV4 ConvertFromOrganization(Organization organization)
        {
            if (organization is null)
                return null;

            return new OrganizationV4
            {
                OrganizationId = organization.OrganizationId,
                OrganizationName = organization.OrganizationName,
                City = organization.City,
                LogoUrl = organization.LogoUrl,
                OrganizationType = (OrganizationTypeEnum)organization.OrganizationTypeId,
                CreatedAt = organization.CreatedAt,
                UpdatedAt = organization.UpdatedAt
            };

        }
    }
}