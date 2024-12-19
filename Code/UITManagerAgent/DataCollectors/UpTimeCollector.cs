using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

namespace UITManagerAgent.DataCollectors;

/// <summary>
/// Collects information about the system's uptime and provides it as an <see cref="UpTimeInformation"/> instance.
/// </summary>
public class UpTimeCollector : DataCollector {
    /// <summary>
    /// Collects the system's uptime information by reading the system tick count.
    /// </summary>
    /// <remarks>
    /// This method is supported only on Windows platforms.
    /// </remarks>
    /// <returns>An <see cref="UpTimeInformation"/> object populated with the current system uptime.</returns>
    [SupportedOSPlatform("windows")]
    public Information Collect() {
        UpTimeInformation upTimeInformation = new();

        upTimeInformation.UpTime.Value = TimeSpan.FromMilliseconds(Environment.TickCount).ToString();

        return upTimeInformation;
    }
}