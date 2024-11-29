using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace UITManagerWebServer.Models {
 
    public class Alarm {
        public int Id { get; set; }
        public List<AlarmStatusHistory> AlarmHistories { get; set; }

        public DateTime TriggeredAt { get; set; }

        public int MachineId { get; set; }
        
        public Machine? Machine { get; set; }

        public int NormGroupId { get; set; }
        
        public NormGroup? NormGroup { get; set; }
        
        public string? UserId { get; set; }
        
        public ApplicationUser User { get; set; }

        public Alarm() {
            AlarmHistories = new List<AlarmStatusHistory>();
        }

        /// <summary>
        /// Gets the most recent status history entry of the alarm.
        /// The status is determined by the most recent modification date.
        /// </summary>
        /// <returns>The most recent status history entry of the alarm, or <c>null</c> if no history is available.</returns>
        public AlarmStatusHistory GetLatestAlarmHistory() {
            return AlarmHistories?.OrderByDescending(h => h.ModificationDate).FirstOrDefault();
        }

        /// <summary>
        /// Adds a new status history to the alarm.
        /// If the status history is already present, it will not be added again.
        /// </summary>
        /// <param name="alarmStatusHistory">The <see cref="AlarmStatusHistory"/> object to add.</param>
        public void AddAlarmHistory(AlarmStatusHistory alarmStatusHistory) {
            if (!AlarmHistories.Contains(alarmStatusHistory)) {
                AlarmHistories.Add(alarmStatusHistory);
            }
        }
    }
}