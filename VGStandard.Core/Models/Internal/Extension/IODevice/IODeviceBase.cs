using Microsoft.Extensions.Logging;
using VGStandard.Core.Metadata;

namespace VGStandard.Core.Models.Internal.Extension.IODevice;

public class IODeviceBase : Trackable
{
    public string Server { get; set; }
    public long ClientId { get; set; }
    public int? Port { get; set; }
    public string Config { get; set; }
    public long? IODeviceId { get; set; }
    public long? DeviceGroupId { get; set; }

}
