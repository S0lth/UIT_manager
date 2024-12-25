namespace UITManagerWebServer.Models.ModelsView {
    
        /// <summary>
        /// Represents the data for a machine, including its details, status, and associated notes.
        /// </summary>
        public class MachineViewModel {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Model { get; set; }

            public DateTime LastSeen { get; set; }

            public string Os { get; set; }

            public string Build { get; set; }

            public string ServiceTag { get; set; }

            public bool IsWorking { get; set; }

            public int NoteCount { get; set; }

            public Note LastNote { get; set; }

            /// <summary>
            /// Gets the difference between the system date and the last seen date
            /// The latest note is determined by the most recent creation date.
            /// </summary>
            /// <returns>A string containing the difference</returns>
            public string GetLastSeen() {
                TimeSpan timeSpan = DateTime.Now - LastSeen;

                return timeSpan.TotalMinutes switch {
                    < 1 => "Just now",
                    < 60 => $"{(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes >= 2 ? "s" : "")} ago",
                    < 1440 => $"{(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours >= 2 ? "s" : "")} ago",
                    < 43200 => $"{(int)timeSpan.TotalDays} day{(timeSpan.TotalDays >= 2 ? "s" : "")} ago",
                    < 525600 =>
                        $"{(int)(timeSpan.TotalDays / 30)} month{(timeSpan.TotalDays / 30 >= 2 ? "s" : "")} ago",
                    _ => $"{(int)(timeSpan.TotalDays / 365)} year{(timeSpan.TotalDays / 365 >= 2 ? "s" : "")} ago",
                };
            }
        }
}