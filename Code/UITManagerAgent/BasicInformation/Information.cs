namespace UITManagerAgent.BasicInformation;

/// <summary>
/// Represents the basic information 
/// </summary>
public abstract class Information {
    
    /// <summary>
    /// Converts the information object into its JSON representation.
    /// </summary>
    /// <returns>A <see cref="string"/> containing the JSON representation of the object.</returns>
    public abstract String ToJson();
}