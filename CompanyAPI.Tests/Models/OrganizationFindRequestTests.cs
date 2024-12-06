using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CompanyAPI.Domain;
using FluentAssertions;
using Xunit;

namespace CompanyAPI.Tests.Models
{
    public class OrganizationFindRequestTests
    {
        static OrganizationFindRequest CreateOrganizationFindRequest() => new OrganizationFindRequest
        {
            OrganizationName = "Test Organization",
            City = "Test City"
        };

        [Fact]
        public void OrganizationFindRequest_Valid()
        {
            var request = CreateOrganizationFindRequest();
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void OrganizationFindRequest_Invalid_MissingOrganizationName()
        {
            var request = new OrganizationFindRequest
            {
                City = "Test City"
            };
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The OrganizationName field is required.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("OrganizationName", result.MemberNames.First());
        }

        [Fact]
        public void OrganizationFindRequest_Invalid_MissingCity()
        {
            var request = new OrganizationFindRequest
            {
                OrganizationName = "Test Company"
            };
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The City field is required.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("City", result.MemberNames.First());
        }

        [Fact]
        public void OrganizationFindRequest_Valid_OrganizationNameAtMax()
        {
            var request = CreateOrganizationFindRequest();
            request.OrganizationName = TestHelper.GenerateRandomText(512);
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void OrganizationFindRequest_Valid_CityAtMax()
        {
            var request = CreateOrganizationFindRequest();
            request.City = TestHelper.GenerateRandomText(245);
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void OrganizationFindRequest_Invalid_OrganizationNameTooLong()
        {
            var request = CreateOrganizationFindRequest();
            request.OrganizationName = TestHelper.GenerateRandomText(513);
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The field OrganizationName must be a string or array type with a maximum length of '512'.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("OrganizationName", result.MemberNames.First());
        }

        [Fact]
        public void OrganizationFindRequest_Invalid_CityTooLong()
        {
            var request = CreateOrganizationFindRequest();
            request.City = TestHelper.GenerateRandomText(246);
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The field City must be a string or array type with a maximum length of '245'.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("City", result.MemberNames.First());
        }
    }
}