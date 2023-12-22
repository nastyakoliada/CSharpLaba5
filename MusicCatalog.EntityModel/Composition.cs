using System.ComponentModel.DataAnnotations;

namespace MusicCatalog.Data;
/// <summary>
/// Музакальная композиция из модели данных
/// </summary>
public class Composition
{
    public int ID { get; set; }

    [Required]
    public string? Author { get; set; }
    [Required]
    public string? SongName { get; set; }
}