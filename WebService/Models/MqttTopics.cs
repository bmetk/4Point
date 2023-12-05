namespace WebService.Models;

public static class MqttTopics
{
    private const string MQTT_TOPICS = "MqttTopics";
    
    private const string RELAY = "Relay";
    private const string DI = "Di";

    private const string RFID = "RfidIn";
    private const string MEASUREMENTS = "Measurements";
    
    public static string AuthTopic(IConfiguration configuration) => 
        configuration.GetSection(MQTT_TOPICS).GetSection(RELAY).GetSection("Auth").Get<string>();

    public static string ErrorCheckedTopic(IConfiguration configuration) =>
        configuration.GetSection(MQTT_TOPICS).GetSection(RELAY).GetSection("ErrorChecked").Get<string>();

    public static string WorkInProgressTopic(IConfiguration configuration) =>
        configuration.GetSection(MQTT_TOPICS).GetSection(DI).GetSection("WorkInProgress").Get<string>();

    public static string ErrorTopic(IConfiguration configuration) =>
        configuration.GetSection(MQTT_TOPICS).GetSection(DI).GetSection("Error").Get<string>();

    public static string DoneTopic(IConfiguration configuration) =>
        configuration.GetSection(MQTT_TOPICS).GetSection(DI).GetSection("Done").Get<string>();

    public static string RFIDAuthTopic(IConfiguration configuration) =>
        configuration.GetSection(MQTT_TOPICS).GetSection(RFID).Get<string>();

    public static string MeasurementsTopic(IConfiguration configuration) =>
        configuration.GetSection(MQTT_TOPICS).GetSection(MEASUREMENTS).Get<string>();
}