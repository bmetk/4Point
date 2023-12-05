using System.Text;
using Common.Classes;
using Common.Interfaces.Service;
using WebService.Models;

namespace WebService.Service;

public class ProductionFailureHandlerService : IDisposable
{
    private readonly ILogger<ProductionFailureHandlerService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMqttService _mqttService;
    private readonly Dictionary<string, Func<Task>> _jobs = new();

    public ProductionFailureHandlerService(
        ILogger<ProductionFailureHandlerService> logger,
        IConfiguration configuration,
        IMqttService mqttService)
    {
        _logger = logger;
        _configuration = configuration;
        _mqttService = mqttService;

        _mqttService.SubscribeAsync(MqttTopics.ErrorTopic(configuration), HandleError);
    }

    private async void HandleError(string topic, MqttMessage payload)
    {
        if(Encoding.UTF8.GetString(payload.Payload) == "0") return;
        _logger.LogInformation("Handling error from topic {TOPIC}", topic);
        foreach (var (_, job) in _jobs)
        {
            try { await job.Invoke(); }
            catch {  }
        }
    }

    public string RegisterJob(Action job)
    {
        return RegisterJob(() =>
        {
            job();
            return Task.CompletedTask;
        });
    }

    public string RegisterJob(Func<Task> job)
    {
        string guid = Guid.NewGuid().ToString();        
        
        _jobs.Add(guid, job);
        _logger.LogInformation("Adding job with an ID of {ID}", guid);
        return guid;
    }

    public void RemoveJobById(string id)
    {
        _logger.LogInformation("Removing job with an ID of {ID}", id);
        if (_jobs.ContainsKey(id)) _jobs.Remove(id);
    }

    public void Dispose()
    {
        _mqttService.UnsubscribeAsync(MqttTopics.ErrorTopic(_configuration), HandleError);
    }
}