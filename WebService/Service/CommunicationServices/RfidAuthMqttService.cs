using Common.Classes;
using Common.Extensions;
using Common.Interfaces.Service;
using WebService.Models;
using WebService.Service.StatisticsServices;

namespace WebService.Service.CommunicationServices;

public class RfidAuthMqttService : AbstractObservableService<string>, IDisposable
{
    private readonly ILogger<RfidAuthMqttService> _logger;
    private readonly IMqttService _mqttService;
    private string RFIDInMqttTopic { get; }

    public RfidAuthMqttService(
        ILogger<RfidAuthMqttService> logger, 
        IConfiguration configuration, 
        IMqttService mqttService)
    {
        _logger = logger;
        _mqttService = mqttService;
        RFIDInMqttTopic = MqttTopics.RFIDAuthTopic(configuration);

        _mqttService.SubscribeAsync(RFIDInMqttTopic, HandleRFIDAuthMessage);
    }

    private async void HandleRFIDAuthMessage(string topic, MqttMessage payload)
    {
        _logger.LogDebug("RFID key read: {DATA}", payload.Payload.AsUTF8String());
        var msg = payload.Payload.AsUTF8String();
        await NotifyObservers(msg);
    }
    
    public void Dispose()
    {
        _mqttService.UnsubscribeAsync(RFIDInMqttTopic, HandleRFIDAuthMessage);
    }
}