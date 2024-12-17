namespace UITManagerAlarmManager.Models {
    public abstract class Information {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        
        public Information? Parent { get; set; }
        
        public List<Information>? Children { get; set; }
        
        public int? MachinesId { get; set; }
        
        public Machine Machine { get; set; }
        public string Value { get; set; }
        public string Format { get; set; }

        public Information() {
           Children = new List<Information>();
        }
    }
}