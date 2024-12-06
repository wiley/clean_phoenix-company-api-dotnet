using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CompanyAPI.Domain;
using FluentAssertions;
using Xunit;

namespace CompanyAPI.Tests.Models
{
    public class OrganizationRolesTests
    {
        [Fact]
        public void OrganizationRoleValid()
        {
            OrganizationRole role = new OrganizationRole
            {
                OrganizationRoleId = 1,
                Name = "RoleName",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,

            };
            var context = new ValidationContext(role);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(role, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void OrganizationUserRole_Valid()
        {
            OrganizationUserRole orguserrole = new OrganizationUserRole
            {
                OrganizationId = 1, 
                OrganizationRoleId = 1,
                UserId = 1,
                GrantedByUserId = 2,
                CreatedAt = DateTime.Now,

            };
            var context = new ValidationContext(orguserrole);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(orguserrole, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void OrganizationUserRoleHistory_Valid()
        {
            OrganizationUserRoleHistory orguserrolehistory = new OrganizationUserRoleHistory
            {
                OrganizationUserRoleHistoryId = 1,
                OrganizationId = 1,
                OrganizationRoleId = 1,
                UserId = 1,
                ChangedByUserId = 2,
                WasDeleted = true,
                CreatedAt = DateTime.Now,

            };
            var context = new ValidationContext(orguserrolehistory);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(orguserrolehistory, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
