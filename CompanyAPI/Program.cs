﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace CompanyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(100, 100);
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(new ConfigurationBuilder().AddEnvironmentVariables(prefix: "ASPNETCORE_").Build())
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    IWebHostEnvironment env = builderContext.HostingEnvironment;
                    config.SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false)
                        .AddEnvironmentVariables();
                })
                .UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false)
                .UseStartup<Startup>();
    }
}
