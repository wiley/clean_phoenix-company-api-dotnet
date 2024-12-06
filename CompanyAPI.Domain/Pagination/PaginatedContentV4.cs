using Newtonsoft.Json;

namespace CompanyAPI.Domain.Pagination
{
    public class PaginatedContentV4<T>
    {
        public T[] Items { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PaginationLinksV4 _links { get; set; }
        public int Count { get; set; }
    }
}