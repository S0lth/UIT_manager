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

        public Alarm() {
            AlarmHistories = new List<AlarmStatusHistory>();
        }

        public AlarmStatusHistory GetLatestAlarmHistory() {
            return AlarmHistories?.OrderByDescending(h => h.ModificationDate).FirstOrDefault();
        }

        public void AddAlarmHistory(AlarmStatusHistory alarmStatusHistory) {
            if (!AlarmHistories.Contains(alarmStatusHistory)) {
                AlarmHistories.Add(alarmStatusHistory);
            }
        }
    }
}