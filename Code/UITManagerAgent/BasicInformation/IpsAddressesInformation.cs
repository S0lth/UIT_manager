namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represents a collection of IP addresses.
/// </summary>
public class IpsAddressesInformation : Information {
    /// <summary>
    /// Ip main fields.
    /// </summary>
    public InnerValue Ip { get; set; } = new("IP", "null");
    
    /// <summary>
    /// Accessor for the list of IP address information agents.
    /// </summary>
    public List<InnerValue> InformationAgents { get; set; } = new();

    /// <summary>
    /// Returns a Json string representation of the IP addresses in the information agents list.
    /// </summary>
    /// <returns>A Json string that represents the IpsAddresses information.</returns>
    public override string ToJson() {
        string agentsJson = string.Join(",", InformationAgents.Select(agent => $@"{{""Name"":""{agent.Name}"",""Value"":""{agent.Value}"",""Format"":""{agent.Format}""}}"));

        return $@"{{""Name"": ""{Ip.Name}"",""Value"": ""{Ip.Value}"",""Format"": ""{Ip.Format}"",""InformationAgents"": [{agentsJson}]}}";
    }
}