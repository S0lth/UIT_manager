namespace UITManagerAgent.DataCollectors;

public class UsersInformation : Information
{
    private List<string> _usersList = new();
    
    public List<string> GetUsersList() => _usersList;
    
    public override string ToString() => $"{string.Join(", ", GetUsersList())}";
}