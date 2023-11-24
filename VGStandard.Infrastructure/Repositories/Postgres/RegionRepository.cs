using VGStandard.Data.Infrastructure.Contexts;
using VGStandard.Data.Infrastructure.Repositories.Interfaces;
using VGStandard.DataImporter.Models;

namespace VGStandard.Data.Infrastructure.Repositories.Postgres;

public class RegionRepository : IRegionRepository
{
    private readonly VideoGameContext _context;
    public RegionRepository(VideoGameContext context)
    {
        _context = context;
    }

    public IQueryable<Region> Read()
    {
        return _context.Regions/*.Where(k => k.IsActive)*/.AsQueryable();
    }
    public void Update(Region model)
    {
        _context.Regions.Update(model);
        _context.SaveChanges();
    }

    public void Add(Region model)
    {
        _context.Regions.Add(model);
        _context.SaveChanges();
    }
    public List<Region> Add(List<Region> models)
    {
        _context.Regions.AddRange(models);
        _context.SaveChanges();
        return models;
    }
    public void Delete(long id)
    {
        var entity = _context.Regions.Single(u => u.Id == id);
        _context.Remove(entity);
        _context.SaveChanges();
    }

    public void BatchDelete(IEnumerable<Region> models)
    {
        var entity = _context.Regions.Where(a => models.Select(m => m.Id).Contains(a.Id)).ToList();
        _context.RemoveRange(entity);
        _context.SaveChanges();
    }
}
