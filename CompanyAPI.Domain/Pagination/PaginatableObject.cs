using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Linq;

namespace CompanyAPI.Domain.Pagination
{
    public abstract class PaginatableObject
    {
        [NotMapped]
        [JsonIgnore]
        public string listUrlFormat { get; private set; }
        [NotMapped]
        [JsonIgnore]
        public string detailUrlFormat { get; private set; }
        [NotMapped]
        [JsonIgnore]
        public List<string> urlParameters { get; private set; }

        public class PaginatableObjectLinks
        {
            public PaginationLink Self { get; set; }
        }

        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PaginatableObjectLinks _links { get; set; }

        //This method must be overridden to provide the unique identifier to be used in a Self link
        public abstract string GetUniqueID();

        public void AddLinks(HttpRequest request)
        {
            listUrlFormat = string.Concat(
                        request.Scheme,
                        "://",
                        request.Host.ToUriComponent(),
                        request.GetEncodedPathAndQuery());

            string[] urlParts = listUrlFormat.Split('?');
            listUrlFormat = urlParts[0];

            int lastSlash = listUrlFormat.LastIndexOf('/'); 
            listUrlFormat = (lastSlash == listUrlFormat.Length - 1) ? listUrlFormat.Substring(0, lastSlash) : listUrlFormat;

            if (urlParts.Length > 1)
            {
                urlParameters = urlParts[1].Split("&").ToList();
            }

            string url = request.Path;

            if(!string.IsNullOrEmpty(url))
            {
                lastSlash = url.LastIndexOf('/');
                url = (lastSlash == url.Length - 1) ? url.Substring(0, lastSlash) : url;
            }
            
            detailUrlFormat = string.Concat(
                request.Scheme,
                "://",
                request.Host.ToUriComponent(),
                url,
                "/{uniqueid}");
        }

        public void CreateSelfLinkV4(HttpRequest request, string id = "")
        {
            AddLinks(request);
            var url = detailUrlFormat;

            if (!string.IsNullOrEmpty(id))
                url = url.Replace("{uniqueid}", id.ToString());
            else
                url = url.Replace("/{uniqueid}", "");

            _links = new PaginatableObjectLinks()
            {
                Self = new PaginationLink(url)
            };
        }

        public void CreateSelfLink(string url, string id = "")
        {
            if (!string.IsNullOrEmpty(id))
                url = url.Replace("{uniqueid}", id.ToString());
            else  
                url = url.Replace("/{uniqueid}", "");
            _links = new PaginatableObjectLinks()
            {
                Self = new PaginationLink(url)
            };
        }
    }
}