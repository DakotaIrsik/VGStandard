using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VGStandard.Core.Metadata;

public interface IActive
{
    public bool? IsActive { get; set; }
}

public class Active : Identifiable, IActive
{
    public bool? IsActive { get; set; }
}
