namespace WebService.Service.StatisticsServices;

public class AbstractObservableService<TOutput>
{
    private readonly HashSet<Common.Interfaces.IObserver<TOutput>> _observers = new();
    
    protected virtual async Task NotifyObservers(TOutput data)
    {
        foreach (var observer in _observers)
        {
            await observer.ConsumeDataAsync(data);
        }
    }

    public virtual void Subscribe(Common.Interfaces.IObserver<TOutput> observer)
    {
        _observers.Add(observer);
    }

    public virtual void Unsubscribe(Common.Interfaces.IObserver<TOutput> observer)
    {
        _observers.Remove(observer);
    }
}