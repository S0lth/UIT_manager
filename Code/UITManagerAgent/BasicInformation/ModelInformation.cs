using System.Text.Json;
namespace UITManagerAgent.BasicInformation;


public class ModelInformation : Information{
    private string? _model;
    private string _format = "Text";

    /// <summary>
    /// accessors of the model field
    /// </summary>
    public string? Model {
        get => _model; 
        set => _model = value; 
    }

    /// <summary>
    /// Returns a Json string representation of the machine model
    /// </summary>
    /// <returns>A Json string that represents the machine model.</returns>
    public override string ToJson() {
        return JsonSerializer.Serialize(this);
    }
}
