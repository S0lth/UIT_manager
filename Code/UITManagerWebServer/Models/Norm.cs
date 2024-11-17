namespace UITManagerWebServer.Models {
    public class Norm {
        public int Id { get; set; }

        public string Name { get; set; }

        public int NormGroupId { get; set; }
        public NormGroup NormGroup { get; set; }
    }
}