namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents a group of norms categorized by priority and severity level.
    /// </summary>
    public class NormGroup {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int Priority { get; set; }

        public SeverityLevel Severity { get; set; }

        public List<Norm> Norms { get; set; }

        public NormGroup() {
            Norms = new List<Norm>();
        }
    }

    /// <summary>
    /// Enumerates severity levels for a norm group, indicating its criticality.
    /// </summary>
    public enum SeverityLevel {
        Critical,
        High,
        Medium,
        Low,
        InfoOnly,
        Warning,
        NonCritical
    }
}