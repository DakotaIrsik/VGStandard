using Microsoft.EntityFrameworkCore;
using VGStandard.DataImporter.Models;

namespace VGStandard.Data.Contexts;

public class VideoGameContext : DbContext
{
    public VideoGameContext(DbContextOptions<VideoGameContext> options)
        : base(options)
    {
    }

    public DbSet<Region> Regions { get; set; }
    public DbSet<Release> Releases { get; set; }
    public DbSet<Rom> Roms { get; set; }
    public DbSet<GameSystem> GameSystems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}