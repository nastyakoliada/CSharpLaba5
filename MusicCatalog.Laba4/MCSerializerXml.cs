using System.Xml.Serialization;

namespace MusicCatalog.Laba5;
/// <summary>
/// Сериализатор xml 
/// </summary>
public class MCSerializerXml : ISerializer<IEnumerable<Composition>>
{
    /// <summary>
    /// Полное имя файла для сериализации
    /// </summary>
    private readonly string fileName;
    public MCSerializerXml(string fileName)
    {
        this.fileName = fileName;
    }
    /// <summary>
    /// Десериализация списка композиций
    /// </summary>
    /// <returns>Список композиций</returns>
    public async Task<IEnumerable<Composition>> Deserialize()
    {
        return await Task.Run(() =>
        {
            if (string.IsNullOrEmpty(fileName)) return null!;
            if (!File.Exists(fileName)) return null!;

            XmlSerializer xs = new(typeof(List<Composition>));

            using (FileStream file = File.OpenRead(fileName))
            {
                return ((List<Composition>)xs.Deserialize(file)!) ?? new List<Composition>();
            }
        });

    }
    /// <summary>
    /// Сериализация списка композиций
    /// </summary>
    /// <param name="compositions">Список композиций</param>
    public async Task Serialize(IEnumerable<Composition> compositions)
    {
        await Task.Run(() =>
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Composition>));
            using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                xs.Serialize(sw, compositions);
            }
        });
    }
}
