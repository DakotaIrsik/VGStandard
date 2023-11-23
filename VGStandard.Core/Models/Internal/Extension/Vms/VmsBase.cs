using VGStandard.Core.Metadata;

namespace VGStandard.Core.Models.Internal.Extension.Vms;

public class VmsBase : Trackable
{
    public string Server { get; set; }
    public int Port { get; set; }
    public long? ClientId { get; set; }
    public string Config { get; set; }
    public long VmspluginId { get; set; }
}
