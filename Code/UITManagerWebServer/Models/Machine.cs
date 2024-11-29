namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents a machine, which can be associated with alarms and notes.
    /// </summary>
    public class Machine {
        public int Id { get; set; }

        public string? Name { get; set; }

        public List<Alarm> Alarms { get; set; }

        public List<Note> Notes { get; set; }
        
        public List<Informations> Informations { get; set; }

        public Boolean IsWorking { get; set; }
        
        public TimeSpan LastSeen { get; set; }
        
        public Machine() {
            Alarms = new List<Alarm>();
            Notes = new List<Note>();
            Informations = new List<Informations>();
        }
    }
}