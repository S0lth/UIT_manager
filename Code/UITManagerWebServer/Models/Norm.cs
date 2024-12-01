namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents a specific norm, which belongs to a norm group and may define compliance standards.
    /// </summary>
    public class Norm {
        public int Id { get; set; }

        public InformationName? InformationName { get; set; }
        public int? InformationNameId { get; set; }
        public string? Name { get; set; }
        public string? Condition { get; set; } // for example "<" ">" "=" "IN" "NOT IN"
        public string? Format { get; set; } // Go, TEXT, etc
        public string? Value { get; set; }
        public int NormGroupId { get; set; }
        public NormGroup? NormGroup { get; set; }
    }
}