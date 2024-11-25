using System.Text.Json;
namespace UITManagerAgent.BasicInformation;

public class TagInformation : Information{
    private string? _tagService;

    
    /// <summary>
    /// accessors of the tagService field
    /// </summary>
    public string? TagService {
        get => _tagService;
        set => _tagService = value;
    }
    
    /// <summary>
    /// Returns a Json string representation of the service tag.
    /// </summary>
    /// <returns>A Json string that represents the service tag.</returns>
    public override string ToJson() {
        return JsonSerializer.Serialize(this);
    }
}
