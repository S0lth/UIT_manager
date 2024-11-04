using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.BasicInformation;
/// <summary>
/// Represent information about the machine name  
/// </summary>
public class MachineNameInformation : Information{
    private string _machineName = String.Empty;
    
    /// <summary>
    /// accessors of the machineName field 
    /// </summary>
    public string MachineName {
        get => _machineName;
        set => _machineName = value;
    }

    /// <summary>
    /// String representation of the machine name
    /// </summary>
    /// <returns>A string representation of the machine name</returns>
    public override string ToString() => MachineName;
}
