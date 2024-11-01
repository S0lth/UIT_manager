using System.Runtime.Versioning;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent;

[SupportedOSPlatform("windows")]
public class Program {
    public static async Task Main(string[] args) {
        await RunOnce();
    }

    private static Task RunOnce() {
        UserCollector userCollector = new();
        Console.WriteLine(userCollector.Collect().ToString());

        CpuCollectors cpuCollected = new CpuCollectors();
        Console.WriteLine(cpuCollected.Collect().ToString());
        
        IpsAddressesCollector ipsAddressesCollector = new();
        Console.WriteLine(ipsAddressesCollector.Collect().ToString());

        RamCollector ramCollector = new RamCollector();
        Console.WriteLine(ramCollector.Collect().ToString());

        DomainNameCollector domainNameCollector = new DomainNameCollector();
        Console.WriteLine(domainNameCollector.Collect().ToString());

        DiskCollector diskCollector = new DiskCollector();
        Console.WriteLine(diskCollector.Collect().ToString());

        return Task.CompletedTask;
    }
}