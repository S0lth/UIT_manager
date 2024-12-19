namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represents information about a service tag.
/// </summary>
public class TagInformation : Information {
    
    /// <summary>
    /// The service tag field as an InnerValue object
    /// </summary>
    public InnerValue TagService { get; set; } = new("Tag Service", "TEXT");
    
    /// <summary>
    /// Returns a Json string representation of the service tag.
    /// </summary>
    /// <returns>A Json string that represents the service tag.</returns>
    public override string ToJson() {
        return $"{{\"Name\":\"{TagService.Name}\",\"Value\":\"{TagService.Value}\",\"Format\":\"{TagService.Format}\"}}";
    }
}