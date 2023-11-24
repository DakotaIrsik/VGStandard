using VGStandard.DataImporter.Models;

namespace VGStandard.Data.Infrastructure.Repositories.Interfaces;

public interface IRegionRepository
{
    public IQueryable<Region> Read();
    public void Update(Region model);
    public void Add(Region model);
    public List<Region> Add(List<Region> models);
    public void Delete(long id);
    public void BatchDelete(IEnumerable<Region> models);
}