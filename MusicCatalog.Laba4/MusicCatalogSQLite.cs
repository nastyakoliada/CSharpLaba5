
using MusicCatalog.Data;
using Microsoft.EntityFrameworkCore;

namespace MusicCatalog.Laba5;
/// <summary>
/// Класс музыкального каталога, котрый хранится в бд sqlite. База данных каталога создается на основе файла- шаблона
/// базы данных. Файл шаблона sqlite_template.db должен находиться в текущей папке приложения.
/// Пользовыатель указывает путь к файлу музыкального каталога, если такой файл не существует,
/// приложение копирует файл шаблона бд в файл с указанным именем. 
/// 
/// </summary>
public class MusicCatalogSQLite:IMusicCatalog
{
    
    /// <summary>
    /// Имя файла бд
    /// </summary>
    private string fileName = string.Empty;
    /// <summary>
    /// Конструктор для создания каталога
    /// </summary>
    /// <param name="path">Имя файла каталога</param>
    public MusicCatalogSQLite(string path)
    {
        fileName = path;
        
    }

    #region IMusicCatalog interface implementation
    /// <summary>
    /// Метод доавляет композицию к перечню
    /// </summary>
    /// <param name="composition">Композиция, которую следует добавить</param>
    public async Task<Composition> AddComposition(Composition composition)
    {
        using(MusicCatalogContext context = new MusicCatalogContext(fileName))
        {
            Data.Composition newC = new Data.Composition
            {
                Author = composition.Author,
                SongName = composition.SongName,
            };

            context.Compositions?.Add(newC);
            int affected = await context.SaveChangesAsync();
            if(affected == 1)
            {
                composition.Number = newC.ID;
            }            
        }
        return composition;
    }
    /// <summary>
    /// Удаляет композицию с указанным id из каталога 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> Delete(int id)
    {
        using (MusicCatalogContext context = new MusicCatalogContext(fileName))
        {
            var existed = await context.Compositions.FindAsync(id);
            if (existed != null)
            {
                context.Compositions.Remove(existed);
                return await context.SaveChangesAsync() > 0;
            }
        }
        return false;
    }

    /// <summary>
    /// Метод возвращает enumerator для перебора всех композици каталога.
    /// Композиции отсортированы сначала по автору, потом по названию
    /// </summary>
    /// <returns>перечень композиций</returns>
    public async Task<IEnumerable<Composition>> EnumerateAllCompositions()
    {
        using MusicCatalogContext context = new MusicCatalogContext(fileName);

        return await context.Compositions
            .OrderBy(c => c.Author)
            .ThenBy(c => c.SongName)
            .Select(c => new Composition
            {
                Author = c.Author ?? "",
                SongName = c.SongName ?? "",
                Number = c.ID
            }).ToListAsync();

    }

    /// <summary>
    /// Метод удаляет из каталога все композиции, которые удовлетворяют условиям поиска
    /// </summary>
    /// <param name="query">Критерий поиска композиций</param>
    /// <returns>Количество удаленных композиций</returns>
    public async Task<int> Remove(string query)
    {
        using MusicCatalogContext context = new MusicCatalogContext(fileName);

        var listToRemove =await context.Compositions
            .Where(c => (c.Author!.Contains(query))
            || (c.SongName!.Contains(query))).ToListAsync();

        context.Compositions.RemoveRange(listToRemove);
        await context.SaveChangesAsync();
        return listToRemove.Count;

    }

    /// <summary>
    /// Метод возвращает enumerator для перебора композици каталога, который удовлетворяют условиям поиска
    /// Композиции отсортированы сначала по автору, потом по названию
    /// </summary>    
    /// <param name="query">Критерий поиска</param>
    /// <returns>перечень композиций</returns>
    public async Task<IEnumerable<Composition>> Search(string query)
    {
        using MusicCatalogContext context = new MusicCatalogContext(fileName);

        return await context.Compositions
            .Where(c => c.Author!.Contains(query) || c.SongName!.Contains(query))
            .OrderBy(c => c.Author)
            .ThenBy(c => c.SongName)
            .Select(c => new Composition
            {
                Author = c.Author ?? "",
                SongName = c.SongName ?? "",
                Number = c.ID
            }).ToListAsync();

    }
    /// <summary>
    /// Возвращает перечень композиций из каталога, которые удовлетворяют заданным условиям поиска
    /// </summary>
    /// <param name="stype">Где искать</param>
    /// <param name="query">Поисковая строка</param>
    /// <returns></returns>
    public async Task<IEnumerable<Composition>> Search(int stype, string query)
    {
        using MusicCatalogContext context = new MusicCatalogContext(fileName);

        return (stype switch
        {
            1 => await context.Compositions
            .Where(c => c.Author!.Contains(query))
            .OrderBy(c => c.Author)
            .ThenBy(c => c.SongName)
            .Select(c => new Composition
            {
                Author = c.Author ?? "",
                SongName = c.SongName ?? "",
                Number = c.ID
            }).ToListAsync(),
            2 => await context.Compositions
            .Where(c => c.SongName!.Contains(query))
            .OrderBy(c => c.Author)
            .ThenBy(c => c.SongName)
            .Select(c => new Composition
            {
                Author = c.Author ?? "",
                SongName = c.SongName ?? "",
                Number = c.ID
            }).ToListAsync(),
            _ => await Search(query),
        });

    }
    /// <summary>
    /// Изменяет существующую композицию
    /// </summary>
    /// <param name="composition"></param>
    /// <returns>true - если все хорошо</returns>
    public async Task<bool> Update(Composition composition)
    {
        using MusicCatalogContext context = new MusicCatalogContext(fileName);
        var existed = await context.Compositions.FindAsync(composition.Number);
        if (existed is not null)
        {
            existed.Author = composition.Author;
            existed.SongName = composition.SongName;
            if (await context.SaveChangesAsync() > 0)
                return true;
        }

        return false;
    }
    #endregion
}
