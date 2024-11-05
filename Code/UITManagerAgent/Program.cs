using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

[SupportedOSPlatform("windows")]
public class Program {
    public static async Task Main(string[] args) {
        await RunOnce();
    }

    [SupportedOSPlatform("windows")]
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
        
        UpTimeCollector upTimeCollector = new();
        Console.WriteLine(upTimeCollector.Collect().ToString());

        OsCollector osCollector = new();
        Console.WriteLine(osCollector.Collect().ToString());
        
        DiskCollector diskCollector = new DiskCollector();
        Console.WriteLine(diskCollector.Collect().ToString());
        
        MachineNameCollector machineNameCollector = new();
        Console.WriteLine(machineNameCollector.Collect().ToString());

        DirectXCollector directXCollector = new DirectXCollector();
        Console.WriteLine(directXCollector.Collect().ToString());

        return Task.CompletedTask;
    }
}