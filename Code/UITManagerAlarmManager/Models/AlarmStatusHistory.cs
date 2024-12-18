namespace UITManagerAlarmManager.Models {

    /// <summary>
    /// Represents a record of changes made to the status of an alarm.
    /// </summary>
    public class AlarmStatusHistory {
        
        public int Id { get; set; }
        
        public int AlarmId { get; set; }
        
        public Alarm? Alarm { get; set; }
        
        public DateTime? ModificationDate { get; set; }
        
        public int StatusTypeId { get; set; }
        
        public AlarmStatusType StatusType { get; set; }
        
        public string? UserId { get; set; }  
        public ApplicationUser? User { get; set; } 
        
    }
}