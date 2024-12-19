using System.Management;
using System.Runtime.Versioning;
using System.Text.Json;

namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represents OS information.
/// </summary>
[SupportedOSPlatform("windows")]
public class OsInformation : Information {
    /// <summary>
    /// OS main fields.
    /// </summary>
    public InnerValue OS { get; set; } = new("OS", "null");

    /// <summary>
    /// Detailed OS information agents.
    /// </summary>
    public List<InnerValue> InformationAgents { get; set; } = new() {
        new InnerValue("OS Name", "TEXT"),
        new InnerValue("OS Build", "TEXT"),
        new InnerValue("OS Version", "TEXT"),
    };

    /// <summary>
    /// Returns a Json string representation of the OSInformation.
    /// </summary>
    /// <returns>A Json string that represents the OSInformation.</returns>
    public override string ToJson() {
        string agentsJson = string.Join(",", InformationAgents.Select(agent => $@"{{""Name"":""{agent.Name}"",""Value"":""{agent.Value}"",""Format"":""{agent.Format}""}}"));

        return $@"{{""Name"": ""{OS.Name}"",""Value"": ""{OS.Value}"",""Format"": ""{OS.Format}"",""InformationAgents"": [{agentsJson}]}}";
    }
}