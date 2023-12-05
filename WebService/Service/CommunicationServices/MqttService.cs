using System.Text;
using Common.Classes;
using Common.Delegates;
using Common.Interfaces.Service;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter.V5;
using MQTTnet.Packets;

namespace WebService.Service.CommunicationServices;

public class MqttService : IMqttService, IDisposable
{
    private readonly ILogger<MqttService> _logger;
    private readonly IManagedMqttClient _client;
    private readonly ManagedMqttClientOptions _mqttOptions;

    private readonly Dictionary<string, List<MqttMessageHandlerDelegate>> _handlers = new();
    
    public MqttService(IConfiguration configuration, ILogger<MqttService> logger)
    {
        _logger = logger;

        var mqttHost = configuration.GetSection("MQTT_HOST").Get<string>();
        _logger.LogInformation($"Mqtt host is {mqttHost}");
        
        _client = new MqttFactory().CreateManagedMqttClient();
        _client.ApplicationMessageReceivedAsync += _ClientOnApplicationMessageReceivedAsync;
        _mqttOptions = new ManagedMqttClientOptionsBuilder()
            .WithClientOptions(new MqttClientOptionsBuilder()
                .WithTcpServer(mqttHost)
                .Build())
            .Build();
    }

    public async Task PublishAsync(string topic, byte[] message)
    {
        await _StartClientIfNeeded(_client);
        await _PublishAsync(topic, message);
    }

    public async Task SubscribeAsync(string topic, MqttMessageHandlerDelegate callback)
    {
        await _StartClientIfNeeded(_client);
        await _SubscribeAsync(topic, callback);
    }

    public async Task UnsubscribeAsync(string topic, MqttMessageHandlerDelegate callback)
    {
        await _StartClientIfNeeded(_client);
        await _UnsubscribeAsync(topic, callback);
    }

    private Task _ClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var topic = arg.ApplicationMessage.Topic;
        _logger.LogDebug("Message received on topic {topic}: {message}", topic, Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment));
        var data = arg.ApplicationMessage.PayloadSegment;

        var msg = new MqttMessage(data).WithCurrentTimestamp();
        if (_handlers.TryGetValue(topic, out var cbs))
        {
            cbs.ForEach(cb => cb.Invoke(topic, msg));
        }
        return Task.CompletedTask;
    }    

    private async Task _StartClientIfNeeded(IManagedMqttClient client)
    {
        if (!client.IsStarted)
        {
            await client.StartAsync(_mqttOptions);
        }
    }
    
    private async Task _PublishAsync(string topic, byte[] message)
    {
        _logger.LogDebug($"Publishing on topic {topic}");
        MqttApplicationMessage payload = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(message)
            .Build();
        await this._client.EnqueueAsync(payload);
    }

    private async Task _SubscribeAsync(string topic, MqttMessageHandlerDelegate callback)
    {
        _logger.LogDebug($"Adding handler to topic {topic}");
        MqttTopicFilterBuilder topicBuilder = new MqttTopicFilterBuilder().WithTopic(topic);
        var shouldSubscribe = _AddHandler(topic, callback);

        if (shouldSubscribe)
        {
            await _client.SubscribeAsync(new List<MqttTopicFilter> { topicBuilder.Build() });
        }
    }

    private async Task _UnsubscribeAsync(string topic, MqttMessageHandlerDelegate callback)
    {
        _logger.LogDebug($"Removing handler of topic {topic}");
        if (_handlers.TryGetValue(topic, out var cbs))
        {
            cbs.Remove(callback);
            if (cbs.Count == 0)
            {
                _handlers.Remove(topic);
                _logger.LogInformation($"Removing last handler for {topic}, unsubscribing");
                await _client.UnsubscribeAsync(new List<string> { topic });
            }
        }
    }

    private bool _AddHandler(string topic, MqttMessageHandlerDelegate callback)
    {
        if (_handlers.TryGetValue(topic, out var cbs))
        {
            cbs.Add(callback);
            return false;
        }
        _handlers.Add(topic, new[]{ callback }.ToList());
        return true;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}