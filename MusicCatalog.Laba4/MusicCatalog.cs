using MusicCatalog.Data;

namespace MusicCatalog.Laba5;
/// <summary>
/// Класс содержит перечень музыкальных композиций и предоставляет методы по работе с ним.
/// Поддерживается сериализация с исполльзование <see cref="ISerializer{T}"/>
/// </summary>
public class MusicCatalog : IMusicCatalog
{
    /// <summary>
    /// Сериализатор для использования
    /// </summary>
    private readonly ISerializer<IEnumerable<Composition>> serializer = null!;
    /// <summary>
    /// Конструктор с указанием сериализатора
    /// </summary>
    /// <param name="serializer">Сериализатор для использования в дальнейшем</param>
    public MusicCatalog(ISerializer<IEnumerable<Composition>> serializer)
    {
        this.serializer = serializer;
        // Поскольку конструктор соедржит чызов асинхронного метода, а теперь
        // класс может создаваться в потоке GUI, то для того, чтобы избежать deadlock
        // создадим объект используя дополнительный поток.
        // Если тотже код исполнять в основном потоке, то будет deadlock при обращении к Result
        var task = Task.Factory.StartNew(() =>
            {
                var res = serializer.Deserialize();
                res.Wait();
                Compositions = res?.Result?.ToList() ?? new List<Composition>();
            });        
        task.Wait();
        
        
        //Compositions = res?.ToList() ?? new List<Composition>();
    }    
    
    /// <summary>
    /// Перечень композиций
    /// </summary>
    private List<Composition> Compositions { get; set; } = null!;

    #region IMusicCatalog interface implementation
    /// <summary>
    /// Метод доавляет композицию к перечню
    /// </summary>
    /// <param name="composition">Композиция, которую следует добавить</param>
    public async Task<Composition> AddComposition(Composition composition)
    {
        composition.Number = Compositions.Count == 0 ? 0:Compositions.Max(c => c.Number) + 1;
        Compositions.Add(composition);
        await Serialize();
        return composition;
    }
    /// <summary>
    /// Метод возвращает enumerator для перебора всех композици каталога.
    /// Композиции отсортированы сначала по автору, потом по названию
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Composition>> EnumerateAllCompositions() => 
        await Task.Run(() =>Compositions.OrderBy(c => c.Author).ThenBy(c => c.SongName));

    /// <summary>
    /// Метод возвращает enumerator для перебора композиций, удовлетворяющих
    /// критерию поиска
    /// </summary>
    /// <param name="query">Критерий поиска композиций</param>
    /// <returns>Enumerator для перебора</returns>
    public async Task<IEnumerable<Composition>> Search(string query) => 
        await Task.Run(() => Compositions
            .Where(c => c.Author.Contains(query,StringComparison.OrdinalIgnoreCase) 
            || c.SongName.Contains(query,StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Author)
            .ThenBy(c => c.SongName));

    public async Task<IEnumerable<Composition>> Search(int stype, string query)
    {
        return stype switch
        {
            1 => Compositions
            .Where(c => c.Author.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Author)
            .ThenBy(c => c.SongName),
            2 => Compositions
            .Where(c => c.SongName.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Author)
            .ThenBy(c => c.SongName),
            _ => await Search(query),
        };
    }

    /// <summary>
    /// Метод удаляет из каталога композиции, удовлетворяющие критерию поиска
    /// </summary>
    /// <param name="query">Критерий поиска</param>
    /// <returns>Количество удаленных композиций</returns>
    public async Task<int> Remove(string query)
    {
        var removeList = await Search(query);
        int res = 0;
        foreach(var item in removeList)
        {
            Compositions.Remove(item);
            res++;
        }
        await Serialize();
        
        return res;
       
    }

    public async Task<bool> Update(Composition composition)
    {
        var existed = Compositions.FirstOrDefault(c => c.Number == composition.Number);
        if(existed is not null)
        {
            existed.Author = composition.Author;
            existed.SongName = composition.SongName;
            await Serialize();
            return true;
        }
        return false;
    }
    public async Task<bool> Delete(int id)
    {
        var existed = Compositions.FirstOrDefault(c => c.Number == id);
        if (existed is not null)
        {
            Compositions.Remove(existed);
            await Serialize();
            return true;
        }
        return false;
    }
    #endregion

    /// <summary>
    /// Сериализация каталога.
    /// </summary>
    private async  Task Serialize()
    {
        await serializer?.Serialize(Compositions)!;
    }
}
