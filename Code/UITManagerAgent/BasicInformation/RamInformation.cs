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
    private ulong _usedMemory;
    private ulong _freeMemory;
    private string _formatValue = "GB";

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
    /// accessors of the used memory field
    /// </summary>
    public ulong UsedMemory {
        get => _usedMemory;
        set => _usedMemory = value;
    }

    /// <summary>
    /// accessors of the free memory field
    /// </summary>
    public ulong FreeMemory {
        get => _freeMemory;
        set => _freeMemory = value;
    }


    /// <summary>
    /// Returns a Json string representation of the ramInformation
    /// </summary>
    /// <returns>A Json string that represents the ramInformation .</returns>
    public override string ToJson() {
        return $"{{\"TotalMemory\":\"{_totalMemory / (float)(1024 * 1024):F2}\",\"UsedMemory\":\"{_usedMemory / (float)(1024 * 1024):F2}\",\"FreeMemory\":\"{_freeMemory / (float)(1024 * 1024):F2}\"}}";
    }
}