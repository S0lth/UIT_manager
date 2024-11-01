using UITManagerAgent.DataCollectors;
using System.Management;

namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Provides information about RAM usage, including total and used memory.
/// </summary>
public class RamInformation : Information
{
    private ulong _totalMemory;
    private ulong _usedMemory;
    private ulong _freeMemory;
    private ManagementObjectSearcher _wmiSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

    /// <summary>
    /// Returns a string representation of the RAM information.
    /// </summary>
    /// <returns>
    /// A formatted string showing total memory and used memory in GB.
    /// </returns>
    public override string ToString()
    {
        return
            $"Total: {_totalMemory / (float)(1024 * 1024):F2} GB; Used: {_usedMemory / (float)(1024 * 1024):F2} GB";
    }

    public ulong GetTotalMemory()
    {
        return _totalMemory;
    }

    public ulong GetFreeMemory()
    {
        return _freeMemory;
    }

    public ulong GetUsedMemory()
    {
        return _usedMemory;
    }

    public ManagementObjectSearcher GetWmiSearcher()
    {
        return _wmiSearcher;
    }

    public void SetTotalMemory(ulong value)
    {
        _totalMemory = value;
    }

    public void SetUsedMemory(ulong value)
    {
        _usedMemory = value;
    }

    public void SetFreeMemory(ulong value)
    {
        _freeMemory = value;
    }
}