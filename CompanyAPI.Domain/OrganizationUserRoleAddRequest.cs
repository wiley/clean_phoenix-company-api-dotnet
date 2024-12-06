using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CompanyAPI.Domain
{
    public class OrganizationUserRoleAddRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The value of UserId is not valid.")]
        public int UserId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The value of OrganizationId is not valid.")]
        public int OrganizationId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The value of Role is not valid.")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OrganizationRoleEnum Role { get; set; }
        [JsonProperty("createdBy")]
        [JsonIgnore]
        public int GrantedByUserId { get; set; }
    }
}