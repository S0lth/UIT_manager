namespace UITManagerAgent;
    
/// <summary>
/// Represents a task scheduler that periodically executes a specified asynchronous task.
/// </summary>
public class TaskSchedulerAgent : TaskScheduler, IDisposable {
    private readonly TimeSpan _interval;
    private readonly Timer _timer;
    private readonly Func<Task> _taskFunc;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskSchedulerAgent"/> class with the specified interval and task to execute.
    /// </summary>
    /// <param name="minutes">
    /// The interval, in minutes, between task executions. 
    /// </param>
    /// <param name="taskFunc">The asynchronous task to execute periodically.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="minutes"/> is less than or equal to zero.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="taskFunc"/> is null.</exception>
    public TaskSchedulerAgent(int hour, Func<Task> taskFunc) 
        // on doit rechanger cette variable en heure pour la production !!! Mettre 24 et changer "minutes" en "hours"
    {
        if (hour <= 0) throw new ArgumentOutOfRangeException(nameof(hour), "=> Hours must be greater than zero.");

        _interval = TimeSpan.FromHours(hour);
        _taskFunc = taskFunc ?? throw new ArgumentNullException(nameof(taskFunc));
        _timer = new Timer(ExecuteScheduledTask, null, TimeSpan.Zero, _interval);
    }

    /// <summary>
    /// Executes the scheduled task. This method is called by the internal timer.
    /// </summary>
    /// <param name="state">An optional state object (not used).</param>
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

    /// <summary>
    /// Releases the resources used by the <see cref="TaskSchedulerAgent"/> instance.
    /// </summary>
    public void Dispose()
    {
        _timer.Dispose();
    }
}
