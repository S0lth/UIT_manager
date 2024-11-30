
namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents a note associated with a machine, typically used for documentation or problem resolution.
    /// </summary>
    public class Note {
        public int Id { get; set; }

        /// <summary>
        /// The title of the note.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content of the note, in markdown format.
        /// </summary>
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? AuthorId { get; set; }
        public ApplicationUser? Author { get; set; }

        public int MachineId { get; set; }
        public Machine? Machine { get; set; }

        /// <summary>
        /// Indicates if this note is a solution or not.
        /// </summary>
        public bool IsSolution { get; set; }

        /// <summary>
        /// A collection of files (e.g., images) associated with this note.
        /// </summary>
        public ICollection<File>? Files { get; set; }
    }

    /// <summary>
    /// Represents a file associated with a note, such as an image in markdown.
    /// </summary>
    public class File {
        public int Id { get; set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The file's byte content stored in the database.
        /// </summary>
        public byte[] FileContent { get; set; }

        /// <summary>
        /// The MIME type of the file (e.g., image/jpeg, image/png).
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// The ID of the note this file is associated with.
        /// </summary>
        public int NoteId { get; set; }
        public Note? Note { get; set; }
    }
}