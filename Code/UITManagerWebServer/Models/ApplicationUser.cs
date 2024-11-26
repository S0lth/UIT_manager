using Microsoft.AspNetCore.Identity;

namespace UITManagerWebServer.Models {
    public class ApplicationUser : IdentityUser{
       public string FirstName { get; set; }
       public string LastName { get; set; }
       public DateTime StartDate { get; set; }
       public DateTime? EndDate { get; set; }
    }
}