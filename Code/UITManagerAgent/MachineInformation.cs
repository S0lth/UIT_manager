using System.Runtime.Versioning;
using UITManagerAgent.BasicInformation;
using UITManagerAgent.DataCollectors;

namespace UITManagerAgent;

/// <summary>
/// Global class that regroups all <see cref="Information"/> in a single List
/// </summary>
public class MachineInformation{
    private List<Information> _informationList;
    private MachineNameInformation _name;    
    private ModelInformation _model;

    /// <summary>
    /// Accessors of <see cref="MachineInformation"/> field
    /// </summary>
    public List<Information> InformationList {
        get => _informationList; 
        set => _informationList = value;
    }

    public MachineNameInformation Name {
        get => _name;
        set => _name = value;
    }
    
    public ModelInformation Model {
        get => _model;
        set => _model = value;
    }

    /// <summary>
    /// Constructor that add each <see cref="Information"/> into the List <see cref="InformationList"/>
    /// </summary>
    [SupportedOSPlatform("windows")]
    public MachineInformation() {
        _informationList = new List<Information>();
        _name = new MachineNameInformation();
        _model =  new ModelInformation();
        
        GetValue();
    }
    
    [SupportedOSPlatform("windows")]
    public void GetValue() {
        Name = (MachineNameInformation) new MachineNameCollector().Collect();
        Model = (ModelInformation) new ModelCollectors().Collect();
        InformationList = new List<Information>{
            new CpuCollectors().Collect(),
            new DirectXCollector().Collect(),
            new DiskCollector().Collect(),
            new DomainNameCollector().Collect(),
            new IpsAddressesCollector().Collect(),
            new OsCollector().Collect(),
            new RamCollector().Collect(),
            new TagCollector().Collect(),
            new UpTimeCollector().Collect(),
            new UserCollector().Collect()
        };
    }

    /// <summary>
    /// This method merge all json formatted data of <see cref="InformationList"/> as a unique json string
    /// </summary>
    /// <returns>A string as a Json format which contains all <see cref="InformationList"/></returns>
    public string ToJson() {
        string informationListJson = string.Join(",", InformationList.Select(info => info.ToJson()));
        string nameJson = $"\"Name\":\"{Name.MachineName.Value}\"";
        string modelJson = $"\"Model\":\"{Model.Model.Value}\"";

        return $"{{{nameJson},\"Informations\":[{informationListJson}],{modelJson}}}";
    }
}

/*List<string> json = new();
json.Add("Name : " +_machineName.ToJson()+ Environment.NewLine);
json.Add(_model.ToJson()+ Environment.NewLine);
foreach (Information information in InformationList) {
    json.Add(information.ToJson() + Environment.NewLine);
}
return $"[{string.Join(",", json)}]";*/