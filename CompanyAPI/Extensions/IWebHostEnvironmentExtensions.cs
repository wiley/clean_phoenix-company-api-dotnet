using Microsoft.AspNetCore.Hosting;

namespace CompanyAPI.Extensions
{
	public static class IWebHostEnvironmentExtensions
	{
		public static bool IsDevelopment(this IWebHostEnvironment webHostEnvironment)
		{
			return webHostEnvironment.EnvironmentName.ToLower() == "development";
		}

		public static bool IsStaging(this IWebHostEnvironment webHostEnvironment)
		{
			return webHostEnvironment.EnvironmentName.ToLower() == "staging";
		}
	}
}
