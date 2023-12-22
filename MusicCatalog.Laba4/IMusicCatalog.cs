
namespace MusicCatalog.Laba5;
/// <summary>
/// Интерфейс для работы с музыкальным каталогом
/// </summary>
public interface IMusicCatalog
{
    /// <summary>
    /// Метод добавляет композицию к перечню
    /// </summary>
    /// <param name="composition">Композиция, которую следует добавить</param>
    Task<Composition> AddComposition(Composition composition);
    /// <summary>
    /// Метод возвращает enumerator для перебора всех композици каталога.
    /// Композиции отсортированы сначала по автору, потом по названию
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Composition>> EnumerateAllCompositions();
    /// <summary>
    /// Метод возвращает enumerator для перебора композиций, удовлетворяющих
    /// критерию поиска
    /// </summary>
    /// <param name="query">Критерий поиска композиций</param>
    /// <returns>Enumerator для перебора</returns>
    Task<IEnumerable<Composition>> Search(string query);
    /// <summary>
    /// Метод возвращает перчень композиций, удовлетворяющих запросу
    /// </summary>
    /// <param name="stype">Тип запроса. 1 - по автору, 2 - по названию, остальное - везде</param>
    /// <param name="query">Запрос</param>
    /// <returns></returns>
    Task<IEnumerable<Composition>> Search(int stype, string query);
    /// <summary>
    /// Метод удаляет из каталога композиции, удовлетворяющие критерию поиска
    /// </summary>
    /// <param name="query">Критерий поиска</param>
    /// <returns>Количество удаленных композиций</returns>
    Task<int> Remove(string query);
    /// <summary>
    /// Метод изменяет композицию в хранилище
    /// </summary>
    /// <param name="composition"></param>
    /// <returns>true - если все хорошо</returns>
    Task<bool> Update(Composition composition);
    /// <summary>
    /// Удалить композицю с указанным id
    /// </summary>
    /// <param name="id">id композиции</param>
    /// <returns>true - если все хорошо</returns>
    Task<bool> Delete(int id);
}
