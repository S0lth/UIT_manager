namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents an employee, including their personal details and role in the system.
    /// </summary>
    public class Employee {
        
        public int Id { get; set; }
        
        public string LastName { get; set; }
        
        public string FirstName { get; set; }
        
        public string Role { get; set; }
    }
}