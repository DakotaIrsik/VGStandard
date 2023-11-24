using Nest;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using VGStandard.Core.Metadata;

namespace VGStandard.Application.DTOs;

public class ReleaseDTO : Trackable
{
    public Guid Identifier { get; set; } = Guid.NewGuid();

    public int RomId { get; set; }

    public virtual RomDTO? Rom { get; set; } = null;

    public string ReleaseTitleName { get; set; } = string.Empty;

    public int RegionLocalizedId { get; set; }

    public string TempRegionLocalizedName { get; set; } = string.Empty;

    public string TempSystemShortName { get; set; } = string.Empty;

    public string TempSystemName { get; set; } = string.Empty;

    public string? ReleaseCoverFront { get; set; } = string.Empty;

    public string? ReleaseCoverBack { get; set; } = string.Empty;

    public string? ReleaseCoverCart { get; set; } = string.Empty;

    public string? ReleaseCoverDisc { get; set; } = string.Empty;

    public string? ReleaseDescription { get; set; } = string.Empty;

    public string? ReleaseDeveloper { get; set; } = string.Empty;

    public string? ReleasePublisher { get; set; } = string.Empty;

    public string? ReleaseGenre { get; set; } = string.Empty;

    public string? ReleaseDate { get; set; } = string.Empty;  // Consider using DateTime? if the date is formatted

    public string? ReleaseReferenceURL { get; set; } = string.Empty;

    public string? ReleaseReferenceImageURL { get; set; } = string.Empty;
}
