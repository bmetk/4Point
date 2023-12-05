namespace Common.Classes;

public readonly record struct TopicData(string Topic, MqttMessage MqttMessage);