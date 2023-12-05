namespace WebService.Models;

public class RfidStatusObserver : Common.Interfaces.IObserver<string>
{
    private readonly ILogger _logger;
    private readonly Func<string, Task> _strategy;

    public RfidStatusObserver(ILogger logger, Func<string, Task> strategy)
    {
        _logger = logger;
        _strategy = strategy;
    }


    public Task ConsumeDataAsync(string data)
    {
        _logger.LogInformation("Received data {DATA}", data);
        return _strategy(data);
    }
}