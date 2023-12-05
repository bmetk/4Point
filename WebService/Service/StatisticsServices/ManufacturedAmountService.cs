using System.Text;
using Common.Classes;
using Common.Extensions;
using Common.Interfaces.Service;
using WebService.Models;

namespace WebService.Service.StatisticsServices;

public class ManufacturedAmountService : AbstractObservableService<ProcessedData<int>>, IAsyncDisposable
{
    private readonly ILogger<ManufacturedAmountService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMqttService _mqttService;

    private bool _hasProductFinished;
    private bool _hasErrorOccured;

    private SlimEventSignaler? _resetSignal;
    public SlimEventSignaler? ResetSignal
    {
        private get => _resetSignal;
        set
        {
            _resetSignal = value;
            _resetSignal?.AddTask(() =>
            {
                _hasErrorOccured = _hasProductFinished = false;
                return Task.CompletedTask;
            });
        }
    }

    public ManufacturedAmountService(
        ILogger<ManufacturedAmountService> logger,
        IConfiguration configuration,
        IMqttService mqttService)
    {
        _logger = logger;
        _configuration = configuration;
        _mqttService = mqttService;

        _mqttService.SubscribeAsync(MqttTopics.WorkInProgressTopic(_configuration), ProductionStarted);
        _mqttService.SubscribeAsync(MqttTopics.DoneTopic(_configuration), ProductionComplete);
        _mqttService.SubscribeAsync(MqttTopics.ErrorTopic(_configuration), ProductionIssue);
    }

    private async void ProductionIssue(string topic, MqttMessage payload)
    {
        if(payload.Payload.AsUTF8String() == "0") return;
        if(_hasErrorOccured) return;
        
        _hasErrorOccured = true;
        _logger.LogInformation("Production failure");
        try
        {
            await this.NotifyObservers(new ProcessedData<int>(new TopicData(topic, payload), 0));
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.ToString());
        }
        finally
        {
            _hasProductFinished = false;
        }
    }

    private void ProductionComplete(string topic, MqttMessage payload)
    {
        if(payload.Payload.AsUTF8String() == "0") return;
        _logger.LogInformation("Production may be completed");
        _hasProductFinished = true;
    }

    private async void  ProductionStarted(string topic, MqttMessage payload)
    {
        if(payload.Payload.AsUTF8String() == "0") return;
        if(!_hasProductFinished) return;
        _hasErrorOccured = false;
        _logger.LogInformation("Production has completed");
        try
        {
            await NotifyObservers(new ProcessedData<int>(new TopicData(topic, payload), 1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        finally
        {
            _hasProductFinished = false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _mqttService.UnsubscribeAsync(MqttTopics.WorkInProgressTopic(_configuration), ProductionStarted);
        await _mqttService.UnsubscribeAsync(MqttTopics.DoneTopic(_configuration), ProductionComplete);
        await _mqttService.UnsubscribeAsync(MqttTopics.ErrorTopic(_configuration), ProductionIssue);
    }
}