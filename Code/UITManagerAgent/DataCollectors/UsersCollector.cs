namespace UITManagerAgent.DataCollectors;
using System.DirectoryServices;

public class UserCollector : DataCollector
{
    public Information Collect()
    {
        UsersInformation users = new UsersInformation();
        
        try {
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
            Console.WriteLine("Erreur lors de la récupération des utilisateurs : " + ex.Message);
        }

        return users;
    }
}