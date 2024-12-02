namespace UITManagerWebServer.Models {
    public abstract class Informations {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        
        public Informations? Parent { get; set; }
        public List<Informations>? Children { get; set; }
        public int? MachinesId { get; set; }
        public Machine Machine { get; set; }
        public string Values { get; set; }

        public Informations() {
           Children = new List<Informations>();
        }
    }
}