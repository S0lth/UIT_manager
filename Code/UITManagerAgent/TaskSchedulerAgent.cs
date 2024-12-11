namespace UITManagerAgent;
    
public class TaskSchedulerAgent : TaskScheduler, IDisposable {
    private readonly TimeSpan _interval;
    private readonly Timer _timer;
    private readonly Func<Task> _taskFunc;

    public TaskSchedulerAgent(int minutes, Func<Task> taskFunc) 
        // on doit rechanger cette variable en heure pour la production !!! Mettre 24 et changer "minutes" en "hours"
    {
        if (minutes <= 0) throw new ArgumentOutOfRangeException(nameof(minutes), "=> Minutes must be greater than zero.");
        if (taskFunc == null) throw new ArgumentNullException(nameof(taskFunc));

        _interval = TimeSpan.FromMinutes(minutes);
        _taskFunc = taskFunc;
        _timer = new Timer(ExecuteScheduledTask, null, TimeSpan.Zero, _interval);
    }

    private async void ExecuteScheduledTask(object? state)
    {
        try
        {
            await _taskFunc();
            Console.WriteLine($"=> Task executed at {DateTime.Now}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=> Task execution failed: {ex.Message}");
        }
    }

    protected override IEnumerable<Task> GetScheduledTasks()
    {
        return Array.Empty<Task>();
    }

    protected override void QueueTask(Task task)
    {
        throw new NotSupportedException("=> This TaskScheduler queues tasks automatically based on the interval.");
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        return TryExecuteTask(task);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
