using System.Collections.Generic;
using System.Linq;
using CompanyAPI.Domain.Pagination;
using Microsoft.AspNetCore.Http;

namespace CompanyAPI.Services
{
    public class PaginationHelperV4<T>
    {
        private int _size;
        private int _offset;
        private int _count;
        private string _listUrlFormat;
        private string _detailUrlFormat;
        private List<string> _urlParameters;

        //Return a count of results
        public bool ShowCount { get; set; } = false;

        //Results provided were already paginated in the database, so the offset should be reported but not re-paginated
        public bool ResultsPrePaginated { get; set; } = false;

        //Assuming that results provided will be a complete set
        public bool ResultsComplete { get; set; } = true;

        //If results are not complete, provide a count by separate mechanism
        public int OverrideCount { get; set; } = -1;

        public PaginationHelperV4(int size, int offset, int count)
        {
            _size = size;
            _offset = offset;
            _count = count;
        }

        public PaginatedContentV4<T> ReturnContent(HttpRequest request, IEnumerable<T> content)
        {
            PaginatedContentV4<T> result = new PaginatedContentV4<T>();
            if (content == null)
            {
                result.Items = new T[0];
            }
            else
            {
                result.Items = content.ToArray();
               
                foreach (object contentItem in result.Items)
                {
                    if (contentItem is PaginatableObject)
                        ((PaginatableObject)contentItem).CreateSelfLinkV4(request, ((PaginatableObject)contentItem).GetUniqueID());

                    _detailUrlFormat = ((PaginatableObject)contentItem).detailUrlFormat;
                    _urlParameters = ((PaginatableObject)contentItem).urlParameters;
                    _listUrlFormat = ((PaginatableObject)contentItem).listUrlFormat;
                }
            }

            if(result.Items.Length > 0)
            {
                result._links = CreatePaginationLinks(content.Count());
                result.Count = _count;
            }

            return result;
        }

        protected PaginationLinksV4 CreatePaginationLinks(int contentCount)
        {
            PaginationLinksV4 result = new PaginationLinksV4();

            if (_urlParameters is not null)
                _urlParameters.RemoveAll(u => u.Equals(""));

            result.Self = new PaginationLink(_listUrlFormat, "", _urlParameters);

            if(_urlParameters is not null)
            {
                _urlParameters.RemoveAll(u => u.StartsWith("offset="));
                _urlParameters.RemoveAll(u => u.StartsWith("size="));
            }

            result.First = new PaginationLink(_listUrlFormat, $"size={_size}&offset=0", _urlParameters);

            if (_offset >= _size)
            {
                result.Previous = new PaginationLink(_listUrlFormat, $"size={_size}&offset={_offset - _size}", _urlParameters);
            }
            if (_offset + _size < _count)
            {
                result.Next = new PaginationLink(_listUrlFormat, $"size={_size}&offset={_offset + _size}", _urlParameters);
            }
            if (_offset + _size <= _count)
            {
                result.Last = new PaginationLink(_listUrlFormat, $"size={_size}&offset={_count - _size}", _urlParameters);
            }
            else
            {
                result.Last = result.First;
            }

            return result;

            
        }
    }
}