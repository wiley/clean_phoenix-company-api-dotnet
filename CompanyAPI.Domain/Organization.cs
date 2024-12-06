using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyAPI.Domain.Pagination;

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
    public class Organization : PaginatableObject
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrganizationId { get; set; }

        public int OrganizationTypeId { get; set; }

        public Guid CrunchbaseUuid { get; set; } = Guid.Empty;

        [Required]
        [StringLength(512)]
        public string OrganizationName { get; set; }

        [StringLength(512)]
        public string Permalink { get; set; }

        [StringLength(512)]
        public string ShortDescription { get; set; }

        [StringLength(512)]
        public string LogoUrl { get; set; }

        [StringLength(245)]
        public string Domain { get; set; }

        [StringLength(1024)]
        public string HomepageUrl { get; set; }

        [StringLength(245)]
        public string City { get; set; }

        [StringLength(245)]
        public string Region { get; set; }

        // Note: API returns "country", but CSV has only 2-char "country_code"
        [StringLength(245)]
        public string Country { get; set; }

        // Note: API returns UNIX timestamps - use UnixTimeHelper to convert
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public override string GetUniqueID()
        {
            return OrganizationId.ToString();
        }
    }
}