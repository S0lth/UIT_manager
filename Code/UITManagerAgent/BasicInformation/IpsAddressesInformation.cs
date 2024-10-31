using UITManagerAgent.DataCollectors;

namespace UITManagerAgent.BasicInformation;

public class IpsAddressesInformation : Information
{
    private List<string> _ipsList = new();

    
    public List<string> GetIpsList()
    {
        return _ipsList;
    }

    public override string ToString() => $"{string.Join(", ", _ipsList)}";

}