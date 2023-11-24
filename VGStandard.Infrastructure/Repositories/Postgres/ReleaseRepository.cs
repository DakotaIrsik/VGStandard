using VGStandard.Data.Infrastructure.Contexts;
using VGStandard.Data.Infrastructure.Repositories.Interfaces;
using VGStandard.DataImporter.Models;

namespace VGStandard.Data.Infrastructure.Repositories.Postgres;

public class ReleaseRepository : IReleaseRepository
{
    private readonly VideoGameContext _context;
    public ReleaseRepository(VideoGameContext context)
    {
        _context = context;
    }

    public IQueryable<Release> Read()
    {
        return _context.Releases/*.Where(k => k.IsActive)*/.AsQueryable();
    }
    public void Update(Release model)
    {
        _context.Releases.Update(model);
        _context.SaveChanges();
    }

    public void Add(Release model)
    {
        _context.Releases.Add(model);
        _context.SaveChanges();
    }
    public List<Release> Add(List<Release> models)
    {
        _context.Releases.AddRange(models);
        _context.SaveChanges();
        return models;
    }
    public void Delete(long id)
    {
        var entity = _context.Releases.Single(u => u.Id == id);
        _context.Remove(entity);
        _context.SaveChanges();
    }

    public void BatchDelete(IEnumerable<Release> models)
    {
        var entity = _context.Releases.Where(a => models.Select(m => m.Id).Contains(a.Id)).ToList();
        _context.RemoveRange(entity);
        _context.SaveChanges();
    }
}
