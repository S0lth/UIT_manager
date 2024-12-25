namespace UITManagerWebServer.Models.ModelsView {
    public class DetailMachineViewModel {
        public int? Id { get; set; }
        public List<AlarmMachineViewModel> Alarms { get; set; }
        public List<NoteMachineViewModel> Notes { get; set; }
        public List<ComponentsViewModel> Informations { get; set; }
        public List<ApplicationUser> Authors { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public bool IsWorking { get; set; }
        public DateTime? LastSeen { get; set; }
        public bool AnyNote { get; set; }
        public bool AnyAlarms { get; set; }
    }
}