using Common.Classes;

namespace Common.Delegates;

public delegate void MqttMessageHandlerDelegate(string topic, MqttMessage payload);