using VGStandard.DataImporter.Models;

namespace VGStandard.Data.Infrastructure.Repositories.Interfaces;

public interface IReleaseRepository
{
    public IQueryable<Release> Read();
    public void Update(Release model);
    public void Add(Release model);
    public List<Release> Add(List<Release> models);
    public void Delete(long id);
    public void BatchDelete(IEnumerable<Release> models);
}
