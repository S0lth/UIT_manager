using System.ComponentModel.DataAnnotations;

namespace UITManagerAlarmManager.Models {
    /// <summary>
    /// Represents a group of norms categorized by priority.
    /// </summary>
    public class NormGroup {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int Priority { get; set; }

        public TimeSpan MaxExpectedProcessingTime { get; set; }

        public bool IsEnable { get; set; }

        public List<Norm> Norms { get; set; }

        public NormGroup() {
            Norms = new List<Norm>();
        }

    }
}