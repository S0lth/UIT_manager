
using UITManagerAgent.DataCollectors;

/// <summary>
///     Represents a collection of user information.
///     Inherits from the <see cref="Information" /> class.
/// </summary>
public class UsersInformation : Information {
    /// <summary>
    ///     List of collected usernames.
    /// </summary>
    private List<string> _usersList = new();

    public List<string> usersList {
        get => _usersList;
        set => _usersList = value;
    }

    /// <summary>
    ///     Returns a string representation of the list of usernames.
    /// </summary>
    /// <returns>
    ///     A comma-separated string of usernames.
    /// </returns>
    public override string ToString() {
        return $"{string.Join(", ", usersList)}";
    }
}