using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VGStandard.Core.Metadata;

public interface IIdentifiable
{
    public long Id { get; set; }
    //public Guid Id { get; set; }
}

public class Identifiable : IIdentifiable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    //[Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //public Guid Id { get; set; }
}
