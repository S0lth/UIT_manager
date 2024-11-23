using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents an alarm entity used to track events or conditions in a system.
    /// </summary>
    public class Alarm {
        public int Id { get; set; }

        public int AlarmStatusId { get; set; }
        
        public AlarmStatus AlarmStatus { get; set; }

        public DateTime TriggeredAt { get; set; }

        public int MachineId { get; set; }
        public Machine? Machine { get; set; }

        public int NormGroupId { get; set; }
        
        public NormGroup? NormGroup { get; set; }
    }
}