namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents a group of norms categorized by priority.
    /// </summary>
    public class NormGroup {
        public int Id { get; set; }
        
        public string? Name { get; set; }

        public int Priority { get; set; }
        
        public TimeSpan MaxExpectedProcessingTime { get; set; }
        
        public bool IsEnable { get; set; }

        public List<SeverityHistory> SeverityHistories { get; set; }

        public List<Norm> Norms { get; set; }

        public NormGroup() {
            Norms = new List<Norm>();
            SeverityHistories = new List<SeverityHistory>();
        }
        
        /// <summary>
        /// Récupère la sévérité la plus récente de l'historique des sévérités du groupe de normes.
        /// </summary>
        /// <returns>La dernière sévérité, ou <c>null</c> si l'historique est vide.</returns>
        public SeverityHistory GetLatestSeverityHistory() {
            return SeverityHistories?.OrderByDescending(h => h.UpdateDate).FirstOrDefault();
        }
    }
}