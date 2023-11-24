using VGStandard.Core.Metadata;

namespace VGStandard.WebAPI.ViewModels;


public class RomViewModel : Trackable
{
    public Guid Identifier { get; set; } = Guid.NewGuid();

    public int SystemId { get; set; }

    public virtual SystemViewModel? System { get; set; } = null;

    public int RegionId { get; set; }

    public virtual RegionViewModel? Region { get; set; } = null;

    public string? RomHashCRC { get; set; } = string.Empty;

    public string? RomHashMD5 { get; set; } = string.Empty;

    public string? RomHashSHA1 { get; set; } = string.Empty;

    public long? RomSize { get; set; }

    public string RomFileName { get; set; } = string.Empty;

    public string RomExtensionlessFileName { get; set; } = string.Empty;

    public string? RomParent { get; set; } = string.Empty;

    public string? RomSerial { get; set; } = string.Empty;

    public string? RomHeader { get; set; } = string.Empty;

    public string? RomLanguage { get; set; } = string.Empty;

    public string? TempRomRegion { get; set; } = string.Empty;

    public string? RomDumpSource { get; set; } = string.Empty;
}

public class CreateRomViewModel
{
    public Guid Identifier { get; set; } = Guid.NewGuid();

    public int SystemId { get; set; }

    public virtual SystemViewModel? System { get; set; } = null;

    public int RegionId { get; set; }

    public virtual RegionViewModel? Region { get; set; } = null;

    public string? RomHashCRC { get; set; } = string.Empty;

    public string? RomHashMD5 { get; set; } = string.Empty;

    public string? RomHashSHA1 { get; set; } = string.Empty;

    public long? RomSize { get; set; }

    public string RomFileName { get; set; } = string.Empty;

    public string RomExtensionlessFileName { get; set; } = string.Empty;

    public string? RomParent { get; set; } = string.Empty;

    public string? RomSerial { get; set; } = string.Empty;

    public string? RomHeader { get; set; } = string.Empty;

    public string? RomLanguage { get; set; } = string.Empty;

    public string? TempRomRegion { get; set; } = string.Empty;

    public string? RomDumpSource { get; set; } = string.Empty;
}

public class UpdateRomViewModel
{
    public long Id { get; set; }

    public Guid Identifier { get; set; } = Guid.NewGuid();

    public int SystemId { get; set; }

    public virtual SystemViewModel? System { get; set; } = null;

    public int RegionId { get; set; }

    public virtual RegionViewModel? Region { get; set; } = null;

    public string? RomHashCRC { get; set; } = string.Empty;

    public string? RomHashMD5 { get; set; } = string.Empty;

    public string? RomHashSHA1 { get; set; } = string.Empty;

    public long? RomSize { get; set; }

    public string RomFileName { get; set; } = string.Empty;

    public string RomExtensionlessFileName { get; set; } = string.Empty;

    public string? RomParent { get; set; } = string.Empty;

    public string? RomSerial { get; set; } = string.Empty;

    public string? RomHeader { get; set; } = string.Empty;

    public string? RomLanguage { get; set; } = string.Empty;

    public string? TempRomRegion { get; set; } = string.Empty;

    public string? RomDumpSource { get; set; } = string.Empty;
}