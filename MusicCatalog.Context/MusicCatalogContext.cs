using Microsoft.EntityFrameworkCore;
namespace MusicCatalog.Data;

public class MusicCatalogContext : DbContext
{
    /// <summary>
    /// Имя файла бд. 
    /// </summary>
    private string fileName = "..\\mc.db";

    public DbSet<Composition> Compositions { get; set; }
    public MusicCatalogContext(string fileName)
    {
        this.fileName = fileName;
        Database.EnsureCreated();
    }
    public MusicCatalogContext()
    {
        Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!string.IsNullOrEmpty(fileName)) optionsBuilder.UseSqlite($"Filename={fileName}");
    }
}
