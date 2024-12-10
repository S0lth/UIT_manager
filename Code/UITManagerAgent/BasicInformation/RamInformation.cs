using System.Management;
using System.Runtime.Versioning;
using System.Text.Json;

namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Provides information about RAM usage, including total and used memory.
/// </summary>
[SupportedOSPlatform("windows")]
public class RamInformation : Information {
    private ulong _totalMemory;
    private string _formatTotalValue = "GB";
    private ulong _usedMemory;
    private string _formatUsedValue = "GB";
    private ulong _freeMemory;
    private string _formatFreeValue = "GB";

    /// <summary>
    /// Returns a string representation of the RAM information.
    /// </summary>
    /// <returns>
    /// A formatted string showing total memory and used memory in GB.
    /// </returns>
    public override string ToString() {
        return
            $"Total memory : {_totalMemory / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
            $"Used memory : {_usedMemory / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
            $"Free memory : {_freeMemory / (float)(1024 * 1024):F2} GB";
    }

    /// <summary>
    /// accessors of the total memory field
    /// </summary>
    public ulong TotalMemory {
        get => _totalMemory;
        set => _totalMemory = value;
    }

    /// <summary>
    /// Gets or sets the format of the total ram information
    /// </summary>
    /// <value>
    /// A string representing the format of total ram.
    /// </value>
    public string FormatTotalValue {
        get => _formatTotalValue;
        set => _formatTotalValue = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// accessors of the used memory field
    /// </summary>
    public ulong UsedMemory {
        get => _usedMemory;
        set => _usedMemory = value;
    }

    /// <summary>
    /// Gets or sets the format of the used ram information
    /// </summary>
    /// <value>
    /// A string representing the format of used ram.
    /// </value>
    public string FormatUsedValue {
        get => _formatUsedValue;
        set => _formatUsedValue = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// accessors of the free memory field
    /// </summary>
    public ulong FreeMemory {
        get => _freeMemory;
        set => _freeMemory = value;
    }

    /// <summary>
    /// Gets or sets the format of the free ram information
    /// </summary>
    /// <value>
    /// A string representing the format of free ram.
    /// </value>
    public string FormatFreeValue {
        get => _formatFreeValue;
        set => _formatFreeValue = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Returns a Json string representation of the ramInformation
    /// </summary>
    /// <returns>A Json string that represents the ramInformation .</returns>
    public override string ToJson() {
        return
            $"{{\"TotalMemory\":\"{_totalMemory / (float)(1024 * 1024):F2}\",\"Format total ram\":\"{_formatTotalValue}\", \"UsedMemory\":\"{_usedMemory / (float)(1024 * 1024):F2}\",\"Format used ram\":\"{_formatUsedValue}\",\"FreeMemory\":\"{_freeMemory / (float)(1024 * 1024):F2}\",\"Format free ram\":\"{_formatFreeValue}\"}}";
    }
}