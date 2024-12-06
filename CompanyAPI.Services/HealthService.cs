using System;
using System.Collections.Generic;
using System.Linq;
using CompanyAPI.Domain;
using CompanyAPI.Domain.Interface;
using CompanyAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using WLS.Monitoring.HealthCheck.Interfaces;
using WLS.Monitoring.HealthCheck.Models;

namespace CompanyAPI.Services
{
    public class HealthService : IHealthService
    {
        private readonly IAppConfig _configuration;
        private readonly IDbHealthCheck _dbHealthCheck;
        private readonly ILogger<HealthService> _logger;

        public HealthService(IAppConfig configuration, ILogger<HealthService> logger, IDbHealthCheck dbHealthCheck)
        {
            _configuration = configuration;
            _logger = logger;
            _dbHealthCheck = dbHealthCheck;
        }

        public bool PerformHealthCheck()
        {
            Dictionary<string, string> result = CheckMySqlConnection();
            return CheckDependenciesResult(result);
        }

        public Dictionary<string, string> VerifyDependencies()
        {
            Dictionary<string, string> result = CheckMySqlConnection();
            return result;
        }

        private Dictionary<string, string> CheckMySqlConnection()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                var connectionString = _configuration.ConnectionString;
                DbHealthCheckResponse mysqlCheck = _dbHealthCheck.MySqlConnectionTest(connectionString);

                result.Add(DependenciesTypes.MySql,
                    mysqlCheck.SuccessfulConnection ? HealthResults.OK : HealthResults.Unavailable);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to reach MySql Server database, {0}", ex.Message);
                result.Add(DependenciesTypes.MySql, HealthResults.Unavailable);
            }

            return result;
        }

        public bool CheckDependenciesResult(Dictionary<string, string> results)
        {
            return results?.All(KeyValuePair => KeyValuePair.Value.Equals(HealthResults.OK)) ?? false;
        }
    }
}