using CompanyAPI.Domain;
using System;

namespace CompanyAPI.Services
{
    public class OrganizationTypeEnumHelper
    {

        /// <summary>
        /// This will validade if the string is a valid enum value, return the "id" and Enum option selected, alongside a true if the validation was successfull
        /// </summary>
        /// <param name="val"></param>
        /// <param name="id"></param>
        /// <param name="organizationTypeEnum"></param>
        /// <returns>bool true if val a valid enum value</returns>
        public static bool ValidateEnum(string val, out int id, out OrganizationTypeEnum organizationTypeEnum)
        {
            id = 1;
            organizationTypeEnum = OrganizationTypeEnum.Learner;

            if (val != null)
            {
                if (!Enum.TryParse<OrganizationTypeEnum>(val, true, out organizationTypeEnum))
                {
                    return false;
                }

                if (!Enum.IsDefined<OrganizationTypeEnum>(organizationTypeEnum))
                {
                    return false;
                }

                id = (int)organizationTypeEnum;
            }

            return true;
        }
    }
}