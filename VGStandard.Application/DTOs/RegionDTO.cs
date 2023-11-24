using VGStandard.Core.Metadata;

namespace VGStandard.Application.DTOs;

public class RegionDTO : Trackable
{
    public Guid Identifier { get; set; } = Guid.NewGuid();

    public string RegionName { get; set; } = string.Empty;
}
