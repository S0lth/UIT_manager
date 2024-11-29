namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents a machine, which can be associated with alarms and notes.
    /// </summary>
    public class Machine {
        public int Id { get; set; }

        public string? Name { get; set; }

        public List<Alarm> Alarms { get; set; }

        public List<Note> Notes { get; set; }

        public Boolean IsWorking { get; set; }

        public string? Model { get; set; }

        public DateTime LastSeenDate { get; set; }

        public string? Build { get; set; }

        public string? Os { get; set; }

        public string? ServiceTag { get; set; }

        public Machine() {
            Alarms = new List<Alarm>();
            Notes = new List<Note>();
        }

        /// <summary>
        /// Gets the most recent note entry of the machine.
        /// The latest note is determined by the most recent creation date.
        /// </summary>
        /// <returns>The most recent note of the machine, or <c>null</c> if no note is available.</returns>
        public Note? GetLatestNote() {
            return Notes.OrderByDescending(n => n.CreatedAt).FirstOrDefault();
        }
    }
}