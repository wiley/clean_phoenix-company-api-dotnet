using Newtonsoft.Json;

namespace CompanyAPI.Domain.Pagination
{
    public class PaginationLinksV4
    {
        public PaginationLink Self { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PaginationLink First { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PaginationLink Next { get; set; } //VB.NET will not support a variable named "Next", it will need to use <JsonProperty(PropertyName:="next")>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PaginationLink Previous { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PaginationLink Last { get; set; }
    }
}