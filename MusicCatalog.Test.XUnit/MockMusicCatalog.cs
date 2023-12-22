using MusicCatalog.Laba5;
using System.Collections.Generic;

namespace MusicCatalog.Test.XUnit;

/// <summary>
/// Фейковый музыкальный каталог. Используется для проверки работы коммандера, который
/// взаимедойствует с пользователм
/// </summary>
internal class MockMusicCatalog : IMusicCatalog
{
    /// <summary>
    /// Список композиций для проверки прохождения команды add
    /// </summary>
    public List<Composition> Compositions { get; set; } = new List<Composition>();
    /// <summary>
    /// Полученный запрос на поиск  - для проверки команды search
    /// </summary>
    public string SearhQuery { get; set; } = null!;
    /// <summary>
    /// Полученный запрос на удаление композиции - для проверки прохождения команды Remove
    /// </summary>
    public string RemoveQuery { get; set; } = null!;
    /// <summary>
    /// Счетчик запросов полного списка - для проверки прохожденния команды list
    /// </summary>
    public int ListCallsNumber { get; set; }
    
    /// <summary>
    /// Метод интерфейса <see cref="IMusicCatalog"/>
    /// </summary>
    /// <param name="composition"></param>
    public Task<Composition> AddComposition(Composition composition)
    {
        composition.Number = Compositions.Count == 0?0:Compositions.Max(c => c.Number)+1;
        Compositions.Add(composition);
        return Task<Composition>.FromResult(composition);
    }

    public Task<bool> Delete(int id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Метод интерфейса <see cref="IMusicCatalog"/>
    /// </summary>

    public Task<IEnumerable<Composition>> EnumerateAllCompositions()
    {
        ListCallsNumber++;
        return Task<IEnumerable<Composition>>.FromResult(Compositions.AsEnumerable());
    }
    /// <summary>
    /// Метод интерфейса <see cref="IMusicCatalog"/>
    /// </summary>

    public Task<int> Remove(string query)
    {
        RemoveQuery = query;
        return Task<int>.FromResult(0);
    }
    /// <summary>
    /// Метод интерфейса <see cref="IMusicCatalog"/>
    /// </summary>

    public Task<IEnumerable<Composition>> Search(string query)
    {
        SearhQuery = query;
        return Task<IEnumerable<Composition>>.FromResult(Compositions.AsEnumerable());
    }

    public Task<IEnumerable<Composition>> Search(int stype, string query)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(Composition composition)
    {
        throw new NotImplementedException();
    }
}
