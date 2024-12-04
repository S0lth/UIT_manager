namespace UITManagerWebServer.Models {
    
    /// <summary>
    /// Represents a history of severity categorized by a severity, a group of norm and a username, with a update date
    /// </summary>
    public class SeverityHistory {

        public int Id { get; set; }
        
        public int IdSeverity { get; set; }
        public Severity? Severity { get; set; }
        
        public int IdNormGroup { get; set; }
        public NormGroup? NormGroup { get; set; }
        
        public string UserId { get; set; }  
        public ApplicationUser? User { get; set; } 
           
        public DateTime UpdateDate { get; set; }
    }
}