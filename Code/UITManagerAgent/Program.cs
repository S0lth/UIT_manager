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
        return Task.CompletedTask;
    }
}