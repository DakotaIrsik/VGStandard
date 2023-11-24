using VGStandard.Data.Infrastructure.Contexts;
using VGStandard.Data.Infrastructure.Repositories.Interfaces;
using VGStandard.DataImporter.Models;

namespace VGStandard.Data.Infrastructure.Repositories.Postgres;

public class SystemRepository : ISystemRepository
{
    private readonly VideoGameContext _context;
    public SystemRepository(VideoGameContext context)
    {
        _context = context;
    }

    public IQueryable<GameSystem> Read()
    {
        return _context.GameSystems/*.Where(k => k.IsActive)*/.AsQueryable();
    }
    public void Update(GameSystem model)
    {
        _context.GameSystems.Update(model);
        _context.SaveChanges();
    }

    public void Add(GameSystem model)
    {
        _context.GameSystems.Add(model);
        _context.SaveChanges();
    }
    public List<GameSystem> Add(List<GameSystem> models)
    {
        _context.GameSystems.AddRange(models);
        _context.SaveChanges();
        return models;
    }
    public void Delete(long id)
    {
        var entity = _context.GameSystems.Single(u => u.Id == id);
        _context.Remove(entity);
    }

    public void BatchDelete(IEnumerable<GameSystem> models)
    {
        var entity = _context.GameSystems.Where(a => models.Select(m => m.Id).Contains(a.Id)).ToList();
        _context.RemoveRange(entity);
    }
}
