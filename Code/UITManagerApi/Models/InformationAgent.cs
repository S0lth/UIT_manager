namespace UITManagerApi.Models;
public class InformationAgent {
    public string Name { get; set; }
    public string Value { get; set; }
    public string Format { get; set; }   
    
    public List<InformationAgent>? InformationAgents { get; set; }

    public InformationAgent() { }
}