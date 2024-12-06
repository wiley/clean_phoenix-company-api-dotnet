using CompanyAPI.Domain;
using CompanyAPI.Domain.Interface;
using CompanyAPI.Extensions;
using CompanyAPI.Infrastructure;
using CompanyAPI.Infrastructure.Interface;
using CompanyAPI.Services;
using CompanyAPI.Services.Interfaces;
using Confluent.Kafka;
using DarwinAuthorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

using Polly;
using Polly.Timeout;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WLS.KafkaMessenger.Infrastructure;
using WLS.KafkaMessenger.Infrastructure.Interface;
using WLS.KafkaMessenger.Services;
using WLS.KafkaMessenger.Services.Interfaces;
using WLS.Log.LoggerTransactionPattern;
using WLS.Monitoring.HealthCheck;
using WLS.Monitoring.HealthCheck.Interfaces;
using DarwinAuthorization;
using Newtonsoft.Json;

namespace CompanyAPI
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
            LogHelper.WriteLine("Starting application...");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDarwinAuthzConfiguration();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:sszzz";
            });

            var connectionString = Environment.GetEnvironmentVariable("COMPANYAPI_CONNECTION_STRING") ??
                                       Configuration.GetConnectionString("CompanyAPI");
			ConfigureDbContext(services, connectionString);
            ConfigureHttpClients(services);

            services.AddSingleton<IAppConfig>(cfg => new AppConfig()
            {
                ConnectionString = connectionString,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            });

            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IOrganizationUserRoleService, OrganizationUserRoleService>();
            services.AddScoped<ILoggerStateFactory, LoggerStateFactory>();
            services.AddScoped<IDbHealthCheck, DbHealthCheck>();
            services.AddScoped<IHealthService, HealthService>();
            services.AddScoped<IOrganizationUserRoleService, OrganizationUserRoleService>();
            services.AddSingleton<IKafkaMessengerService, KafkaMessengerService>();
            string host = Environment.GetEnvironmentVariable("KAFKA_HOST");
            var senders = new List<KafkaSender>
            {
                new KafkaSender
                {
                    Topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")
                }
            };
            services.AddSingleton<IKafkaConfig>(kc =>
                new KafkaConfig() { Host = host, Sender = senders, Source = "company-api" }
            );
            services.AddSingleton(p => new ProducerBuilder<string, string>(new ProducerConfig
            {
                BootstrapServers = host
            }).Build());
            services.AddScoped<IKafkaService, KafkaService>();
            services.AddSingleton<IKafkaMessengerService, KafkaMessengerService>();

            ConfigureResponseEncoding(services);
            ConfigureLogging(services);
            ConfigureSwagger(services);
            ConfigureVersioning(services);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerDocument(document =>
            {
                document.Version = "v1";
                document.DocumentName = "v1";
                document.Title = "WLS Company API";
                document.Description = "WLS Company API";
                document.DocumentProcessors.Add(new SecurityDefinitionAppender(
                    "ApiKey", Enumerable.Empty<string>(),
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "CompanyAPIToken",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "An API Key is required."
                    }));
                document.ApiGroupNames = new string[] { "v1" };
            });
            services.AddSwaggerDocument(document =>
            {
                document.Version = "v4";
                document.DocumentName = "v4";
                document.Title = "WLS Company API";
                document.Description = "WLS Company API";
                document.DocumentProcessors.Add(new SecurityDefinitionAppender(
                    "ApiKey", Enumerable.Empty<string>(),
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "CompanyAPIToken",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "An API Key is required."
                    }));
                document.DocumentProcessors.Add(new SecurityDefinitionAppender(
                   "Bearer", Enumerable.Empty<string>(),
                   new OpenApiSecurityScheme
                   {
                       Type = OpenApiSecuritySchemeType.ApiKey,
                       Name = "Authorization",
                       Scheme = "Bearer",
                       In = OpenApiSecurityApiKeyLocation.Header,
                       Description = "Enter 'Bearer <token>'."
                   }));
                document.ApiGroupNames = new string[] { "v4" };
            });
        }

        private static void ConfigureVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            // Changed in v3.x https://github.com/dotnet/aspnet-api-versioning/issues/330
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }

        private void ConfigureResponseEncoding(IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>(); //Brotli will be chosen first based upon order here
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });

            services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
        }

        private void ConfigureLogging(IServiceCollection services)
        {
            /* Switching to using "Serilog" log provider for everything
                NOTE: Call to ClearProviders() is what turns off the default Console Logging

                Output to the Console is now controlled by the WriteTo format below
                DevOps can control the Log output with environment variables
                    LOG_MINIMUMLEVEL - values like INFORMATION, WARNING, ERROR
                    LOG_JSON - true means to output log to console in JSON format
            */
            LogLevel level = LogLevel.None;
            var serilogLevel = new LoggingLevelSwitch
            {
                MinimumLevel = LogEventLevel.Information
            };

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOG_MINIMUMLEVEL")))
            {
                Enum.TryParse(Environment.GetEnvironmentVariable("LOG_MINIMUMLEVEL"), out level);
                LogEventLevel eventLevel = LogEventLevel.Information;
                Enum.TryParse(Environment.GetEnvironmentVariable("LOG_MINIMUMLEVEL"), out eventLevel);
                serilogLevel.MinimumLevel = eventLevel;
            }

            bool useJson = Environment.GetEnvironmentVariable("LOG_JSON") == "true";

            var config = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(Configuration);

            if (useJson)
                config.WriteTo.Console(new ElasticsearchJsonFormatter());
            else
                config.WriteTo.Console(outputTemplate: "[{Timestamp:MM-dd-yyyy HH:mm:ss.SSS} {Level:u3}] {Message:lj} {TransactionID}{NewLine}{Exception}", theme: SystemConsoleTheme.Literate);

            if (level != LogLevel.None)
                config.MinimumLevel.ControlledBy(serilogLevel);

            Log.Logger = config.CreateLogger();

            services.AddLogging(lb =>
            {
                lb.ClearProviders();
                lb.AddSerilog();
                lb.AddDebug(); //Write to VS Output window (controlled by appsettings "Logging" section)
            });
        }

        private static void ConfigureDbContext(IServiceCollection services, string connectionString)
        {
            //*** Comment below when using in-memory db ***
            //string connectionString = Environment.GetEnvironmentVariable("COMPANYAPI_CONNECTION_STRING");
            //if (string.IsNullOrEmpty(connectionString))
            //    connectionString = Configuration.GetConnectionString("CompanyAPI");

            // Set global command timeout (default=30s)
            const int commandTimeout = 30;
            services.AddDbContext<ICompanyDbContext, CompanyDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    o => o.CommandTimeout(commandTimeout));
            }, ServiceLifetime.Transient);

            //*** Comment below when using physical db ***
            //LogHelper.Write("Using in-memory database");
            //services.AddEntityFrameworkInMemoryDatabase();
            //services.AddDbContext<CompanyDbContext>(options =>
            //{
            //	options.UseInMemoryDatabase("CompanyAPI");
            //}, ServiceLifetime.Singleton);
        }

        private void ConfigureHttpClients(IServiceCollection services)
        {
            AddHttpClientWithPolicy(services, "ImageAPI", new List<HttpStatusCode>
            {
                HttpStatusCode.OK,
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.NotFound
            });
            AddHttpClientWithPolicy(services, "CrunchbaseApi", new List<HttpStatusCode>
            {
                HttpStatusCode.OK,
                HttpStatusCode.BadRequest,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.NotFound
            });
        }

        private void AddHttpClientWithPolicy(IServiceCollection services, string serviceName, IEnumerable<HttpStatusCode> acceptableStatuses)
        {
            services.AddHttpClient(serviceName, options =>
                {
                    options.BaseAddress = new Uri(Configuration[$"{serviceName}:BaseURL"]);

                    // 31 seconds. Overall Timeout includes four attempts (three retries plus initial)
                    // and variable wait delays between retries, and +3 seconds for timingPrecision error
                    options.Timeout = TimeSpan.FromSeconds(5 + 3 * 5 + (1 + 3 + 5) + 3);
                    options.DefaultRequestHeaders.Accept.Clear();
                    options.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .ConfigurePrimaryHttpMessageHandler(messageHandler =>
                {
                    var handler = new HttpClientHandler();
                    if (handler.SupportsAutomaticDecompression)
                    {
                        handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    }

                    return handler;
                })
                .AddPolicyHandler(
                    Policy.HandleResult<HttpResponseMessage>(r => !acceptableStatuses.Contains(r.StatusCode))
                        .Or<TimeoutRejectedException>()
                        .FallbackAsync(
                            new HttpResponseMessage(HttpStatusCode.InternalServerError),
                            onFallbackAsync: (delegateResult, context) =>
                            {
                                _logger.LogWarning($"Call to {serviceName} failed");
                                return Task.CompletedTask;
                            })
                        .WrapAsync(
                            Policy.HandleResult<HttpResponseMessage>(r => !acceptableStatuses.Contains(r.StatusCode))
                                .Or<TimeoutRejectedException>()
                                .WaitAndRetryAsync(
                                    new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5) }
                                    ,
                                    onRetry: (outcome, timespan, retryAttempt, context) =>
                                    {
                                        if (outcome.Exception is TimeoutRejectedException)
                                            _logger.LogWarning($"{serviceName} did not respond after attempt {retryAttempt}");
                                        else if (outcome.Result != null)
                                            _logger.LogWarning($"{serviceName} returned {(int)outcome.Result.StatusCode}");
                                    })
                                .WrapAsync(
                                    Policy.TimeoutAsync(TimeSpan.FromSeconds(5)))
                        )
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            // This approach was needed for an In Memory Database because it needed to be the same copy of the object
            var companyDbContext = app.ApplicationServices.GetService<ICompanyDbContext>();
            companyDbContext.Initialize();
            _logger.LogInformation("Starting database migration...");
            const string CsvFolder = @"Crunchbase\_csv\";
            // Windows: ..\wls-companyapi\CompanyAPI, Linux: /app
            string ImportFolder = Path.Combine(Directory.GetCurrentDirectory(), CsvFolder);
            string[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Contains("--import"))
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    var arg1 = arguments[i];
                    if (arg1.Contains("--import") && i < arguments.Length)
                    {
                        var arg2 = arguments[i + 1];
                        string filepath = Path.Combine(ImportFolder, arg2);

                        if (File.Exists(filepath))
                        {
                            var service = new CrunchbaseImportService(companyDbContext);
                            var task = service.ImportFromCsv(filepath);
                        }
                        else
                            _logger.LogWarning(string.Format("*** WARNING *** Import file '{0}' not found!", filepath));
                    }
                }
            }

            // *** NOTE: The code above works for a physical MySql instance too. Do we need the code below? ***

            // Because a MySql database context can be transient and initialization goes against the actual database 
            // and not a common in-memory object, then we can just create a new instance here:
            //var optionsBuilder = new DbContextOptionsBuilder<CompanyDbContext>();
            //optionsBuilder.UseMySql(Configuration.GetConnectionString("CompanyAPI"));
            //var companyDbContext = new CompanyDbContext(optionsBuilder.Options);
            //companyDbContext.Initialize();

            app.UseResponseCompression();
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseDarwinAuthenticationContext();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi(options =>
                {
                    options.PostProcess = (document, x) =>
                    {
                        document.Schemes = new[] { OpenApiSchema.Https };
                        document.Host = Environment.GetEnvironmentVariable("DOMAIN") ?? "";
                    };
                });

                app.UseSwaggerUi3(options =>
                {
                    options.Path = "/swagger";
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        string version = $"v{description.ApiVersion.MajorVersion}";
                        options.SwaggerRoutes.Add(new SwaggerUi3Route($"{version}", $"/swagger/{version}/swagger.json"));
                    }
                    options.DocumentPath = "swagger/{documentName}/swagger.json";
                });
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
        }
    }
}
