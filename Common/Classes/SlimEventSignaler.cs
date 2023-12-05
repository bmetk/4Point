using System.Collections.Immutable;

namespace Common.Classes;

public class SlimEventSignaler
{
    private ImmutableList<Func<Task>> _tasks = ImmutableList<Func<Task>>.Empty;

    public void AddTask(Func<Task> task)
    {
        _tasks = _tasks.Add(task);
    }

    public async Task CallAsync()
    {
        foreach (var task in _tasks)
        {
            await task();
        }
    }
}