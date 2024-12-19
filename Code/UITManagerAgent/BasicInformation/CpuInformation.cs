namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represents detailed CPU information.
/// </summary>
public class CpuInformation : Information {
    /// <summary>
    /// CPU main fields.
    /// </summary>
    public InnerValue CPU { get; set; } = new("CPU", "null");

    /// <summary>
    /// Detailed CPU information agents.
    /// </summary>
    public List<InnerValue> InformationAgents { get; set; } = new() {
        new InnerValue("Logical core", "TEXT"),
        new InnerValue("Core count", "TEXT"),
        new InnerValue("Clockspeed", "TEXT"),
        new InnerValue("Model", "TEXT")
    };

    /// <summary>
    /// Returns a Json string representation of the CpuInformation.
    /// </summary>
    /// <returns>A Json string that represents the CpuInformation.</returns>
    public override string ToJson() {
        string agentsJson = string.Join(",", InformationAgents.Select(agent => $@"{{""Name"":""{agent.Name}"",""Value"":""{agent.Value}"",""Format"":""{agent.Format}""}}"));

        return $@"{{""Name"": ""{CPU.Name}"",""Value"": ""{CPU.Value}"",""Format"": ""{CPU.Format}"",""InformationAgents"": [{agentsJson}]}}";
    }
}