using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyAPI.Domain.Pagination;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CompanyAPI.Domain
{
    public class OrganizationUserRoleV4 : PaginatableObject
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        [StringLength(512)]
        [JsonConverter(typeof(StringEnumConverter))]
        public OrganizationRoleEnum Role { get; set; }

        // Note: API returns UNIX timestamps - use UnixTimeHelper to convert
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [Required]
        [JsonProperty("createdBy")]
        public int GrantedByUserId { get; set; }

        public override string GetUniqueID()
        {
            return Id.ToString();

        }

        public static OrganizationUserRoleV4 ConvertFromOrganizationUserRole(OrganizationUserRole organizationUserRole)
        {
            if (organizationUserRole is null)
                return null;

            return new OrganizationUserRoleV4
            {
                Id = organizationUserRole.Id,
                UserId = organizationUserRole.UserId,
                OrganizationId = organizationUserRole.OrganizationId,
                Role = (OrganizationRoleEnum)organizationUserRole.OrganizationRoleId,
                CreatedAt = organizationUserRole.CreatedAt,
                GrantedByUserId = organizationUserRole.GrantedByUserId,
            };

        }
    }
}
