namespace UITManagerWebServer.Models.ModelsView {
    public class UserViewModel {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActivate { get; set; }
        public string Role { get; set; }
    }
}