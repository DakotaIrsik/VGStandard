using VGStandard.Data.Infrastructure.Contexts;
using VGStandard.Data.Infrastructure.Repositories.Interfaces;
using VGStandard.DataImporter.Models;

namespace VGStandard.Data.Infrastructure.Repositories.Postgres;

public class RomRepository : IRomRepository
{
    private readonly VideoGameContext _context;
    public RomRepository(VideoGameContext context)
    {
        _context = context;
    }

    public IQueryable<Rom> Read()
    {
        return _context.Roms/*.Where(k => k.IsActive)*/.AsQueryable();
    }
    public void Update(Rom model)
    {
        _context.Roms.Update(model);
        _context.SaveChanges();
    }

    public void Add(Rom model)
    {
        _context.Roms.Add(model);
        _context.SaveChanges();
    }
    public List<Rom> Add(List<Rom> models)
    {
        _context.Roms.AddRange(models);
        _context.SaveChanges();
        return models;
    }
    public void Delete(long id)
    {
        var entity = _context.Roms.Single(u => u.Id == id);
        _context.Remove(entity);
        _context.SaveChanges();
    }

    public void BatchDelete(IEnumerable<Rom> models)
    {
        var entity = _context.Roms.Where(a => models.Select(m => m.Id).Contains(a.Id)).ToList();
        _context.RemoveRange(entity);
        _context.SaveChanges();
    }
}
