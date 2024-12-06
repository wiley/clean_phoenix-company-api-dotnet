using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WLS.KafkaMessenger.Services.Interfaces;
using WLS.KafkaMessenger;
using CompanyAPI.Services.Interfaces;

namespace CompanyAPI.Services
{
    public class KafkaService : IKafkaService
    {
        private readonly IKafkaMessengerService _kafkaMessengerService;
        private readonly ILogger<KafkaService> _logger;

        public KafkaService(IKafkaMessengerService kafkaMessengerService, ILogger<KafkaService> logger)
        {
            _kafkaMessengerService = kafkaMessengerService;
            _logger = logger;
        }

        public async Task<List<ReturnValue>> SendKafkaMessage(string id, string subject, object data, string topic = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(topic))
                    return await _kafkaMessengerService.SendKafkaMessage(id, subject, data, topic);
                else
                    return await _kafkaMessengerService.SendKafkaMessage(id, subject, data);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SendKafkaMessage - Error - id:{id}, subject:{subject}, topic: {topic}", id, subject, topic);
                return null;
            }
        }
    }
}
