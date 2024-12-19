namespace UITManagerAgent.BasicInformation;
/// <summary>
/// Provides information on all disk names, total storage capacity, free storage capacity, and number of disks
/// </summary>
public class DiskInformation : Information {
    
    /// <summary>
    /// DisksList main fields.
    /// </summary>
    public InnerValue DisksList { get; set; } = new("List Disk", "null");

    /// <summary>
    /// Detailed DisksList information agents.
    /// </summary>
    public List<InnerValue> InformationAgents { get; set; } = new();

    /// <summary>
    /// Returns a Json string representation of the DiskInformation.
    /// </summary>
    /// <returns>A Json string that represents the DiskInformation.</returns>
    public override string ToJson() {

        string agentsJson = CreateAgentsJson();
        
        return $@"{{""Name"": ""{DisksList.Name}"",""Value"": ""{DisksList.Value}"",""Format"": ""{DisksList.Format}"",""InformationAgents"": [{agentsJson}]}}";
    }
    
    
    /// <summary>
    /// Returns a Json string representation of the Disks.
    /// </summary>
    /// <returns>A Json string that represents the Disks.</returns>
    private string CreateAgentsJson() {
        
        var agentsJsonList = new List<string>();

        foreach (var agent in InformationAgents) {
            var agentJsonList = new List<string>();

            if (agent.InformationAgents != null) {
                foreach (var disks in agent.InformationAgents) {
                    var diskJsonList = new List<string>();

                    if (disks.InformationAgents != null) {
                        foreach (var disk in disks.InformationAgents) {
                            diskJsonList.Add(
                                $@"{{""Name"":""{disk.Name}"",""Value"":""{disk.Value}"",""Format"":""{disk.Format}""}}");
                        }
                    }

                    agentJsonList.Add(
                        $@"{{""Name"":""{disks.Name}"",""Value"":""{disks.Value}"",""Format"":""{disks.Format}"",""InformationAgents"":[{string.Join(",", diskJsonList)}]}}");
                }
            }

            agentsJsonList.Add(
                $@"{{""Name"":""{agent.Name}"",""Value"":""{agent.Value}"",""Format"":""{agent.Format}"",""InformationAgents"":[{string.Join(",", agentJsonList)}]}}");
        }

        string agentsJson = string.Join(",", agentsJsonList);

        return agentsJson;
    }
    
    /// <summary>
    /// Represents a disk with properties for its name, total size, and available free space.
    /// </summary>
    public class Disk {
        
        /// <summary>
        /// accessors of the Disk field
        /// </summary>
        public InnerValue DiskTot { get; set; } = new("Disk Total Size","GB");
        public InnerValue DiskUsed { get; set; } = new("Disk Used","GB");
        public InnerValue DiskUsedPercent { get; set; } = new("Disk Used","%");
        public InnerValue DiskFree { get; set; } = new("Disk Free","GB");
        
        /// <summary>
        /// Return a List that represents information of a Disk.
        /// </summary>
        /// <returns>A List that represents information of a Disk.</returns>
        public List<InnerValue> GetList() {
            List<InnerValue> innerValues = new();
            innerValues.Add(DiskTot);
            innerValues.Add(DiskUsed);
            innerValues.Add(DiskUsedPercent);
            innerValues.Add(DiskFree);
            return innerValues;
        }
    }
    
}