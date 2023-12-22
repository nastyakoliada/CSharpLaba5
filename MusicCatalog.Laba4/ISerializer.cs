
namespace MusicCatalog.Laba5;
/// <summary>
/// Интерфейс для сериализации
/// </summary>
public interface ISerializer<T>
{
    /// <summary>
    /// Сериализация объекта
    /// </summary>
    /// <param name="compositions"></param>
    Task Serialize(T compositions);
    /// <summary>
    /// Десериализация объекта
    /// </summary>
    /// <returns></returns>
    Task<T> Deserialize();
}
