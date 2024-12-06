namespace UITManagerWebServer.Models.ModelsView {
    public class HomePageViewModel {
        public List<NoteViewModel> Notes { get; set; }
        public int TotalMachines { get; set; }
        public int MachinesWithActiveAlarms { get; set; }
        public int AlarmsNotResolvedCount { get; set; }
        public int AlarmsTriggeredTodayCount { get; set; }
        public Dictionary<string, int> SeverityAlarmsCount { get; set; }
        public Dictionary<string, int> AssignedOrNotAlarmCount { get; set; }
        public Dictionary<string, Dictionary<string, double>> AlarmCountsBySiteAndSeverity { get; set; }
        public List<AlarmViewModel> Alarms { get; set; }
        public List<ApplicationUser> Authors { get; set; }
        public string ActiveTab { get; set; }
    }
}