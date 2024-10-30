
using UITManagerAgent.DataCollectors;

/// <summary>
/// Represents a collection of user information.
/// Inherits from the <see cref="Information"/> class.
/// </summary>
public class UsersInformation : Information
{
    /// <summary>
    /// List of collected usernames.
    /// </summary>
    private List<string> _usersList = new();

    /// <summary>
    /// Retrieves the list of collected usernames.
    /// </summary>
    /// <returns>
    /// A list of strings containing the usernames.
    /// </returns>
    public List<string> GetUsersList() => _usersList;

    /// <summary>
    /// Returns a string representation of the list of usernames.
    /// </summary>
    /// <returns>
    /// A comma-separated string of usernames.
    /// </returns>
    public override string ToString() => $"{string.Join(", ", GetUsersList())}";
}
