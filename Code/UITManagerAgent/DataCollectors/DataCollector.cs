
using System.Net.NetworkInformation;

namespace UITManagerAgent.DataCollectors;

public interface DataCollector
{ 
    public Information Collect();
}
