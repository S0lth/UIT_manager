using Microsoft.AspNetCore.Identity;

namespace UITManagerAlarmManager.Models {
    public class ApplicationUser : IdentityUser{
       public string FirstName { get; set; }
       public string LastName { get; set; }
       public DateTime StartDate { get; set; }
       public DateTime? EndDate { get; set; }
       public bool IsActivate { get; set; }
    }
}