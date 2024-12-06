using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyAPI.Domain;
using CompanyAPI.Domain.Interface;
using CompanyAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyAPI.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [ApiController]
    public class HealthController : Controller
    {
        private readonly IHealthService _healthService;
        private readonly IAppConfig _appConfig;

        public HealthController(IHealthService healthCheckService, IAppConfig config)
        {
            _healthService = healthCheckService;
            _appConfig = config;
        }

        [Route("api/v{version:apiVersion}/Health")]
        [ProducesResponseType(typeof(HealthResponseLegacy), 200)]
        [HttpGet]
        [Obsolete("Kept during transition, to be removed at a later date")]
        public IActionResult Health()
        {
            HealthResponseLegacy response = new HealthResponseLegacy();

            //TODO: Check status of one or more dependencies
            //i.e. EPIC Forgot Password API call...

            response.Status = "Healthy";
            response.Environment = _appConfig.Environment;

            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(HealthResponse), 200)]
        [ProducesResponseType(typeof(HealthResponseExtended), 503)]
        [Route("/Healthz")]
        public IActionResult HealthCheck()
        {
            if (_healthService.PerformHealthCheck())
            {
                return Ok(new HealthResponse { Status = HealthResults.OK });
            }

            return StatusCode(StatusCodes.Status503ServiceUnavailable, new HealthResponseExtended { Status = HealthResults.Fail, Message = "Unable to connect to database" });
        }

        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, string>), 200)]
        [ProducesResponseType(typeof(Dictionary<string, string>), 503)]
        [Route("/Healthz/Dependencies")]
        public async Task<IActionResult> HealthCheckDependenciesAsync()
        {
            var result = _healthService.VerifyDependencies();

            if (_healthService.CheckDependenciesResult(result))
            {
                return Ok(result);
            }

            return StatusCode(StatusCodes.Status503ServiceUnavailable, result);
        }
    }
}