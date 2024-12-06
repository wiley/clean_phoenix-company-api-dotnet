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
    [Route("api/v{version:apiVersion}/organization-user-roles")]
    [Produces("application/json")]
    [ApiController]
    [ApiVersion("4.0")]
    [OpenApiTag("OrganizationUserRoles")]
    public class OrganizationUserRolesV4Controller : Controller
    {
        private readonly IOrganizationUserRoleService _organizationUserRoleService;
        private readonly ILogger<OrganizationUserRolesV4Controller> _logger;
        private readonly ILoggerStateFactory _loggerStateFactory;
        private readonly DarwinAuthorizationContext _darwinAuthorizationContext;

        public OrganizationUserRolesV4Controller(
            IOrganizationUserRoleService organizationUserRoleService,
            ILogger<OrganizationUserRolesV4Controller> logger,
            ILoggerStateFactory loggerStateFactory,
            DarwinAuthorizationContext darwinAuthorizationContext
        )
        {
            _organizationUserRoleService = organizationUserRoleService;
            _logger = logger;
            _loggerStateFactory = loggerStateFactory;
            _darwinAuthorizationContext = darwinAuthorizationContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrganizationUserRoleV4>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public IActionResult GetOrganizationUserRoles([FromQuery] int size, [FromQuery] int offset,
            [FromQuery] int userId,
            [FromQuery] int organizationId,
            [FromQuery] [JsonConverter(typeof(StringEnumConverter))] OrganizationRoleEnum organizationRoleId,
            [FromQuery] int CreatedBy)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                _logger.LogInformation(
                    "GetOrganizationUserRoles - {size}, {offset}, {userId}, {organizationId}, {organizationRoleId}, {CreatedBy}",
                    size, offset, userId, organizationId, organizationRoleId, CreatedBy);

                // Pagination Parameters
                if (size <= 0)
                    size = Constants.DefaultSearchResultCount;
                if (size > Constants.MaxSearchResultsPerPage)
                    size = Constants.MaxSearchResultsPerPage;

                if (offset < 0)
                    offset = 0;

                int count = 0;
                try
                {
                    var organizationUserRoleList = _organizationUserRoleService.SearchOrganizationUserRole(
                        new OrganizationUserRoleFind()
                        {
                            offset = offset,
                            size = size,
                            UserId = userId,
                            OrganizationId = organizationId,
                            OrganizationRoleId = organizationRoleId,
                            GrantedByUserId = CreatedBy
                        }, out count);

                    PaginationHelperV4<OrganizationUserRoleV4> paginationHelper =
                        new PaginationHelperV4<OrganizationUserRoleV4>
                            (size, offset, count);
                    paginationHelper.ShowCount = true;

                    var paginatedResult = paginationHelper.ReturnContent(HttpContext.Request, organizationUserRoleList);

                    return Ok(paginatedResult);
                }
                catch (ConflictException e)
                {
                    _logger.LogWarning(e,
                        "GetOrganizationUserRoles - Conflict - {size}, {offset}, {userId}, {organizationId}, {organizationRoleId}, {grantedByUserId}",
                        size, offset, userId, organizationId, organizationRoleId, CreatedBy);
                    return Conflict(e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e,
                        "GetOrganizationUserRoles - Fail - {size}, {offset}, {userId}, {organizationId}, {organizationRoleId}, {grantedByUserId}",
                        size, offset, userId, organizationId, organizationRoleId, CreatedBy);
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
        }

        [HttpGet("{organizationUserRoleId}")]
        [ProducesResponseType(typeof(OrganizationUserRoleV4), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [Authorize]
        public async Task<IActionResult> GetOrganizationUserRole(int organizationUserRoleId)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                _logger.LogInformation("GetOrganizationUserRoleV4 - {organizationUserRoleId}", organizationUserRoleId);

                try
                {
                    var organizationUserRole =
                        await _organizationUserRoleService.GetOrganizationUserRole(organizationUserRoleId);

                    organizationUserRole.CreateSelfLinkV4(HttpContext.Request);

                    return Ok(organizationUserRole);
                }
                catch (NotFoundException e)
                {
                    _logger.LogWarning("GetOrganizationUserRoleV4 - Not Found - {organizationUserRoleId}",
                        organizationUserRoleId);
                    return NotFound("The requested resource does not exist.");
                }
                catch (ConflictException e)
                {
                    _logger.LogWarning(e, "GetOrganizationUserRoleV4 - Conflict - {organizationUserRoleId}",
                        organizationUserRoleId);
                    return Conflict(e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "GetOrganizationUserRoleV4 - Fail - {organizationUserRoleId}",
                        organizationUserRoleId);
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrganizationUserRoleV4), 200)]
        [ProducesResponseType(typeof(OrganizationUserRoleV4), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [Authorize]
        public async Task<IActionResult> AddOrganizationUserRole([FromBody] OrganizationUserRoleAddRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                if (request == null)
                {
                    _logger.LogWarning("AddOrganizationUserRole - Bad Request Object");
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("AddOrganizationUserRole - Invalid Model State");
                    return BadRequest(ModelState);
                }

                if (!Enum.IsDefined<OrganizationRoleEnum>(request.Role))
                {
                    _logger.LogWarning("AddOrganizationUserRole - Invalid Role");
                    return BadRequest("The Role is invalid.");
                }

                _logger.LogInformation(
                    "AddOrganizationUserRole - {request.UserId}, {request.OrganizationId}, {request.Role}",
                    request.UserId, request.OrganizationId, request.Role);

                try
                {
                    request.GrantedByUserId = _darwinAuthorizationContext.UserId;

                    var organizationUserRole = await _organizationUserRoleService.AddOrganizationUserRoleV4(request);

                    organizationUserRole.CreateSelfLinkV4(HttpContext.Request, organizationUserRole.GetUniqueID());
                    return Created(organizationUserRole._links.Self.Href, organizationUserRole);
                }
                catch (ConflictException e)
                {
                    _logger.LogWarning(e,
                        "AddOrganizationUserRole - Conflict - {request.UserId}, {request.OrganizationId}, {request.Role}",
                        request.UserId, request.OrganizationId, request.Role);
                    return Conflict(e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e,
                        "AddOrganizationUserRole - Fail - {request.UserId}, {request.OrganizationId}, {request.Role}",
                        request.UserId, request.OrganizationId, request.Role);
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
        }

        [HttpDelete("{organizationUserRoleId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [Authorize]
        public async Task<IActionResult> DeleteOrganizationUserRole(int organizationUserRoleId)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                _logger.LogInformation("DeleteOrganizationUserRole - {organizationUserRoleId}", organizationUserRoleId);
                try
                {
                    var organizationDeleted =
                        await _organizationUserRoleService.DeleteOrganizationUserRole(organizationUserRoleId);
                    return NoContent();
                }
                catch (NotFoundException e)
                {
                    _logger.LogWarning("DeleteOrganizationUserRole - Not Found - {organizationUserRoleId}",
                        organizationUserRoleId);
                    return NotFound("The requested resource does not exist.");
                }
                catch (ConflictException e)
                {
                    _logger.LogWarning(e, "DeleteOrganizationUserRole - Conflict - {organizationUserRoleId}",
                        organizationUserRoleId.ToString());
                    return Conflict(e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "DeleteOrganizationUserRole - Fail - {organizationUserRoleId}",
                        organizationUserRoleId.ToString());
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}