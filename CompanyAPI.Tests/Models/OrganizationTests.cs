using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CompanyAPI.Domain;
using FluentAssertions;
using Xunit;

namespace CompanyAPI.Tests.Models
{
    public class OrganizationTests
    {
        private static Organization CreateOrganization() => new Organization
        {
            OrganizationId = 1,
            CrunchbaseUuid = Guid.NewGuid(),
            OrganizationName = "OrganizationName",
            Permalink = "Permalink",
            Domain = "Domain",
            HomepageUrl = "HomepageUrl",
            LogoUrl = "LogoUrl",
            City = "City",
            Region = "Region",
            Country = "Country",
            ShortDescription = "ShortDescription",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        [Fact]
        public void Organization_Valid()
        {
            var org = CreateOrganization();
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Organization_Invalid_MissingOrganizationName()
        {
            var org = CreateOrganization();
            org.OrganizationName = "";
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The OrganizationName field is required.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("OrganizationName", result.MemberNames.First());
        }

        [Fact]
        public void Organization_Valid_OrganizationNameAtMax()
        {
            var org = CreateOrganization();
            org.OrganizationName = TestHelper.GenerateRandomText(512);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Organization_Invalid_OrganizationNameTooLong()
        {
            var org = CreateOrganization();
            org.OrganizationName = TestHelper.GenerateRandomText(513);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The field OrganizationName must be a string with a maximum length of 512.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("OrganizationName", result.MemberNames.First());
        }

        [Fact]
        public void Organization_Valid_PermalinkAtMax()
        {
            var org = CreateOrganization();
            org.Permalink = TestHelper.GenerateRandomText(512);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Organization_Invalid_PermalinkTooLong()
        {
            var org = CreateOrganization();
            org.Permalink = TestHelper.GenerateRandomText(513);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The field Permalink must be a string with a maximum length of 512.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("Permalink", result.MemberNames.First());
        }

        [Fact]
        public void Organization_Valid_ShortDescriptionAtMax()
        {
            var org = CreateOrganization();
            org.ShortDescription = TestHelper.GenerateRandomText(512);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Organization_Invalid_ShortDescriptionTooLong()
        {
            var org = CreateOrganization();
            org.ShortDescription = TestHelper.GenerateRandomText(513);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The field ShortDescription must be a string with a maximum length of 512.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("ShortDescription", result.MemberNames.First());
        }

        [Fact]
        public void Organization_Valid_LogoUrlAtMax()
        {
            var org = CreateOrganization();
            org.LogoUrl = TestHelper.GenerateRandomText(512);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Organization_Invalid_LogoUrlTooLong()
        {
            var org = CreateOrganization();
            org.LogoUrl = TestHelper.GenerateRandomText(513);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The field LogoUrl must be a string with a maximum length of 512.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("LogoUrl", result.MemberNames.First());
        }

        [Fact]
        public void Organization_Valid_DomainAtMax()
        {
            var org = CreateOrganization();
            org.Domain = TestHelper.GenerateRandomText(245);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Organization_Invalid_DomainTooLong()
        {
            var org = CreateOrganization();
            org.Domain = TestHelper.GenerateRandomText(246);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The field Domain must be a string with a maximum length of 245.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("Domain", result.MemberNames.First());
        }

        [Fact]
        public void Organization_Valid_HomepageUrlAtMax()
        {
            var org = CreateOrganization();
            org.HomepageUrl = TestHelper.GenerateRandomText(1024);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Organization_Invalid_HomepageUrlTooLong()
        {
            var org = CreateOrganization();
            org.HomepageUrl = TestHelper.GenerateRandomText(1025);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The field HomepageUrl must be a string with a maximum length of 1024.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("HomepageUrl", result.MemberNames.First());
        }

        [Fact]
        public void Organization_Valid_CityAtMax()
        {
            var org = CreateOrganization();
            org.City = TestHelper.GenerateRandomText(245);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Organization_Invalid_CityTooLong()
        {
            var org = CreateOrganization();
            org.City = TestHelper.GenerateRandomText(246);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The field City must be a string with a maximum length of 245.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("City", result.MemberNames.First());
        }

        [Fact]
        public void Organization_Valid_RegionAtMax()
        {
            var org = CreateOrganization();
            org.Region = TestHelper.GenerateRandomText(245);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Organization_Invalid_RegionTooLong()
        {
            var org = CreateOrganization();
            org.Region = TestHelper.GenerateRandomText(246);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The field Region must be a string with a maximum length of 245.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("Region", result.MemberNames.First());
        }

        [Fact]
        public void Organization_Valid_CountryAtMax()
        {
            var org = CreateOrganization();
            org.Country = TestHelper.GenerateRandomText(245);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Organization_Invalid_CountryTooLong()
        {
            var org = CreateOrganization();
            org.Country = TestHelper.GenerateRandomText(246);
            var context = new ValidationContext(org);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(org, context, results, true);

            Assert.False(isValid);
            results.Should().HaveCount(1);
            var result = results.First();
            Assert.Equal("The field Country must be a string with a maximum length of 245.", result.ErrorMessage);
            result.MemberNames.Should().HaveCount(1);
            Assert.Equal("Country", result.MemberNames.First());
        }
    }
}