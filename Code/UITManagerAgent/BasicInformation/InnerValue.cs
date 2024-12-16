namespace UITManagerAgent.BasicInformation {
    public class InnerValue(string name, string format,string value = "null",List<InnerValue>? innerValues = null) {
        public string? Name { get; set; } = name;
        public string? Value { get; set; } = value;
        public string? Format { get; set; } = format;
        public List<InnerValue>? InformationAgents { get; set; } = innerValues;
    }
}