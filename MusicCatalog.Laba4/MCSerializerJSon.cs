using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MusicCatalog.Laba5;
/// <summary>
/// Сериализатор JSon
/// </summary>
public class MCSerializerJSon : ISerializer<IEnumerable<Composition>>
{
    /// <summary>
    ///  Полное имя файла для сериализации
    /// </summary>
    private readonly string fileName;
    /// <summary>
    /// Параметризованный конструктор
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    public MCSerializerJSon(string fileName)
    {
        this.fileName=fileName;
    }
    /// <summary>
    /// Десериализация списка композиций
    /// </summary>
    /// <returns>Список композиций</returns>
    public async Task<IEnumerable<Composition>> Deserialize()
    {
        if (string.IsNullOrEmpty(fileName)) return null!;
        if (!File.Exists(fileName)) return null!;

        using (FileStream file = File.OpenRead(fileName))
        {
            return await JsonSerializer.DeserializeAsync<IEnumerable<Composition>>(file,
                new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
                })!
                ?? new List<Composition>();

        }
    }
    /// <summary>
    /// Сериализация списка композиций
    /// </summary>
    /// <param name="compositions">Список композиций</param>

    public async Task Serialize(IEnumerable<Composition> compositions)
    {
        using (FileStream file = File.Create(fileName))
        {
            await JsonSerializer.SerializeAsync(file, compositions, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true,
            }); ;
        }
    }
}
