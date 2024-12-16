namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represents information about the system's uptime, in milliseconds, with a formatted string representation.
/// </summary>
public class UpTimeInformation : Information {
    
    /// <summary>
    /// accessors of the UpTime field
    /// </summary>
    public InnerValue UpTime { get; set; } = new("UpTime","TEXT");
    

    /// <summary>
    /// Returns a Json string representation of the uptime.
    /// </summary>
    /// <returns>A Json string that represents the uptime.</returns>
    public override string ToJson() {
        return $"{{\"Name\":\"{UpTime.Name}\",\"Value\":\"{UpTime.Value}\",\"Format\":\"{UpTime.Format}\"}}";
    }
}