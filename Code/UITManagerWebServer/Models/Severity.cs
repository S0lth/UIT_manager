namespace UITManagerWebServer.Models {
    
    /// <summary>
    /// Represents a severity with a name and a description
    /// </summary>
    public class Severity {
        
        public int Id { get; set; }

        public string? Name { get; set; }
        
        public string? Description { get; set; }
        
        public List<SeverityHistory> SeverityHistories { get; set; }
        
        public Severity() {
            SeverityHistories = new List<SeverityHistory>();
        }
    }
}