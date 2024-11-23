namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents the status of an alarm, including its type, modification details, and the person who modified it.
    /// Allow some history about the change of an alarm's status
    /// </summary>
    public class AlarmStatus {
        
        public int Id { get; set; }
        
        public DateTime ModificationDate { get; set; }
        
        public int StatusTypeId { get; set; }
        
        public AlarmStatusType? StatusType { get; set; }
        
        public int ModifierId { get; set; }
        
        public Employee? Modifier { get; set; }
    }
}