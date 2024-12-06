using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CompanyAPI.Domain;
using CompanyAPI.Domain.Exceptions;
using CompanyAPI.Services;
using CompanyAPI.Services.Interfaces;
using DarwinAuthorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NSwag.Annotations;
using WLS.Log.LoggerTransactionPattern;

namespace CompanyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/organizations")]
    [Produces("application/json")]
    [ApiController]
    [ApiVersion("4.0")]
    [OpenApiTag("Organizations")]
    public class OrganizationsV4Controller : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly ILogger<OrganizationsV4Controller> _logger;
        private readonly ILoggerStateFactory _loggerStateFactory;
        private readonly DarwinAuthorizationContext _darwinAuthorizationContext;

        public OrganizationsV4Controller(
            IOrganizationService organizationService,
            ILogger<OrganizationsV4Controller> logger,
            ILoggerStateFactory loggerStateFactory,
            DarwinAuthorizationContext darwinAuthorizationContext
        )
        {
            _organizationService = organizationService;
            _logger = logger;
            _loggerStateFactory = loggerStateFactory;
            _darwinAuthorizationContext = darwinAuthorizationContext;
        }
        
        [HttpPut("generate-kafka-events")]
        [Authorize]
        [ProducesResponseType(202)]
        public IActionResult GenerateKafkaEvents()
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                try
                {
                    _ = _organizationService.GenerateKafkaEvents();

                    return Accepted();
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GenerateKafkaEvents - Unhandled Exception");
                    return StatusCode(500);
                }
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrganizationV4>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize]
        public IActionResult GetOrganizations([FromQuery] int size, [FromQuery] int offset,
            [FromQuery] string organizationName,
            [FromQuery] string city, [JsonConverter(typeof(StringEnumConverter))] OrganizationTypeEnum organizationType)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                _logger.LogInformation(
                    "OrganizationV4 - GetOrganizations - {size}, {offset}, {organizationName}, {city}, {type}",
                    size, offset, organizationName, city, organizationType);

                try
                {
                    // Pagination Parameters
                    if (size <= 0)
                        size = Constants.DefaultSearchResultCount;
                    if (size > Constants.MaxSearchResultsPerPage)
                        size = Constants.MaxSearchResultsPerPage;

                    if (offset < 0)
                        offset = 0;

                    int count = 0;
                    var organizationList = _organizationService.GetOrganizationList(new OrganizationV4Find()
                    {
                        offset = offset,
                        size = size,
                        OrganizationName = organizationName,
                        City = city,
                        OrganizationType = organizationType
                    }, out count);

                    PaginationHelperV4<OrganizationV4> paginationHelper = new PaginationHelperV4<OrganizationV4>
                        (size, offset, count);
                    paginationHelper.ShowCount = true;

                    var paginatedResult = paginationHelper.ReturnContent(HttpContext.Request, organizationList);

                    return Ok(paginatedResult);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        "OrganizationV4 - GetOrganizations Fail - {size}, {offset}, {organizationName}, {city}, {type}",
                        size, offset, organizationName, city, organizationType);
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
        }

        [HttpGet("{organizationId}")]
        [ProducesResponseType(typeof(OrganizationV4), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [Authorize]
        public IActionResult GetOrganization(int organizationId)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                _logger.LogInformation("OrganizationV4 - GetOrganization - {organizationId}", organizationId);

                try
                {
                    var organization = _organizationService.GetOrganizationV4(organizationId);

                    if (organization == null)
                    {
                        _logger.LogWarning("OrganizationV4 - GetOrganization - Not Found - {organizationId}",
                            organizationId);
                        return NotFound("The requested resource does not exist.");
                    }
                    else
                    {
                        organization.CreateSelfLinkV4(HttpContext.Request);

                        return Ok(organization);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "OrganizationV4 - GetOrganization Fail - {organizationId}", organizationId);
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrganizationV4), 200)]
        [ProducesResponseType(typeof(OrganizationV4), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [Authorize]
        public async Task<IActionResult> AddOrganization([FromBody] OrganizationAddRequestV4 request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                if (request == null)
                {
                    _logger.LogWarning("OrganizationV4 - AddOrganization - Bad Request Object");
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("OrganizationV4 - AddOrganization - Invalid Model State");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation(
                    "OrganizationV4 - AddOrganization - {request.OrganizationName}, {request.City}, {request.LogoUrl}, {request.OrganizationType}",
                    request.OrganizationName, request.City, request.LogoUrl, request.OrganizationType);

                try
                {
                    var organization = await _organizationService.AddOrganizationV4(request);

                    if (organization != null)
                    {
                        organization.CreateSelfLinkV4(HttpContext.Request, organization.GetUniqueID());
                        return Created(organization._links.Self.Href, organization);
                    }
                    else
                    {
                        _logger.LogWarning("OrganizationV4 - AddOrganization - Null Returned");
                        return BadRequest();
                    }
                }
                catch (ConflictException e)
                {
                    _logger.LogWarning(e,
                        "OrganizationV4 - AddOrganization - Conflict - {request.OrganizationName}, {request.City}, {request.LogoUrl}, {request.OrganizationType}",
                        request.OrganizationName, request.City, request.LogoUrl, request.OrganizationType);
                    return Conflict(e.Message);
                }
                catch (Exception e)
                {
                    if (e.InnerException is BadRequestException)
                    {
                        _logger.LogWarning(e,
                            "OrganizationV4 - AddOrganization - Fail - {request.OrganizationName}, {request.City}, {request.LogoUrl}, {request.OrganizationType}",
                            request.OrganizationName, request.City, request.LogoUrl, request.OrganizationType);

                        return BadRequest(e.Message);
                    }
                    else
                    {
                        _logger.LogError(e,
                            "OrganizationV4 - AddOrganization - Fail - {request.OrganizationName}, {request.City}, {request.LogoUrl}, {request.OrganizationType}",
                            request.OrganizationName, request.City, request.LogoUrl, request.OrganizationType);
                        return StatusCode((int)HttpStatusCode.InternalServerError);
                    }
                }
            }
        }

        [HttpPut("{organizationId}")]
        [ProducesResponseType(typeof(OrganizationV4), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [Authorize]
        public async Task<IActionResult> UpdateOrganization(int organizationId,
            [FromBody] OrganizationUpdateRequestV4 request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("OrganizationV4 - UpdateOrganization - Invalid Model State");
                    return BadRequest(ModelState);
                }

                if (request == null)
                {
                    _logger.LogWarning("OrganizationV4 - UpdateOrganization - Bad Request Object");
                    return BadRequest();
                }

                _logger.LogInformation(
                    "OrganizationV4 - UpdateOrganization - {request.OrganizationName}, {request.City}, {request.LogoUrl}",
                    request.OrganizationName, request.City, request.LogoUrl);

                try
                {
                    var organization = await _organizationService.UpdateOrganizationV4(request, organizationId);

                    if (organization != null)
                    {
                        if (request.OrganizationName.Equals(organization.OrganizationName))
                        {
                            organization.CreateSelfLinkV4(HttpContext.Request);
                            return Ok(organization);
                        }
                        else
                        {
                            _logger.LogWarning(
                                "OrganizationV4 - UpdateOrganization - Conflict - {request.OrganizationName}, {request.City}, {request.LogoUrl}",
                                request.OrganizationName, request.City, request.LogoUrl);
                            return Conflict("A duplicate organization exists.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("OrganizationV4 - UpdateOrganization - Not Found - {organizationId}",
                            organizationId);
                        return NotFound("The requested resource does not exist.");
                    }
                }
                catch (ConflictException e)
                {
                    _logger.LogWarning(e,
                        "OrganizationV4 - UpdateOrganization - Conflict - {request.OrganizationName}, {request.City}, {request.LogoUrl}",
                        request.OrganizationName, request.City, request.LogoUrl);
                    return Conflict(e.Message);
                }
                catch (Exception e)
                {
                    if (e.InnerException is BadRequestException)
                    {
                        _logger.LogWarning(e,
                            "OrganizationV4 - UpdateOrganization - Fail - {request.OrganizationName}, {request.City}, {request.LogoUrl}",
                            request.OrganizationName, request.City, request.LogoUrl);

                        return BadRequest(e.Message);
                    }
                    else
                    {
                        _logger.LogError(e,
                            "OrganizationV4 - UpdateOrganization - Fail - {request.OrganizationName}, {request.City}, {request.LogoUrl}",
                            request.OrganizationName, request.City, request.LogoUrl);

                        return StatusCode((int)HttpStatusCode.InternalServerError);
                    }
                }
            }
        }

        [HttpDelete("{organizationId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [Authorize]
        public IActionResult DeleteOrganization(int organizationId)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                var userId = _darwinAuthorizationContext.UserId;

                _logger.LogInformation("OrganizationV4 - DeleteOrganization - {organizationId}, {userId}",
                    organizationId, userId);

                try
                {
                    var organizationDeleted = _organizationService.DeleteOrganizationV4(organizationId, userId);
                    if (organizationDeleted == true)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return NotFound("The requested resource does not exist.");
                    }
                }
                catch (ForbiddenException e)
                {
                    _logger.LogWarning(e,
                        "OrganizationV4 - DeleteOrganization - Missing Permissions - {organizationId}, {userId}",
                        organizationId, userId);
                    return StatusCode((int)HttpStatusCode.Forbidden);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "OrganizationV4 - DeleteOrganization Fail - {organizationId}, {userId}",
                        organizationId, userId);
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}
