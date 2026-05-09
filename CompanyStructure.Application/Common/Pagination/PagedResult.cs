using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Common.Pagination
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
