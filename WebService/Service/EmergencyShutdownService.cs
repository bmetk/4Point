using System.Text;
using Common.Interfaces.Service;

namespace WebService.Service;

public class EmergencyShutdownService : IHostedService
{
    private readonly IMqttService _mqttService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmergencyShutdownService> _logger;

    public EmergencyShutdownService(
        IMqttService mqttService, 
        IConfiguration configuration,
        ILogger<EmergencyShutdownService> logger)
    {
        _mqttService = mqttService;
        _configuration = configuration;
        _logger = logger;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        string[] publishTopics =
        {
            Models.MqttTopics.AuthTopic(_configuration),
            Models.MqttTopics.ErrorCheckedTopic(_configuration)
        };
        Task[] tasks = new Task[publishTopics.Length];
        int idx = 0;
        foreach (var topic in publishTopics)
        {
            tasks[idx++] = _mqttService.PublishAsync(topic, Encoding.UTF8.GetBytes("0"));
            _logger.LogInformation($"Switched off relay on topic {topic}");
        }

        Task.WaitAll(tasks);
        return Task.CompletedTask;
    }
}