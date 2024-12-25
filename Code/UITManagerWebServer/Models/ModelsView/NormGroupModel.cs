namespace UITManagerWebServer.Models.ModelsView {
    public class NormGroupModel {
        public int Id { get; set; }
        public string? NormGroupName { get; set; }
        public int IdSeverity { get; set; }
        public int Priority { get; set; }
        public TimeSpan MaxExpectedProcessingTime { get; set; }
        public bool IsEnable { get; set; }

        public List<Severity> Severities { get; set; } = new();
        public List<Norm> Norms { get; set; } = new();
        public List<SeverityHistory>? SeverityHistories { get; set; } = new();
        public List<InformationName> Informations { get; set; } = new();
        public List<int> IdNormToDelete { get; set; } = new();
    }
}