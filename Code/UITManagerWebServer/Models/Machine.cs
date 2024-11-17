using System.Collections.Generic;

namespace UITManagerWebServer.Models {
    public class Machine {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Alarm> Alarms { get; set; }

        public List<Note> Notes { get; set; }

        public Machine() {
            Alarms = new List<Alarm>();
            Notes = new List<Note>();
        }
    }
}