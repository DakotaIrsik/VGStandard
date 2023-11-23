using VGStandard.Core.Metadata;

namespace VGStandard.Core.Models.Audit
{
    public class AlertAudit : Trackable
    {
        public DateTime Starttime { get; set; }
        public DateTime Alerttime { get; set; }
        public DateTime? Endtime { get; set; }
        public string? Filename { get; set; }
        public bool Falsepositive { get; set; }
        public bool Friendly { get; set; }
        public long CameraId { get; set; }
        public long ClientId { get; set; }
        public bool Alertacknowledged { get; set; }
        public bool Dispatchreceived { get; set; }
        public bool Dispatchacknowledged { get; set; }
        public string? DispatchedBy { get; set; }
        public string? AcknowledgedBy { get; set; }
        public string? Presignedurl { get; set; }
        public bool Alertcleared { get; set; }
        public bool Dispatchcleared { get; set; }
        public bool Errored { get; set; }
        //public long? MetricId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

