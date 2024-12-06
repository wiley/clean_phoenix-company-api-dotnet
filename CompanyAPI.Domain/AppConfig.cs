using CompanyAPI.Domain.Interface;

namespace CompanyAPI.Domain
{
    public class AppConfig : IAppConfig
    {
        public string ConnectionString { get; set; }
        public string Environment { get; set; }
    }
}
