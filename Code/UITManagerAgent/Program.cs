using System.Runtime.Versioning;
using UITManagerAgent;

[SupportedOSPlatform("windows")]
public class Program {

    public static async Task Main(string[] args) {
        using (TaskSchedulerAgent scheduler = new TaskSchedulerAgent(2, SendMachineInformation))
        {
            Console.WriteLine("=> Task scheduler is running. Press Enter to exit...");
            Console.ReadLine();
        }

        Console.WriteLine("=> Task scheduler stopped.");
    }

    [SupportedOSPlatform("windows")]
    private static async Task<Task> SendMachineInformation() {
        ApiCommunicator apiCommunicator = new ApiCommunicator("api/v1/agent");
        MachineInformation machineInformation = new MachineInformation();
        Console.WriteLine(machineInformation.ToJson());
        bool success = await apiCommunicator.SendMachineInformationAsync(machineInformation);

        Console.WriteLine(success
                ? "=> Machine's Information sent successfully."
                : "=> Machine's Information could not be sent.");

        return Task.CompletedTask;
    }
    
    
    
}