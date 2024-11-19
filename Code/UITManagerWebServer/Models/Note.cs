using Microsoft.AspNetCore.Identity;

namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents a note associated with a machine, typically used for documentation or problem resolution.
    /// </summary>
    public class Note {
        public int Id { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? AuthorId { get; set; }
        public IdentityUser? Author { get; set; }

        public int MachineId { get; set; }
        public Machine? Machine { get; set; }

        public bool IsSolution { get; set; }
    }
}