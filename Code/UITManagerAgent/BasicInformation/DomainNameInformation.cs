using System.Text.Json;
using UITManagerAgent.BasicInformation;

/// <summary>
/// Represents information about a domain name.
/// </summary>
public class DomainNameInformation : Information {
    
    /// <summary>
    /// accessors of the Domain Name field
    /// </summary>
    public InnerValue DomaineName { get; set; } = new("Domaine name","TEXT");
    
    /// <summary>
    /// Returns a Json string representation of the Domain Name.
    /// </summary>
    /// <returns>A Json string that represents the Domain Name.</returns>
    public override string ToJson() {
        return $"{{\"Name\":\"{DomaineName.Name}\",\"Value\":\"{DomaineName.Value}\",\"Format\":\"{DomaineName.Format}\"}}";
    }
}