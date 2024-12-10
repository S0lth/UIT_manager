using System.Management;
using System.Runtime.Versioning;
using System.Text.Json;

namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represents OS information.
/// </summary>
[SupportedOSPlatform("windows")]
public class OsInformation : Information {
    private ManagementObjectSearcher _wmiSearcher = new("SELECT * FROM Win32_OperatingSystem");
    private ManagementObject? _queryObj;
    private string? _osName;
    private string _formatOsName = "TEXT";
    private string? _osVersion;
    private string _formatOsVersion = "TEXT";
    private string? _osBuild;
    private string _formatOsBuild = "TEXT";

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
    /// Gets or sets the format of the os name information
    /// </summary>
    /// <value>
    /// A string representing the format of os name.
    /// </value>
    public string FormatOsName {
        get => _formatOsName;
        set => _formatOsName = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// accessors of the osBuild field
    /// </summary>
    public string? OsBuild {
        get => _osBuild;
        set => _osBuild = value;
    }

    /// <summary>
    /// Gets or sets the format of the os build information
    /// </summary>
    /// <value>
    /// A string representing the format of os build.
    /// </value>
    public string FormatOsBuild {
        get => _formatOsBuild;
        set => _formatOsBuild = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// accessors of the osVersion field
    /// </summary>
    public string? OsVersion {
        get => _osVersion;
        set => _osVersion = value;
    }

    /// <summary>
    /// Gets or sets the format of the os version information
    /// </summary>
    /// <value>
    /// A string representing the format of os version.
    /// </value>
    public string FormatOsVersion {
        get => _formatOsVersion;
        set => _formatOsVersion = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Returns a Json string representation of os information.
    /// </summary>
    /// <returns>A Json string that represents the os informations.</returns>
    public override string ToJson() {
        return
            $"{{\"OsName\":\"{_osName}\",\"Format os name\":\"{_formatOsName}\",\"OsVersion\":\"{_osVersion}\",\"Format os version\":\"{_formatOsVersion}\",\"OsBuild\":\"{_osBuild}\",\"Format build\":\"{_formatOsBuild}\"}}";
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