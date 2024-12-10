using System.DirectoryServices;
using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;

/// <summary>
///     Collects user information from the system and returns it as a <see cref="UsersInformation" /> instance.
/// </summary>
public class UserCollector : DataCollector {
    /// <summary>
    ///     Collects user information by accessing the system directory entries.
    /// </summary>
    /// <returns>
    ///     A <see cref="UsersInformation" /> object containing the list of usernames.
    /// </returns>
    [SupportedOSPlatform("windows")]
    public Information Collect() {
        UsersInformation users = new();

        try {
            using DirectoryEntry localMachine = new("WinNT://" + Environment.MachineName);
            foreach (DirectoryEntry child in localMachine.Children) {
                if (child.SchemaClassName == "User") {
                    UsersInformation.User user= new ();
                    user.UserName = child.Name;
                    user.UserScope = IsDomainUser() ? "Domaine" : "Local";
                    users.UsersList.Add(user);
                }
            }
        }
        catch (Exception ex) {
            Console.WriteLine("Error while retrieving users: " + ex.Message);
        }

        return users;
    }
    
    /// <summary>
    /// Verrify if the user is a local user or a domain
    /// </summary>
    /// <returns>return true if user is local or false if is a domain</returns>
    private bool IsDomainUser()
    {
        string userDomain = Environment.UserDomainName;
        string machineName = Environment.MachineName;

        return !userDomain.Equals(machineName, StringComparison.OrdinalIgnoreCase);
    }
}