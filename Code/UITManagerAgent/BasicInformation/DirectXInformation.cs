using System.Text.Json;

namespace UITManagerAgent.BasicInformation;
/// <summary>
/// Represents information about the DirectX version installed on the system.
/// </summary>
public class DirectXInformation : Information {
    /// <summary>
    /// Holds the DirectX version as a string.
    /// </summary>
    private string? _directXVersion;
    private string? _formatdirectX = "TEXT";

    public DirectXInformation() {
        _directXVersion = "";
    }

    /// <summary>
    /// accessors of the domainName field
    /// </summary>
    public string? DirectXVersion {
        get => _directXVersion;
        set => _directXVersion = value;
    }
    
    /// <summary>
    /// Gets or sets the format of the directX information
    /// </summary>
    /// <value>
    /// A string representing the format of directX.
    /// </value>
    public string? FormatdirectX {
        get => _formatdirectX;
        set => _formatdirectX = value;
    }

    /// <summary>
    /// Returns a string representation of the DirectX information.
    /// </summary>
    /// <returns>A string in the format "DirectX Version: {version}".</returns>
    public override string ToString() {
        return "DirectX Version: " + _directXVersion;
    }

    /// <summary>
    /// Returns a Json string representation of the directX version.
    /// </summary>
    /// <returns>A Json string that represents the directX version.</returns>
    public override string ToJson() {
        return JsonSerializer.Serialize(this);
    }
}