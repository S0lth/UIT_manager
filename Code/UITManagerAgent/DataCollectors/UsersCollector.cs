using System.DirectoryServices;
using UITManagerAgent.DataCollectors;

/// <summary>
/// Collects user information from the system and returns it as a <see cref="UsersInformation"/> instance.
/// </summary>
public class UserCollector : DataCollector
{
    /// <summary>
    /// Collects user information by accessing the system directory entries.
    /// </summary>
    /// <returns>
    /// A <see cref="UsersInformation"/> object containing the list of usernames.
    /// </returns>
    public Information Collect()
    {
        UsersInformation users = new UsersInformation();
        
        try
        {
            using (DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName))
            {
                foreach (DirectoryEntry child in localMachine.Children)
                {
                    if (child.SchemaClassName == "User")
                    {
                        if (child != null) users.GetUsersList().Add(child.Name);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while retrieving users: " + ex.Message);
        }

        return users;
    }
}
