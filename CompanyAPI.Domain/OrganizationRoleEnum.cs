using System.Collections.Generic;
using System;
using System.Linq;

namespace CompanyAPI.Domain
{
    public enum OrganizationRoleEnum
    {
        OrgAdmin = 1,
        Learner = 2,
        Facilitator = 3,
        InstructionalDesigner = 4,
        ReportingUser = 5
    }

    //Class to transform the Enum into a dictionary, for ease of access to values/keys
    public class OrganizationRoleDict
    {
        public static Dictionary<int, string> EnumToDictByKey()
        {
            return Enum.GetValues(typeof(OrganizationRoleEnum))
               .Cast<OrganizationRoleEnum>()
               .ToDictionary(t => (int)t, t => t.ToString());
        }

        public static Dictionary<string, int> EnumToDictByValue()
        {
            return Enum.GetValues(typeof(OrganizationRoleEnum))
               .Cast<OrganizationRoleEnum>()
               .ToDictionary(t => t.ToString(), t => (int)t);
        }
    }
}
