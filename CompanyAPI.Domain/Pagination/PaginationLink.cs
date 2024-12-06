using System.Collections.Generic;

namespace CompanyAPI.Domain.Pagination
{
    public class PaginationLink
    {
        public PaginationLink(string href, string pagination = "", List<string> parameters = null)
        {
            if (parameters is not null)
            {
                List<string> list = new List<string>(parameters);
                
                if(!string.IsNullOrEmpty(pagination))
                    list.Add(pagination);

                if (list.Count > 0)
                {
                    Href = href + "?" + string.Join("&", list);
                }
            }
            else
            {
                Href = href;
                if(!string.IsNullOrEmpty(pagination))
                {
                    Href += $"&{pagination}";
                }
            }
            
        }

        public string Href { get; set; }
    }
}