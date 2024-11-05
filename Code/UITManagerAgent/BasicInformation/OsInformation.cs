using System.Management;
using System.Runtime.Versioning;
namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represents OS information.
/// </summary>
[SupportedOSPlatform("windows")]
public class OsInformation : Information {
    private ManagementObjectSearcher _wmiSearcher = new("SELECT * FROM Win32_OperatingSystem");
    private ManagementObject? _queryObj;
    private string? _osName;
    private string? _osVersion;

    /// <summary>
    /// accessors of the wmiSearcher field
    /// </summary>
    public ManagementObjectSearcher WmiSearcher {
        get => _wmiSearcher;
        set => _wmiSearcher = value;
    }

    /// <summary>
    /// accessors of the queryObj field
    /// </summary>
    public ManagementObject? QueryObj {
        get => _queryObj;
        set => _queryObj = value;
    }

    /// <summary>
    /// accessors of the osName field
    /// </summary>
    public string? OsName {
        get => _osName;
        set => _osName = value;
    }

    /// <summary>
    /// accessors of the osVersion field
    /// </summary>
    public string? OsVersion {
        get => _osVersion;
        set => _osVersion = value;
    }

    /// <summary>
    /// Returns a string representation of the operating system name and version.
    /// </summary>
    /// <returns>A string in the format
    /// "OS name: {Name}
    /// OS version: {Version}".
    /// </returns>
    public override string ToString() {
        return "OS name : " + _osName + Environment.NewLine + "OS version : " + _osVersion;
    }
}