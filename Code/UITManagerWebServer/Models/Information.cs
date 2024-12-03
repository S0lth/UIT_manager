namespace UITManagerWebServer.Models {
    public abstract class Information {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        
        public Information? Parent { get; set; }
        
        public List<Information>? Children { get; set; }
        
        public int? MachinesId { get; set; }
        
        public Machine Machine { get; set; }
        public string Values { get; set; }
        
        public Information() {
           Children = new List<Information>();
        }
    }
}