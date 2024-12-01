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

        public string? Model { get; set; }

        public DateTime LastSeen { get; set; }

        public Machine() {
            Alarms = new List<Alarm>();
            Notes = new List<Note>();
            Informations = new List<Informations>();
        }

        /// <summary>
        /// Gets the most recent note entry of the machine.
        /// The latest note is determined by the most recent creation date.
        /// </summary>
        /// <returns>The most recent note of the machine, or <c>null</c> if no note is available.</returns>
        public Note GetLatestNote() {
            return Notes.OrderByDescending(n => n.CreatedAt).FirstOrDefault();
        }

        public string GetOsName() {
            Component? osComponent = Informations
                .OfType<Component>()
                .FirstOrDefault(c => c.Name == "OS");

            Value? osName = osComponent?.Children?
                .OfType<Value>()
                .FirstOrDefault(v => v.Name == "OS Name");

            return osName?.Values;
        }

        public string GetOsVersion() {
            Component? osComponent = Informations
                .OfType<Component>()
                .FirstOrDefault(c => c.Name == "OS");

            Value? osVersion = osComponent?.Children?
                .OfType<Value>()
                .FirstOrDefault(v => v.Name == "Os Version");

            return osVersion?.Values;
        }

        public string GetOsBuild() {
            Component? osComponent = Informations
                .OfType<Component>()
                .FirstOrDefault(c => c.Name == "OS");

            Value? osBuild = osComponent?.Children?
                .OfType<Value>()
                .FirstOrDefault(v => v.Name == "Os Build");

            return osBuild?.Values;
        }

        public string GetServiceTag() {
            Value? serviceTag = Informations
                .OfType<Value>()
                .FirstOrDefault(v => v.Name == "Tag Service");

            return serviceTag?.Values;
        }

        
    }
}