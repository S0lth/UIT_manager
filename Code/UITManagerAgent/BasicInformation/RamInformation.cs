using System.Management;
using System.Runtime.Versioning;
using System.Text.Json;

namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Provides information about RAM usage, including total and used memory.
/// </summary>
[SupportedOSPlatform("windows")]
public class RamInformation : Information {
    private ulong _totalRam;
    private string _formatTotalValue = "GB";
    private ulong _usedRam;
    private string _formatUsedValue = "GB";
    private ulong _freeRam;
    private string _formatFreeValue = "GB";

    /// <summary>
    /// Returns a string representation of the RAM information.
    /// </summary>
    /// <returns>
    /// A formatted string showing total memory and used memory in GB.
    /// </returns>
    public override string ToString() {
        return
            $"Total memory : {_totalRam / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
            $"Used memory : {_usedRam / (float)(1024 * 1024):F2} GB" + Environment.NewLine +
            $"Free memory : {_freeRam / (float)(1024 * 1024):F2} GB";
    }

    /// <summary>
    /// accessors of the total memory field
    /// </summary>
    public ulong TotalRam {
        get => _totalRam;
        set => _totalRam = value;
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
    public ulong UsedRam {
        get => _usedRam;
        set => _usedRam = value;
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
    public ulong FreeRam {
        get => _freeRam;
        set => _freeRam = value;
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
            $"{{\"TotalRam\":\"{_totalRam / (float)(1024 * 1024):F2}\",\"Format total ram\":\"{_formatTotalValue}\", \"UsedRam\":\"{_usedRam / (float)(1024 * 1024):F2}\",\"Format used ram\":\"{_formatUsedValue}\",\"FreeRam\":\"{_freeRam / (float)(1024 * 1024):F2}\",\"Format free ram\":\"{_formatFreeValue}\"}}";
    }
}