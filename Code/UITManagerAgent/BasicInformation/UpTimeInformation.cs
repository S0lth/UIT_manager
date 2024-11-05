using System.Runtime.Versioning;

namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represents information about the system's uptime, in milliseconds, with a formatted string representation.
/// </summary>
[SupportedOSPlatform("windows")]
public class UpTimeInformation : Information {
    private int _milliseconds;

    /// <summary>
    /// Provides a string representation of the uptime in the format "X days HH:MM:SS".
    /// </summary>
    /// <returns>A formatted string showing the uptime in days, hours, minutes, and seconds.</returns>
    public override string ToString() {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(_milliseconds);

        return $"Uptime : {(int)timeSpan.TotalDays} days {timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    /// <summary>
    /// accessors for the uptime in milliseconds field
    /// </summary>
    public int Milliseconds {
        get => _milliseconds;
        set => _milliseconds = value;
    }
}