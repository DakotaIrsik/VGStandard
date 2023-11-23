using System.Collections.Generic;

namespace VGStandard.Core.Metadata;

public class StandardResponse<T>
{
    public List<T> Data { get; set; }
    public long Total { get; set; }
}


