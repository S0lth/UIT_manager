namespace UITManagerApi.Models;
/// <summary>
/// Represents a machine
/// </summary>
public class MachineAgent {

    public string Name { get; set; }

    public List<InformationAgent> Informations { get; set; }
    
    public string? Model { get; set; }
    
    public MachineAgent() {
    }

}