namespace VGStandard.Core.Metadata.Paging;

public interface IPaged : IPageable, ISortable
{
    int RowCount { get; }
}
