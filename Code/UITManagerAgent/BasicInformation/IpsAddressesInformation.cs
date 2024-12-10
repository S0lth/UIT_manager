using System.Text.Json;
namespace UITManagerAgent.BasicInformation;
/// <summary>
/// Represents a collection of IP addresses.
/// </summary>
public class IpsAddressesInformation : Information {
    private List<string> _ipsList = new();
    private string? _formatIp = "TEXT";
    
    /// <summary>
    /// accessors of IpsAddresses List
    /// </summary>
    public List<string> IpsList {
        get => _ipsList;
        set => _ipsList = value;
    }

    /// <summary>
    /// Gets or sets the format of the ip information
    /// </summary>
    /// <value>
    /// A string representing the format of ip.
    /// </value>
    public string? FormatIp {
        get => _formatIp;
        set => _formatIp = value;
    }

    /// <summary>
    ///     Returns a string representation of all IP addresses in the list.
    /// </summary>
    /// <returns>A string containing all IP addresses separated by comas.</returns>
    public override string ToString() {
        return $"{string.Join(", ", _ipsList)}";
    }
    
    /// <summary>
    /// Get a string JSON format of IPS
    /// </summary>
    /// <returns>A Json string that represents IPS List</returns>
    public override string ToJson() {
        return JsonSerializer.Serialize(this);
    }
}