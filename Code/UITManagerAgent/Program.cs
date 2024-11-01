using UITManagerAgent.DataCollectors;

public class Program {
    public static async Task Main(string[] args) {
        await RunOnce();
    }

    private static Task RunOnce() {
        UserCollector userCollector = new();
        Console.WriteLine(userCollector.Collect().ToString());

        IpsAddressesCollector ipsAddressesCollector = new();
        Console.WriteLine(ipsAddressesCollector.Collect().ToString());
        
        RamCollector ramCollector = new RamCollector();
        Console.WriteLine(ramCollector.Collect().ToString());

        DomainNameCollector domainNameCollector = new DomainNameCollector();
        Console.WriteLine(domainNameCollector.Collect().ToString());
        
        OsCollector osCollector = new();
        Console.WriteLine(osCollector.Collect().ToString());
        
        return Task.CompletedTask;
    }
}