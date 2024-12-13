using System.Runtime.Versioning;
using System.Text.Json;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent;

/// <summary>
/// Global class that regroups all <see cref="Information"/> in a single List
/// </summary>
public class MachineInformation{
    private List<Information> _informationList = new();
    private Information _machineName;    
    private Information _model;

    /// <summary>
    /// Accessors of <see cref="MachineInformation.Informations"/> field
    /// </summary>
    public List<Information> InformationList {
        get => _informationList; 
        set => _informationList = value;
    }

    public Information MachineName {
        get => _machineName;
        set => _machineName = value;
    }
    
    public Information Model {
        get => _model;
        set => _model = value;
    }

    /// <summary>
    /// Constructor that add each <see cref="Information"/> into the List <see cref="Informations"/>
    /// </summary>
    [SupportedOSPlatform("windows")]
    public MachineInformation() {
        MachineName = new MachineNameCollector().Collect();
        Model = new ModelCollectors().Collect();
        InformationList = new(){
            new CpuCollectors().Collect(),
            new DirectXCollector().Collect(),
            new DiskCollector().Collect(),
            new DomainNameCollector().Collect(),
            new IpsAddressesCollector().Collect(),
            new OsCollector().Collect(),
            new RamCollector().Collect(),
            new TagCollector().Collect(),
            new UpTimeCollector().Collect(),
            new UserCollector().Collect(),
        };
    }

    /// <summary>
    /// Get a formatted string representation of all <see cref="Information"/>
    /// </summary>
    /// <returns> a formatted string of all <see cref="InformationList"/> data</returns>
    public override String ToString() {
        string str = String.Empty;
        foreach (Information information in InformationList) {
            str += information + Environment.NewLine;
        }

        return str;
    }

    /// <summary>
    /// This method merge all json formatted data of <see cref="InformationList"/> as a unique json string
    /// </summary>
    /// <returns>A string as a Json format which contains all <see cref="InformationList"/></returns>
    public string ToJson() {
        List<string> json = new();
        json.Add("Name : " +_machineName.ToJson()+ Environment.NewLine);
        json.Add(_model.ToJson()+ Environment.NewLine);
        foreach (Information information in InformationList) {
            json.Add(information.ToJson() + Environment.NewLine);
        }
        return $"[{string.Join(",", json)}]";
    }
}