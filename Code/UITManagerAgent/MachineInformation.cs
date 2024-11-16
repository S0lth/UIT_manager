using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent;
/// <summary>
/// Global class that regroups all <see cref="Information"/> in a single List
/// </summary>
public class MachineInformation{
    private List<Information> _informations = new();

    /// <summary>
    /// Accessors of <see cref="MachineInformation.Informations"/> field
    /// </summary>
    public List<Information> Informations {
        get => _informations; 
        set => _informations = value;
    }

    /// <summary>
    /// Constructor that add each <see cref="Information"/> into the List <see cref="Informations"/>
    /// </summary>
    [SupportedOSPlatform("windows")]
    public MachineInformation() {
        Informations = new(){
            new CpuCollectors().Collect(),
            new DirectXCollector().Collect(),
            new DiskCollector().Collect(),
            new DomainNameCollector().Collect(),
            new IpsAddressesCollector().Collect(),
            new MachineNameCollector().Collect(),
            new OsCollector().Collect(),
            new RamCollector().Collect(),
            new UpTimeCollector().Collect(),
            new UserCollector().Collect(),
        };
    }

    /// <summary>
    /// Get a formatted string representation of all <see cref="Informations"/>
    /// </summary>
    /// <returns> a formatted string of all <see cref="Informations"/></returns>
    public override String ToString() {
        string str = String.Empty;
        foreach (Information information in Informations) {
            str += information + Environment.NewLine;
        }

        return str;
    }

    /// <summary>
    /// This method merge all json formatted datas of <see cref="Informations"/> as a unique json string
    /// </summary>
    /// <returns>A string as a Json format which contains all <see cref="Informations"/></returns>
    public string ToJson() {
        List<string> json = new();
        foreach (Information information in Informations) {
            json.Add(information.ToJson());
        }
        return $"[{string.Join(",", json)}]";
    }

}