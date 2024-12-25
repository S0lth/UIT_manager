namespace UITManagerWebServer.Models.ModelsView {
    public class ComponentsViewModel {
        public int MachineId { get; set; }
        public int? ParentId { get; set; }
        public int id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Format { get; set; }
        public List<ComponentsViewModel> Children { get; set; }
    }
}