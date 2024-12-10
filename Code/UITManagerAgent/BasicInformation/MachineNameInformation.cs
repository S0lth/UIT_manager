namespace UITManagerAgent.BasicInformation;
using System.Text.Json;
/// <summary>
/// Represent information about the machine name  
/// </summary>
public class MachineNameInformation : Information {
    private string _machineName = String.Empty;
    private string _format = "TEXT";

    /// <summary>
    /// accessors of the machineName field 
    /// </summary>
    public string MachineName {
        get => _machineName;
        set => _machineName = value;
    }

    /// <summary>
    /// Gets or sets the format of the machine name information
    /// </summary>
    /// <value>
    /// A string representing the format of machine name.
    /// </value>
    public string Format {
        get => _format;
        set => _format = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Returns a Json string representation of the machine name.
    /// </summary>
    /// <returns>A Json string that represents the machine name.</returns>
    public override string ToJson() {
        return JsonSerializer.Serialize(this);
    }

    /// <summary>
    /// String representation of the machine name
    /// </summary>
    /// <returns>A string representation of the machine name</returns>
    public override string ToString() => MachineName;
}