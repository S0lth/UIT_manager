namespace UITManagerWebServer.Models {
    public class InformationName {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }

        public List<InformationName> SubInformationNames;

        public InformationName() {
            SubInformationNames = new List<InformationName>();
        }
    }
}