using System.Runtime.Versioning;
namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Provides information about RAM usage, including total and used memory.
/// </summary>
[SupportedOSPlatform("windows")]
public class RamInformation : Information {
   
    /// <summary>
    /// Ram main fields.
    /// </summary>
    public InnerValue Ram { get; set; } = new("Ram", "null");

    /// <summary>
    /// Detailed Ram information agents.
    /// </summary>
    public List<InnerValue> InformationAgents { get; set; } = new() {
        new InnerValue("Total RAM", "GB"),
        new InnerValue("Used RAM", "GB"),
        new InnerValue("Free RAM", "GB"),
        new InnerValue("Used RAM", "%")
    };

    /// <summary>
    /// Returns a Json string representation of the RamInformation.
    /// </summary>
    /// <returns>A Json string that represents the RamInformation.</returns>
    public override string ToJson() {
        string agentsJson = string.Join(",", InformationAgents.Select(agent => $@"{{""Name"":""{agent.Name}"",""Value"":""{agent.Value}"",""Format"":""{agent.Format}""}}"));

        return $@"{{""Name"": ""{Ram.Name}"",""Value"": ""{Ram.Value}"",""Format"": ""{Ram.Format}"",""InformationAgents"": [{agentsJson}]}}";
    }
}