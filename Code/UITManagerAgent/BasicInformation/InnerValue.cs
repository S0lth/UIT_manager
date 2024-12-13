using System.Text.Json;

namespace UITManagerAgent.BasicInformation;


public class InnerValue {
    private string? _name; 
    private string? _value; 
    private string? _format;

    /// <summary>
    /// Gets or sets the value associated with this instance.
    /// </summary>
    /// <value>
    /// A string representing the value. Can be null.
    /// </value>
    public string? Value {
        get => _value;
        set => _value = value;
    }

    /// <summary>
    /// Gets or sets the format associated with this instance.
    /// </summary>
    /// <value>
    /// A string representing the format. Can be null.
    /// </value>
    public string? Format {
        get => _format;
        set => _format = value;
    }

    /// <summary>
    /// Gets or sets the name associated with this instance.
    /// </summary>
    /// <value>
    /// A string representing the name. Can be null.
    /// </value>
    public string? Name {
        get => _name;
        set => _name = value;
    }

    /// <summary>
    /// Converts the instance to its string representation.
    /// </summary>
    /// <returns>
    /// A string containing the name, value, and format of the instance.
    /// </returns>
    private string toString() {
        return "name : " + _name + "value : " + _value + "format : " + _format;
    }

    /// <summary>
    /// Returns a Json string representation of the directX version.
    /// </summary>
    /// <returns>A Json string that represents the directX version.</returns>
    public string ToJson() {
        return JsonSerializer.Serialize(this);
    }
    
}
