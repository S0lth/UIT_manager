namespace UITManagerWebServer.Models.ModelsView {
    public class AlarmViewModel {
        public string MachineName { get; set; }
        public int MachineId { get; set; }
        public string ModelName { get; set; }
        public int AlarmId { get; set; }
        public string Status { get; set; }
        public string Severity { get; set; }
        public string AlarmGroupName { get; set; }
        public DateTime TriggeredAt { get; set; }
    }
}