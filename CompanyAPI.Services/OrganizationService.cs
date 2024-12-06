using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyAPI.Domain;
using CompanyAPI.Domain.Exceptions;
using CompanyAPI.Infrastructure.Interface;
using CompanyAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WLS.KafkaMessenger.Services;
using WLS.KafkaMessenger.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace CompanyAPI.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly ICompanyDbContext _iCompanyDbContext;
        private IKafkaMessengerService _kafkaService;
        private readonly ILogger<OrganizationService> _logger;

        public OrganizationService(ICompanyDbContext companyDbContext, 
            ILogger<OrganizationService> logger, IKafkaMessengerService kafkaService)
        {
            _iCompanyDbContext = companyDbContext;
            _logger = logger;
            _kafkaService = kafkaService;
        }

        [Obsolete("This endpoint that calls this service is not being used anywhere")]
        public IEnumerable<Organization> GetCustomOrganizations(int typeId = 1)
        {
            return _iCompanyDbContext.Organizations.AsNoTracking()
                .Where(x => x.CrunchbaseUuid == Guid.Empty && x.OrganizationTypeId == typeId).ToList();
        }

        //V4 Items
        public IEnumerable<OrganizationV4> GetOrganizationList(OrganizationV4Find request, out int count)
        {
            return _iCompanyDbContext.FindOrganizationV4(request, true, out count).Select((x) => new OrganizationV4
            {
                OrganizationId = x.OrganizationId,
                City = x.City,
                LogoUrl = x.LogoUrl,
                OrganizationName = x.OrganizationName,
                OrganizationType = (OrganizationTypeEnum)x.OrganizationTypeId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).OrderBy(x => x.OrganizationName).ToList();
        }

        public OrganizationV4 GetOrganizationV4(int organizationId)
        {
            var organization = _iCompanyDbContext.Organizations.FirstOrDefault(x => x.OrganizationId == organizationId);
            return OrganizationV4.ConvertFromOrganization(organization);
        }

        public async Task<OrganizationV4> AddOrganizationV4(OrganizationAddRequestV4 request, bool useFullText = true)
        {
            int typeId = 1;
            if (!OrganizationTypeEnumHelper.ValidateEnum(request.OrganizationType, out typeId, out OrganizationTypeEnum organizationTypeEnum))
            {
                throw new Exception("Invalid Organization Type Provided", new BadRequestException());
            }

            var organization = new Organization
            {
                OrganizationName = request.OrganizationName,
                OrganizationTypeId = typeId,
                City = request.City,
                LogoUrl = request.LogoUrl
            };

            var result = await _iCompanyDbContext.AddOrganizationV4(organization, true, useFullText);
            switch (result)
            {
                case SaveOrganizationResult.Success:
                    // Save and return object
                    _iCompanyDbContext.SaveChanges();
                    await _kafkaService.SendKafkaMessage(organization.OrganizationId.ToString(), "OrganizationUpdated", organization, "ck-phoenix-organization");
                    return OrganizationV4.ConvertFromOrganization(organization);
                case SaveOrganizationResult.ErrorDuplicate:
                    // Return object without saving
                    throw new ConflictException($"Organization '{request.OrganizationName} - {request.City} - {request.OrganizationType}' already exists.");
                default:
                    return null;
            }
        }

        public async Task<OrganizationV4> UpdateOrganizationV4(OrganizationUpdateRequestV4 request, int organizationId, bool useFullText = true)
        {
            // TODO: Move logic to ICompanyDbContext and use SaveOrganizationResult enum, like AddOrganization does?
            var organization = await _iCompanyDbContext.Organizations.FirstOrDefaultAsync(x => x.OrganizationId == organizationId);

            if (organization != null)
            {   
                // Prevent duplicates (i.e. different OrganizationId, same OrganizationName AND City and OrganizationTYpe)
                Organization organizationLookup;
                if (useFullText)
                {
                    // Use Full Text index Search to speed up the search by Name
                    organizationLookup = _iCompanyDbContext.Organizations.FromSqlRaw(
                            $"SELECT * FROM Organizations " +
                            $"WHERE MATCH(OrganizationName) AGAINST (@p0 IN BOOLEAN MODE) " +
                            $"AND LCASE(OrganizationName) = CONVERT(LCASE(@p1) USING utf8mb4) COLLATE utf8mb4_bin " +
                            $"AND LCASE(IFNULL(City, '')) = CONVERT(LCASE(@p2) USING utf8mb4) COLLATE utf8mb4_bin " +
                            $"AND OrganizationId != @p3 " +
                            $"AND OrganizationTypeId = @p4",
                            "\"" + request.OrganizationName + "\"", request.OrganizationName, request.City ?? "", organizationId, organization.OrganizationTypeId) 
                        .AsNoTracking().FirstOrDefault();
                }
                else
                {
                    //Use "Contains" to search in Unit tests(cannot use "FromSql" against an in-memory database)
                    organizationLookup = _iCompanyDbContext.Organizations.Where(
                        o => o.OrganizationId != organization.OrganizationId &&
                                o.OrganizationName.Equals(request.OrganizationName) &&
                                o.City.Equals(request.City) &&
                                o.OrganizationTypeId == organization.OrganizationTypeId)
                        .AsEnumerable().FirstOrDefault(o =>
                                o.OrganizationName.Equals(request.OrganizationName, StringComparison.OrdinalIgnoreCase) &&
                                o.City.Equals(request.City, StringComparison.OrdinalIgnoreCase));
                }

                if (organizationLookup == null)
                {
                    organization.OrganizationName = request.OrganizationName;
                    organization.City = request.City;
                    organization.UpdatedAt = DateTime.Now;
                    organization.LogoUrl = request.LogoUrl;

                    //set organization history data
                    OrganizationHistory organizationHistory = OrganizationHistory.CreateOrganizationHistory(organization);
                    organizationHistory.UpdatedBy = -1; //set to userID making change when that is passed.


                    _iCompanyDbContext.OrganizationsHistory.Add(organizationHistory);

                    _iCompanyDbContext.SaveChanges();
                    await _kafkaService.SendKafkaMessage(organization.OrganizationId.ToString(), "OrganizationUpdated", organization, "ck-phoenix-organization");
                    return OrganizationV4.ConvertFromOrganization(organization);
                }

                throw new ConflictException($"Organization '{request.OrganizationName} - {request.City} - {organization.OrganizationTypeId}' already exists.");
            }
            else
            {
                return null;
            }
        }

        public bool DeleteOrganizationV4(int organizationId, int userId)
        {
            return _iCompanyDbContext.DeleteOrganizationV4(organizationId, userId);
        }
        

        public async Task GenerateKafkaEvents()
        {
            List<Organization> organizations = _iCompanyDbContext.Organizations.ToList();
            _logger.LogInformation($"GenerateKafkaEvents - Generate {organizations.Count} events");
            foreach(var organization in organizations)
            {
                await _kafkaService.SendKafkaMessage(organization.OrganizationId.ToString(), "OrganizationUpdated", organization, "ck-phoenix-organization");
            }
        }
    }
}