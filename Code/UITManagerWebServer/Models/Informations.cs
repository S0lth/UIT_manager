namespace UITManagerWebServer.Models {
    public abstract class Informations {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        
        public Informations? Parent { get; set; }
        public List<Informations>? Children { get; set; }
    }
}