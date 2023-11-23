using Nest;
using System;

namespace VGStandard.Core.Models.ElasticSearch;

public class BaseElastic
{
    public long Id { get; set; }

    [Keyword]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; } = "System";

    public string UpdatedBy { get; set; } = "System";
}
