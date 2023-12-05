namespace Common.Interfaces.Service;

public interface IDataAnalizer<TInput, TOutput>
{
    public event Action<TInput, TOutput> DataProcessed;
    public void FeedData(TInput data);
    public void ClearCache();
}