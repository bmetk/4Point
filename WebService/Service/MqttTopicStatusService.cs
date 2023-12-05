namespace WebService.Service;

public class MqttTopicStatusService<T>
{
    private readonly Dictionary<string, T> _status = new();

    public bool AddTopic(string topic)
    {
        return _status.TryAdd(topic, default!);
    }

    public T ChangeValue(string topic, Func<T, T> func)
    {
        var current = _status[topic];
        _status[topic] = func(current);
        return _status[topic];
    }
}