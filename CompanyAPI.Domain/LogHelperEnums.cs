namespace CompanyAPI.Domain
{
    public enum TimeFormat
    {
        None = -1,
        Standard = 0,
        Military = 1,
        UnixTime = 2
    }

    public enum TimeZone
    {
        Local = 0,
        Utc = 1
    }

    public enum MessageFormat
    {
        Standard = 1,
        Verbose = 2
    }
}
