using Microsoft.AspNetCore.Mvc;
using MusicCatalog.Data; // Composition
using MusicCatalog.WebService.Repositories; // IMusicCatalogRepository

namespace MusicCatalog.WebService.Controllers;
/// <summary>
/// Контроллер для музыкального каталога
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MusicCatalogController : ControllerBase
{
    /// <summary>
    ///  Репозиторий, содержащий методы для выполнения действий с музыкальным каталогом
    ///  Репозиторий работает с dbContext
    /// </summary>
    private readonly IMusicCatalogRepository musicCatalogRepository;
    /// <summary>
    /// Конструктор, в который окружение предает репозиторий
    /// </summary>
    /// <param name="injectedRepository"></param>
    public MusicCatalogController(IMusicCatalogRepository injectedRepository)
    {
        musicCatalogRepository=injectedRepository;
    }

    // GET: api/musiccatalog
    // GET: api/musiccatalog/?query=[query]
    /// <summary>
    /// Всегда возвращает список композиций, может быть пустым
    /// </summary>
    /// <param name="query">Запрос композиций, может отсутствовать</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Composition>))]
    public async Task<IEnumerable<Composition>> GetCompositions(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await musicCatalogRepository.EnumerateAllCompositionsAsync();
        }
        else
        {
            return await musicCatalogRepository.SearchAsync(query);
        }
    }

    // GET: api/musiccatalog/[id]
    /// <summary>
    /// Возвращает композицию с указанным id
    /// </summary>
    /// <param name="id">Уникальный идентификатор композиции</param>
    /// <returns>Найденную композицию <see cref="Composition"/></returns>
    [HttpGet("{id}",Name = nameof(GetComposition))]
    [ProducesResponseType(201, Type = typeof(Composition))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetComposition(int? id)
    {
        if (!id.HasValue) return BadRequest("You must pass the composition id in the query string, " +
            "for example api/compositions/23");
        Composition? c = await musicCatalogRepository.GetCompositionAsync(id);
        if (c is null) return NotFound();

        return Ok(c);   // 200 с композицией в теле
    }
    //GET: api/musiccatalog/[stype]/[query]
    /// <summary>
    /// Возвращет перечень композиий, удовбелвторяющих запросу
    /// </summary>
    /// <param name="stype">Тип запроса, 1 - по автору, 2 - по названию</param>
    /// <param name="query">запрос</param>
    /// <returns></returns>
    [HttpGet("{stype}/{query}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Composition>))]
    [ProducesResponseType(400)]
    public async Task<IEnumerable<Composition>> GetCompositions(int? stype, string? query)
    {
        if (stype is null || query is null) return Enumerable.Empty<Composition>();
        return await musicCatalogRepository.SearchAsync(stype.Value, query);
    }

    //POST: api/musiccatalog
    //BODY: Composition(JSON)
    /// <summary>
    /// Заносит композицию в каталог
    /// </summary>
    /// <param name="c"><see cref="Composition"/> для занесения</param>
    /// <returns>Занесенная <see cref="Composition"/> с установленным id</returns>
    [HttpPost]
    [ProducesResponseType(201,Type=typeof(Composition))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> StoreComposition([FromBody] Composition c)
    {
        if (c is null) return BadRequest(); // Некорректный запрос
        
        Composition? added =  await musicCatalogRepository.AddCompositionAsync(c);
        if (added is null) return BadRequest("Repositiory failed to store composition");

        return CreatedAtRoute(
            routeName: nameof(GetComposition),
            routeValues: new { id = added.ID },
            value: added);
    }
    //PUT: api/musiccatalog
    //BODY: Composition(JSON)
    /// <summary>
    /// Метод изменяет существующую композицию в каталоге
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(201, Type = typeof(bool))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateComposition([FromBody] Composition c)
    {
        if (c is null) return BadRequest(); // Некорректный запрос
        bool result = await musicCatalogRepository.UpdateCompositionAsync(c);
        return result ? Ok(result) : NotFound();
    }


    //DELETE: api/musiccatalog/query
    /// <summary>
    /// Удаляет композиции, удовлетворяющие запросу, из каталога
    /// </summary>
    /// <param name="query">Запрос, например имя автора или название песни</param>
    /// <returns>Количество удаленных композиций</returns>
    [HttpDelete("{query}")]
    [ProducesResponseType(201,Type = typeof(int))] 
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteCompositions(string? query)
    {
        if(string.IsNullOrWhiteSpace(query)) return BadRequest("You must pass query string for deleting, " +
            "for example - api/compositions/song");

        int deleted = await musicCatalogRepository.RemoveAsync(query);

        return Ok(deleted); // 201 со счетчиком удаленных композиций в теле
    }

    //DELETE: api/musiccatalog/composition/id
    /// <summary>
    /// Удаляет из каталога композицию с указанным id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("composition/{id}")]
    [ProducesResponseType(201, Type = typeof(int))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteComposition(int? id)
    {
        if (id == null) return BadRequest();
        return await musicCatalogRepository.DeleteCompositionAync(id.Value)?Ok(true):NotFound();
    }
}
