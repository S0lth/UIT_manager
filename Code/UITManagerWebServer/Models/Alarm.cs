using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace UITManagerWebServer.Models {
    /// <summary>
    /// Represents an alarm associated with a machine, including its status, trigger time, and related entities.
    /// </summary>
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

    /// <summary>
    /// Enumerates the possible statuses of an alarm during its lifecycle.
    /// </summary>
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