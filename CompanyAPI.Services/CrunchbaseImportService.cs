using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CompanyAPI.Domain;
using CompanyAPI.Infrastructure.Interface;
using CsvHelper;

namespace CompanyAPI.Services
{
    public class CrunchbaseImportService
    {
        private readonly ICompanyDbContext _ICompanyDbContext;

        public CrunchbaseImportService(ICompanyDbContext companyDbContext)
        {
            _ICompanyDbContext = companyDbContext;
        }

        public async Task<bool> ImportFromCsv(string filePath)
        {
            const int SaveAtCount = 100000;
            DateTime dtStart = DateTime.Now;
            DateTime dtStop = DateTime.Now;
            TimeSpan tsDuration, tsElapsed, tsRemaining;
            bool initialLoad = false;
            int i = 0;
            double dbTotal, csvTotal, added = 0, remaining;
            int curRpm = 0, avgRpm = 0;
            double progress = 0;

            try
            {
                LogHelper.WriteLine("Starting CSV Import...");

                // NOTE: This is not 100% accurate when line breaks exist in CSV data
                csvTotal = File.ReadLines(filePath).Count();
                dbTotal = _ICompanyDbContext.Organizations.Count();

                if (dbTotal >= csvTotal)
                {
                    // CSV has already been imported
                    return true;
                }
                else if (dbTotal == 0)
                {
                    // If the database is empty, no need to check before inserting records
                    initialLoad = true;
                }

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader))
                {
                    // CSVHelper: https://joshclose.github.io/CsvHelper/
                    // You can ignore missing fields by setting MissingFieldFound to null
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        var org = new Organization
                        {
                            CrunchbaseUuid = csv.GetField<Guid>("uuid"),
                            OrganizationName = csv.GetField("name"),
                            Permalink = csv.GetField("permalink"),
                            CreatedAt = csv.GetField<DateTime>("created_at"),
                            UpdatedAt = csv.GetField<DateTime>("updated_at"),
                            Domain = csv.GetField("domain"),
                            HomepageUrl = csv.GetField("homepage_url"),
                            Country = csv.GetField("country_code"),
                            Region = csv.GetField("region"),
                            City = csv.GetField("city"),
                            ShortDescription = csv.GetField("short_description"),
                            LogoUrl = csv.GetField("logo_url")
                        };

                        if (await _ICompanyDbContext.AddOrganization(org, !initialLoad) == SaveOrganizationResult.Success)
                        {
                            // Only call SaveChanges every n rows for efficiency
                            if (++added % SaveAtCount == 0)
                            {
                                //LogHelper.WriteLine(string.Format("BEGIN SAVE AT {0}", DateTime.Now.ToString("MM/dd/yyyy h:mm tt")));
                                _ICompanyDbContext.SaveChanges();
                                //LogHelper.WriteLine(string.Format("END SAVE AT {0}", DateTime.Now.ToString("MM/dd/yyyy h:mm tt")));

                                tsDuration = DateTime.Now - dtStop;
                                tsElapsed = DateTime.Now - dtStart;
                                remaining = csvTotal - added;
                                progress = added / csvTotal;
                                curRpm = (int)(SaveAtCount / tsDuration.TotalMinutes);
                                avgRpm = (int)(added / (DateTime.Now - dtStart).TotalMinutes);
                                if (avgRpm == 0) avgRpm = curRpm;
                                tsRemaining = new TimeSpan(0, (int)(remaining / curRpm), 0);
                                var dtEstimatedFinish = DateTime.Now.AddMinutes(remaining / avgRpm);

                                LogHelper.WriteLine(string.Format("{0:#,0} records added in {1}m:{2}s",
                                    added, tsDuration.Minutes, tsDuration.Seconds));
                                LogHelper.WriteLine(string.Format("  Progress:		{0:P2}", progress));
                                LogHelper.WriteLine(string.Format("  Current rate:		{0:#,0}/m", curRpm));
                                LogHelper.WriteLine(string.Format("  Overall rate:		{0:#,0}/m", avgRpm));
                                LogHelper.WriteLine(string.Format("  Time elapsed:		{0}h:{1}m:{2}s",
                                    tsElapsed.Hours, tsElapsed.Minutes, tsElapsed.Seconds));
                                LogHelper.WriteLine(string.Format("  Time remaining:	{0}h:{1}m:{2}s",
                                    tsRemaining.Hours, tsRemaining.Minutes, tsRemaining.Seconds));
                                LogHelper.WriteLine(string.Format("  Est. finish:		{0}", dtEstimatedFinish.ToString("MM/dd/yyyy h:mm tt")));

                                dtStop = DateTime.Now;
                            }
                        }

                        i++;
                    }
                }

                if (added > 0) _ICompanyDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLine(ex.ToString());
                return false;
            }

            dtStop = DateTime.Now;
            tsDuration = dtStop - dtStart;
            LogHelper.WriteLine(String.Format("Process completed at {0}. Duration: {1}h, {2}m, {3}s, {4}ms. Records added: {5})",
                dtStop, tsDuration.Hours, tsDuration.Minutes, tsDuration.Seconds, tsDuration.Milliseconds, added));

            return true;
        }
    }
}