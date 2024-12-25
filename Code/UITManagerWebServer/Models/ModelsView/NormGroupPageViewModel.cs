namespace UITManagerWebServer.Models.ModelsView {
    public class NormGroupPageViewModel {
        public int Id { get; set; }
        public string? NormGroupName { get; set; }
        public string? Severity { get; set; }
        public int Priority { get; set; }
        public int TotalAlarms { get; set; }
        public bool IsEnable { get; set; }
    }
}