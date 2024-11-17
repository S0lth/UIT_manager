using System;

namespace UITManagerWebServer.Models {
    public class Alarm {
        public int Id { get; set; }

        public AlarmStatus Status { get; set; }

        public DateTime TriggeredAt { get; set; }

        public string? Description { get; set; }

        public int MachineId { get; set; }
        public Machine? Machine { get; set; }

        public int NormGroupId { get; set; }
        public NormGroup? NormGroup { get; set; }
    }

    public enum AlarmStatus {
        New,
        Acknowledged,
        InProgress,
        Escalated,
        Monitoring,
        PendingReview,
        Deferred,
        Suppressed,
        FalsePositive,
        Resolved,
        Closed,
        Recurring,
        Reopened,
        UnderInvestigation,
        PendingVendorSupport,
        ResolvedWithWorkaround
    }
}