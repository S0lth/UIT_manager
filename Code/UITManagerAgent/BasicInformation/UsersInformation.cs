using System.Text.Json;
using UITManagerAgent.BasicInformation;

/// <summary>
///     Represents a collection of user information.
///     Inherits from the <see cref="Information" /> class.
/// </summary>
public class UsersInformation : Information {
    /// <summary>
    ///     List of collected usernames.
    /// </summary>
    private List<string> _usersList = new();

    public List<string> UsersList {
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
        return $"{string.Join(", ", UsersList)}";
    }

    /// <summary>
    /// Returns a Json string representation of the users
    /// </summary>
    /// <returns>A Json string that represents the users.</returns>
    public override string ToJson() {
        return JsonSerializer.Serialize(this);
    }
}