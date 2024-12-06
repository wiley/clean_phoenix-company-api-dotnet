namespace CompanyAPI.Domain.Interface
{
    public interface IAppConfig
    {
        string ConnectionString { get; set; }
        string Environment { get; set; }
    }
}