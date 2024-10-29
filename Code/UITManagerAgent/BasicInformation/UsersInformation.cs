namespace UITManagerAgent.DataCollectors;

public class UsersInformation : Information
{
    public List<string> usersList = new();
    
    public override string ToString() => $"{string.Join(", ", usersList)}";
}