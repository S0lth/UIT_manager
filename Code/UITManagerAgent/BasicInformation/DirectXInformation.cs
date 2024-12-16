namespace UITManagerAgent.BasicInformation;
/// <summary>
/// Represents information about the DirectX version installed on the system.
/// </summary>
public class DirectXInformation : Information {
    
    /// <summary>
    /// accessors of the DirectX field
    /// </summary>
    public InnerValue DirectX { get; set; } = new("DirectX","TEXT");
    
    /// <summary>
    /// Returns a Json string representation of the directX version.
    /// </summary>
    /// <returns>A Json string that represents the directX version.</returns>
    public override string ToJson() {
        return $"{{\"Name\":\"{DirectX.Name}\",\"Value\":\"{DirectX.Value}\",\"Format\":\"{DirectX.Format}\"}}";
    }
}