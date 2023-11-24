using VGStandard.Core.Metadata;

namespace VGStandard.WebAPI.ViewModels;

public class RegionViewModel : Trackable
{
    public Guid Identifier { get; set; } = Guid.NewGuid();

    public string RegionName { get; set; } = string.Empty;
}

public class CreateRegionViewModel
{
    public Guid Identifier { get; set; } = Guid.NewGuid();

    public string RegionName { get; set; } = string.Empty;
}

public class UpdateRegionViewModel
{
    public long Id { get; set; }

    public Guid Identifier { get; set; } = Guid.NewGuid();

    public string RegionName { get; set; } = string.Empty;
}
