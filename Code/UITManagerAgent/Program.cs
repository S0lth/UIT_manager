using System.Runtime.Versioning;
using UITManagerAgent;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

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

    /// <summary>
    /// Sends the machine information to a specified API endpoint.
    /// </summary>
    /// <returns>
    /// An asynchronous task that completes once the operation is executed. 
    /// The returned task always represents a completed operation.
    /// </returns>
    [SupportedOSPlatform("windows")]
    private static async Task<Task> SendMachineInformation() {
        ApiCommunicator apiCommunicator = new ApiCommunicator("api/v1.0/agent");
        MachineInformation machineInformation = new MachineInformation();
        Console.WriteLine(machineInformation.ToJson());
        bool success = await apiCommunicator.SendMachineInformationAsync(machineInformation);

        Console.WriteLine(success
                ? "=> Machine's Information sent successfully."
                : "=> Machine's Information could not be sent.");

        return Task.CompletedTask;
    }
}