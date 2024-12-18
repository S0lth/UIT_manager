using System.ComponentModel.DataAnnotations;

namespace UITManagerApi.Models;
 /// <summary>
 /// Represents a machine
 /// </summary>
 public class Machine {
     public int Id { get; set; }

     [Required]
     public string Name { get; set; }

     public List<Information> Informations { get; set; }

     public Boolean IsWorking { get; set; }

     public string? Model { get; set; }

     public DateTime? LastSeen { get; set; }

     public Machine() {
         Informations = new List<Information>();
     }

}
 