namespace UITManagerWebServer.Models {
    public class Note {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AuthorId { get; set; }
        public ApplicationUser? Author { get; set; }
        public int MachineId { get; set; }
        public Machine? Machine { get; set; }
        public bool IsSolution { get; set; }
        public ICollection<File>? Files { get; set; }
    }

    public class File {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string MimeType { get; set; }
        public bool IsTemporary { get; set; } = false;
        public int? NoteId { get; set; }
        public Note? Note { get; set; }
    }
}