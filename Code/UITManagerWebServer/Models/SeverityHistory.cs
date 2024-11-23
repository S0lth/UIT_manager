namespace UITManagerWebServer.Models {
    
    public class SeverityHistory {

        public int Id { get; set; }
        
        public int IdSeverity { get; set; }
        public Severity? Severity { get; set; }
        
        public int IdNormGroup { get; set; }
        public NormGroup? NormGroup { get; set; }
        
        public DateTime UpdateDate { get; set; }
    }
}