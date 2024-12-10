using System.Text.Json;
namespace UITManagerAgent.BasicInformation;


public class ModelInformation : Information{
    private string? _model;
    private string _format = "TEXT";

    /// <summary>
    /// accessors of the model field
    /// </summary>
    public string? Model {
        get => _model; 
        set => _model = value; 
    }

    /// <summary>
    /// Gets or sets the format of the model information
    /// </summary>
    /// <value>
    /// A string representing the format of model.
    /// </value>
    public string Format {
        get => _format;
        set => _format = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Returns a Json string representation of the machine model
    /// </summary>
    /// <returns>A Json string that represents the machine model.</returns>
    public override string ToJson() {
        return JsonSerializer.Serialize(this);
    }
}
