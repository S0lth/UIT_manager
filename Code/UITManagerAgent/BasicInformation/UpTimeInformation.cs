using System;
using System.Runtime.Versioning;
using System.Text.Json;

namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represents information about the system's uptime, in milliseconds, with a formatted string representation.
/// </summary>
[SupportedOSPlatform("windows")]
public class UpTimeInformation : Information {
    private int _milliseconds;
    private string? _format = "TEXT";

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

    /// <summary>
    /// Gets or sets the format of the uptime information
    /// </summary>
    /// <value>
    /// A string representing the format of uptime.
    /// </value>
    public string? Format {
        get => _format;
        set => _format = value;
    }

    /// <summary>
    /// Returns a Json string representation of the up time.
    /// </summary>
    /// <returns>A Json string that represents the up time.</returns>
    public override string ToJson() {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(_milliseconds);
        return $"{{\"Days\":{(int)timeSpan.TotalDays},\"Hours\":{timeSpan.Hours:D2},\"Minutes\":{timeSpan.Minutes:D2},\"Seconds\":{timeSpan.Seconds:D2},\"Format\":\"{_format}\"}}";
    }
}