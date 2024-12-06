using System.Collections.Generic;

namespace CompanyAPI.Services.Interfaces
{
    public interface IHealthService
    {
        bool PerformHealthCheck();
        public Dictionary<string, string> VerifyDependencies();
        bool CheckDependenciesResult(Dictionary<string, string> results);
    }
}