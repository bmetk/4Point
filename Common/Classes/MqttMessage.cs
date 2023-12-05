namespace Common.Classes;

public readonly record struct MqttMessage(ArraySegment<byte> Payload, long Timestamp = default);

public static class MqttMessageExtensions
{
    public static MqttMessage WithCurrentTimestamp(this MqttMessage message) => 
        message with { Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() };
}