using UITManagerAgent.BasicInformation;
using System.Text.Json;

/// <summary>
/// Represents information about a domain name.
/// </summary>
public class DomainNameInformation : Information {
    private string? _domainName;

    /// <summary>
    /// accessors of the domainName field
    /// </summary>
    public string? DomainName {
        get => _domainName;
        set => _domainName = value;
    }

    /// <summary>
    /// Returns a string representation of the domain name.
    /// </summary>
    /// <returns>A string that represents the domain name, prefixed with "Domain name : ".</returns>
    public override string ToString() {
        return "Domain name : " + _domainName;
    }

    /// <summary>
    /// Returns a Json string representation of the domain name.
    /// </summary>
    /// <returns>A Json string that represents the domain name.</returns>
    public string ToJson() {
        return JsonSerializer.Serialize(this);
    }

}