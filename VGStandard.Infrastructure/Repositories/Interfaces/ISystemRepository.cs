using VGStandard.DataImporter.Models;

namespace VGStandard.Data.Infrastructure.Repositories.Interfaces;

public interface ISystemRepository
{
    public IQueryable<GameSystem> Read();
    public void Update(GameSystem model);
    public void Add(GameSystem model);
    public List<GameSystem> Add(List<GameSystem> models);
    public void Delete(long id);
    public void BatchDelete(IEnumerable<GameSystem> models);
}
