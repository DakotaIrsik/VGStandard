using VGStandard.DataImporter.Models;

namespace VGStandard.Data.Infrastructure.Repositories.Interfaces;

public interface IRomRepository
{
    public IQueryable<Rom> Read();
    public void Update(Rom model);
    public void Add(Rom model);
    public List<Rom> Add(List<Rom> models);
    public void Delete(long id);
    public void BatchDelete(IEnumerable<Rom> models);
}
