using System;
using System.Collections.Generic;

namespace CompanyAPI.Domain
{
    public class CrunchbaseApiRequest
    {
        public List<string> Field_IDs { get; set; }
        public List<Query> Query { get; set; }
        public List<Order> Order { get; set; }
        public int Limit { get; set; } // 50 items are returned by default; 1000 items max
        public Guid? Before_ID { get; set; } // uuid of the first item in the current page (cannot use with After_ID)
        public Guid? After_ID { get; set; } // uuid of the last item in the current page (cannot use with Before_ID)
    }

    public class Query
    {
        public string Type { get; set; }
        public string Field_ID { get; set; }
        public string Operator_ID { get; set; }
        public List<string> Values { get; set; }
    }

    public class Order
    {
        public string Field_ID { get; set; }
        public string Sort { get; set; } = "asc";
    }
}