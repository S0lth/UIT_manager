using System.Text.Json;
namespace UITManagerAgent.BasicInformation;


public class ModelInformation : Information{
    /// <summary>
    /// accessors of the Model field
    /// </summary>
    public InnerValue Model { get; set; } = new("Model","TEXT");
    
    /// <summary>
    /// Returns a Json string representation of the Model.
    /// </summary>
    /// <returns>A Json string that represents the Model.</returns>
    public override string ToJson() {
        return $"{{\"Name\":\"{Model.Name}\",\"Value\":\"{Model.Value}\",\"Format\":\"{Model.Format}\"}}";
    }
}
