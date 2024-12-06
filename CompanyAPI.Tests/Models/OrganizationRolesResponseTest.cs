using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CompanyAPI.Domain;
using FluentAssertions;
using Xunit;

namespace CompanyAPI.Tests.Models
{
    public class OrganizationRolesResponseTests
    {
        private static OrganizationRolesResponse CreateOrganizationRolesResponse() => new OrganizationRolesResponse
        {
            OrganizationId = 1,
            OrganizationName = "OrganizationName",
            City = "Curitiba (PR)",
            LogoUrl = "LogoUrl",
            Roles = new List<string>() {
                OrganizationRoleEnum.OrgAdmin.ToString(),
                OrganizationRoleEnum.Learner.ToString(),
                OrganizationRoleEnum.Facilitator.ToString(),
                OrganizationRoleEnum.InstructionalDesigner.ToString(),
                OrganizationRoleEnum.ReportingUser.ToString()
            }
        };

        [Fact]
        public void OrganizationRolesResponse_Valid()
        {
            var organizationRolesResponse = CreateOrganizationRolesResponse();
            var context = new ValidationContext(organizationRolesResponse);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(organizationRolesResponse, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }
        
    }
}