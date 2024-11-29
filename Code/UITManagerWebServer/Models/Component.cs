namespace UITManagerWebServer.Models {
    public class Component : Informations{
        public int? MachinesId { get; set; }
        public Machine machine { get; set; }
    }
}