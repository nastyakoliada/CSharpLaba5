using Microsoft.EntityFrameworkCore; //UseSqlite
using Microsoft.Extensions.DependencyInjection; //IServiceCollection

namespace MusicCatalog.Data;
/// <summary>
/// Метод расширения для подклчения контекста е сервисам webapi
/// </summary>
public static class MusicCatalogContextExtension
{
    public static IServiceCollection AddMusicCatalogContext(this IServiceCollection services,
        string relativePath="..")
    {
        string dbFile = Path.Combine(relativePath, "mc.db");
        services.AddDbContext<MusicCatalogContext>(op => op.UseSqlite($"Data Source={dbFile}"));
        
        return services;
    }
}
