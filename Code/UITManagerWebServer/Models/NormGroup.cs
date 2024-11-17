namespace UITManagerWebServer.Models {
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