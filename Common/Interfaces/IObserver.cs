namespace Common.Interfaces;

public interface IObserver<in T>
{
    public Task ConsumeDataAsync(T data);
}