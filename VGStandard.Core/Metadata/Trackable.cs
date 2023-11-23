namespace VGStandard.Core.Metadata;

public interface ITrackable
{
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

public class Trackable : Active, ITrackable
{
    public string? CreatedBy { get; set; } = "System";
    public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; } = "System";
    public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;
}
