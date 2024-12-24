namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represent information about the machine name  
/// </summary>
public class MachineNameInformation : Information {
    
    /// <summary>
    /// accessors of the Machine Name field
    /// </summary>
    public InnerValue MachineName { get; set; } = new("Machine Name","TEXT");
    
    /// <summary>
    /// Returns a Json string representation of the Machine Name.
    /// </summary>
    /// <returns>A Json string that represents the Machine Name.</returns>
    public override string ToJson() {
        return $"{{\"Name\":\"{MachineName.Name}\",\"Value\":\"{MachineName.Value}\",\"Format\":\"{MachineName.Format}\"}}";
    }
}