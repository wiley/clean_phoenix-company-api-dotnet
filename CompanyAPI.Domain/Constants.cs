namespace CompanyAPI.Domain
{
    public static class Constants
    {
        // API Tokens
        public const string ApiToken = "";
        public const string ImageApiToken = "";
        public const string CrunchbaseImportApiToken = "";
        public const string CscApiToken = "";

        // Crunchbase API Settings
        public const string CrunchbaseApiKey = "c545c38d34da5f5014423bc2d5db03d8";
        public const string CrunchbaseLogoBaseUrl = "https://crunchbase-production-res.cloudinary.com/image/upload/c_lpad,h_120,w_120,f_auto,b_white,q_auto:eco/";
        public const int CrunchbaseApiMinItemsPerPage = 1;
        public const int CrunchbaseApiMaxItemsPerPage = 1000;
        public const int CustomOrganizationsListMaxSize = 100000;

        // Search Settings
        public const int MinSearchKeywordLength = 1;
        public const int DefaultSearchResultCount = 10;
        public const int MaxSearchResultsPerPage = 250;
    }

    public class DependenciesTypes
    {
        public const string MySql = "MySql";
    }

    public class HealthResults
    {
        public const string OK = "OK";
        public const string Unavailable = "Unavailable";
        public const string Fail = "Fail";
    }
}