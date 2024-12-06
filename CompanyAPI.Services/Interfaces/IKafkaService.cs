using System.Collections.Generic;
using System.Threading.Tasks;
using WLS.KafkaMessenger;

namespace CompanyAPI.Services.Interfaces
{
    public interface IKafkaService
    {
        Task<List<ReturnValue>> SendKafkaMessage(string id, string subject, object data, string topic = "");
    }
}
