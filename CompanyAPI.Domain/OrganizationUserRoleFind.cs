using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CompanyAPI.Domain
{
    public class OrganizationUserRoleFind
    {
        public int offset;
        public int size;
        public int UserId;
        public int OrganizationId;
        [JsonConverter(typeof(StringEnumConverter))]
        public OrganizationRoleEnum OrganizationRoleId;
        [JsonProperty("createdBy")]
        public int GrantedByUserId;
    }
}
