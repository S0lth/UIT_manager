namespace UITManagerAgent.BasicInformation {
    
    /// <summary>
    /// Represents the value of basic information 
    /// </summary>
    public class InnerValue(string name, string format,string value = "null",List<InnerValue>? innerValues = null) {
        
        /// <summary>
        /// accessors of the Name field
        /// </summary>
        public string? Name { get; set; } = name;
        
        /// <summary>
        /// accessors of the Value field
        /// </summary>
        public string? Value { get; set; } = value;
        
        /// <summary>
        /// accessors of the Format field
        /// </summary>
        public string? Format { get; set; } = format;
        
        /// <summary>
        /// accessors of the InnerValue list
        /// </summary>
        public List<InnerValue>? InformationAgents { get; set; } = innerValues;
    }
}