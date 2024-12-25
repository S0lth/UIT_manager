using UITManagerAgent.BasicInformation;

/// <summary>
/// Defines a contract for classes that collect data and return it as an <see cref="Information"/> object.
/// </summary>
public interface DataCollector {
    
    /// <summary>
    /// Collects data and returns it as an <see cref="Information"/> object.
    /// </summary>
    /// <returns>
    /// An <see cref="Information"/> object representing the collected data.
    /// </returns>
    public Information Collect();
}