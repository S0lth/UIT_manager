using System;
using Microsoft.AspNetCore.Identity;

namespace UITManagerWebServer.Models {
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