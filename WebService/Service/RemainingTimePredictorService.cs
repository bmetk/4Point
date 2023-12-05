using System.Collections.Immutable;

namespace WebService.Service;

public class RemainingTimePredictorService
{
    private long _goalAmount;
    private readonly List<DateTimeOffset> _times = new();
    private ImmutableList<Func<TimeSpan, Task>> _handlers = ImmutableList<Func<TimeSpan, Task>>.Empty;

    private ILogger? _logger;

    public long GoalAmount { set => _goalAmount = value; }
    
    public RemainingTimePredictorService(long goalAmount)
    {
        _goalAmount = goalAmount;
    }

    public RemainingTimePredictorService WithLogger(ILogger logger)
    {
        _logger = logger;
        return this;
    }

    public void AddEventHandler(Func<TimeSpan, Task> handler)
    {
        _handlers = _handlers.Add(handler);
    }

    public async Task AddNextTimeAsync(DateTimeOffset time)
    {
        _times.Add(time);
        if(_times.Count <= 1) return;
        
        TimeSpan remaining = PredictEndTime();
        foreach (var handler in _handlers)
        {
            await handler(remaining);
        }
    }

    private TimeSpan PredictEndTime()
    {
        double produced = _times.Count;
        var timeSoFar = _times.Last() - _times.First();
        _logger?.LogInformation("Time between start and last is {difference}", timeSoFar);
        double ratioOfGoalAndProduced = _goalAmount / produced;
        var timeForAll = timeSoFar * ratioOfGoalAndProduced;
        _logger?.LogInformation("Time for everything to finish is {time}", timeForAll);
        return timeForAll == TimeSpan.Zero ? TimeSpan.Zero : timeForAll - timeSoFar;
    }
}