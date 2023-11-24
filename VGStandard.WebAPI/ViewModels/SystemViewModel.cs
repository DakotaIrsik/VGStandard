using VGStandard.Core.Metadata;

namespace VGStandard.WebAPI.ViewModels;

public class SystemViewModel : Trackable
{
    public Guid Identifier { get; set; } = Guid.NewGuid();

    public string SystemName { get; set; } = string.Empty;

    public string SystemShortName { get; set; } = string.Empty;

    public int? SystemHeaderSizeBytes { get; set; }

    public string? SystemHashless { get; set; } = string.Empty;

    public string? SystemHeader { get; set; } = string.Empty;

    public string? SystemSerial { get; set; } = string.Empty;

    public string SystemOEID { get; set; } = string.Empty;
}

public class CreateSystemViewModel
{
    public Guid Identifier { get; set; } = Guid.NewGuid();

    public string SystemName { get; set; } = string.Empty;

    public string SystemShortName { get; set; } = string.Empty;

    public int? SystemHeaderSizeBytes { get; set; }

    public string? SystemHashless { get; set; } = string.Empty;

    public string? SystemHeader { get; set; } = string.Empty;

    public string? SystemSerial { get; set; } = string.Empty;

    public string SystemOEID { get; set; } = string.Empty;
}

public class UpdateSystemViewModel
{
    public long Id { get; set; }

    public Guid Identifier { get; set; } = Guid.NewGuid();

    public string SystemName { get; set; } = string.Empty;

    public string SystemShortName { get; set; } = string.Empty;

    public int? SystemHeaderSizeBytes { get; set; }

    public string? SystemHashless { get; set; } = string.Empty;

    public string? SystemHeader { get; set; } = string.Empty;

    public string? SystemSerial { get; set; } = string.Empty;

    public string SystemOEID { get; set; } = string.Empty;
}
