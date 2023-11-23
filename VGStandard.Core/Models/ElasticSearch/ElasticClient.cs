using Nest;

namespace VGStandard.Core.Models.ElasticSearch;

public class ElasticClient : BaseElastic
{
    public static CreateIndexDescriptor IndexDescriptor => new CreateIndexDescriptor("client").Map<ElasticClient>(m => m.AutoMap());
    [Text]
    public string Name { get; set; }

    [Text]
    public string Guid { get; set; } = string.Empty;

    [Text]
    public string Notes { get; set; }

    [Text]
    public string Endpoint { get; set; }
}
