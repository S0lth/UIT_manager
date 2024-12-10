using System.Text.Json;
namespace UITManagerAgent.BasicInformation;

public class TagInformation : Information{
    private string? _tagService;
    private string? _format = "TEXT";
    
    /// <summary>
    /// accessors of the tagService field
    /// </summary>
    public string? TagService {
        get => _tagService;
        set => _tagService = value;
    }

    /// <summary>
    /// Gets or sets the format of the tag service information
    /// </summary>
    /// <value>
    /// A string representing the format of tag service.
    /// </value>
    public string? Format {
        get => _format;
        set => _format = value;
    }

    /// <summary>
    /// Returns a Json string representation of the service tag.
    /// </summary>
    /// <returns>A Json string that represents the service tag.</returns>
    public override string ToJson() {
        return JsonSerializer.Serialize(this);
    }
}
