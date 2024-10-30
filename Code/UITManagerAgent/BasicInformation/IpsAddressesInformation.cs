namespace UITManagerAgent.DataCollectors;

public class IpsAddressesInformation : Information
{
    public List<string> IPSList = new();

    public override string ToString() => $"{string.Join(", ", IPSList)}";

}