using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CompanyAPI.Domain;
using Xunit;

namespace CompanyAPI.Tests.Models
{
    public class CrunchbaseApiRequestTests
    {
        private static CrunchbaseApiRequest CreateCrunchbaseApiRequest() => new CrunchbaseApiRequest
        {
            Field_IDs = new List<string>
            {
                "name",
                "permalink",
                "location_identifiers",
                "created_at",
                "updated_at",
                "short_description",
                "website_url"
            },
            Query = new List<Query>
            {
                new Query
                {
                    Type = "predicate",
                    Field_ID = "updated_at",
                    Operator_ID = "gte",
                    Values = new List<string>
                    {
                        DateTime.Now.ToString("yyyy-MM-ddTmm:ss:ffZ")
                    }
                }
            },
            Order = new List<Order>
            {
                new Order
                {
                    Field_ID = "updated_at",
                    Sort = "asc"
                }
            },
            Limit = 1000,
            After_ID = new Guid()
        };

        [Fact]
        public void CrunchbaseApiRequest_Valid()
        {
            var request = CreateCrunchbaseApiRequest();
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}