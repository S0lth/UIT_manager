using System.Runtime.Versioning;
using UITManagerAgent;


[SupportedOSPlatform("windows")]
public class Program {

    public static async Task Main(string[] args) {
        await RunOnce();
    }

    [SupportedOSPlatform("windows")]
    private static async Task<Task> RunOnce() {

        ApiCommunicator apiCommunicator = new ApiCommunicator("api/v1/agent");
        MachineInformation machineInformation = new MachineInformation();
        Console.WriteLine(machineInformation.ToJson());
        bool success = await apiCommunicator.SendMachineInformationAsync(machineInformation);

        Console.WriteLine(success
                ? "Les informations de la machine ont �t� envoy�es avec succ�s."
                : "�chec de l'envoi des informations de la machine.");

        return Task.CompletedTask;
    }
}