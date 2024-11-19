namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents a specific norm, which belongs to a norm group and may define compliance standards.
    /// </summary>
    public class Norm {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int NormGroupId { get; set; }
        public NormGroup? NormGroup { get; set; }
    }
}