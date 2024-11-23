namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents the type of an alarm status, providing details such as name and description.
    /// </summary>
    public class AlarmStatusType {
        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
    }
}