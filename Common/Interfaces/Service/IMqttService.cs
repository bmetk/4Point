using Common.Delegates;

namespace Common.Interfaces.Service;

public interface IMqttService
{
    public Task PublishAsync(string topic, byte[] message);
    public Task SubscribeAsync(string topic, MqttMessageHandlerDelegate callback);
    public Task UnsubscribeAsync(string topic, MqttMessageHandlerDelegate callback);
}