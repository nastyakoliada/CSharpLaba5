using MusicCatalog.Data;
namespace MusicCatalog.WebService.Repositories;
/// <summary>
/// Interface для взаимодействия с композициями
/// </summary>
public interface IMusicCatalogRepository
{
    /// <summary>
    /// Добавить композицию в хранилище
    /// </summary>
    /// <param name="composition">Композиция</param>
    /// <returns>Возвращаемая композиция содержит ID</returns>
    Task<Composition?> AddCompositionAsync(Composition composition);
    /// <summary>
    /// Возвращает композицию по ID
    /// </summary>
    /// <param name="id">Уникальный ключ к композиции</param>
    /// <returns>Композиция</returns>
    Task<Composition?> GetCompositionAsync(int? id);
    /// <summary>
    /// Все композиции в хранилище
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Composition>> EnumerateAllCompositionsAsync();
    /// <summary>
    /// Все композиции в хранилище, удовлетворящие запросу
    /// </summary>
    /// <param name="query">Запрос, например, имя автора или название песни</param>
    /// <returns></returns>
    Task<IEnumerable<Composition>> SearchAsync(string query);
    /// <summary>
    /// Метод возвращает перечень композиций, удовлетфоряющих условиям поиска
    /// </summary>
    /// <param name="stype">Тип поиска - указывает где искать</param>
    /// <param name="query">Строка поиска</param>
    /// <returns></returns>
    Task<IEnumerable<Composition>> SearchAsync(int stype,string query);
    /// <summary>
    /// Удаляет копозиции, удовлетворяющие запросу, из хранилища
    /// </summary>
    /// <param name="query">Запрос, например, имя автора или название песни</param>
    /// <returns>Количество удаленных композиций</returns>
    Task<int> RemoveAsync(string query);
    /// <summary>
    /// Изменяет композицию в хранилище
    /// </summary>
    /// <param name="composition">Композиция для изменения</param>
    /// <returns>true - если все хорошо</returns>
    Task<bool> UpdateCompositionAsync(Composition composition);
    /// <summary>
    /// Удаляет композицию с указанным id
    /// </summary>
    /// <param name="id">ID композиции для удаления</param>
    /// <returns>true - если все хорошо</returns>
    Task<bool> DeleteCompositionAync(int id);
}
