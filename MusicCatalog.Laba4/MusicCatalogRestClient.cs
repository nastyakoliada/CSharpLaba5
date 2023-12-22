
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MusicCatalog.Laba5;
/// <summary>
/// Класс музакального каталога, который взаимодействует с музыкальным каталогом через веб сервис. Конечная точка
/// указывается в контсрукторе. Путь к контроллеру указан в <see cref="API_PATH"/>
/// </summary>
public class MusicCatalogRestClient : IMusicCatalog,IDisposable
{
    /// <summary>
    /// Путь к контроллеру
    /// </summary>
    private const string API_PATH = "api/musiccatalog/";
    /// <summary>
    /// Клиент для взаимодействия с сервисом
    /// </summary>
    private readonly HttpClient httpClient;
    /// <summary>
    /// Конструктор каталога
    /// </summary>
    /// <param name="uri">Конечная точка сервиса</param>
    public MusicCatalogRestClient(string uri)
    {
        httpClient = new HttpClient
        {
            BaseAddress = new Uri(uri),            
        };
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
    }
    #region IMusicCatalog interface
    /// <summary>
    /// Метод доавляет композицию к перечню
    /// </summary>
    /// <param name="composition">Композиция, которую следует добавить</param>
    public async Task<Composition> AddComposition(Composition composition)
    {
        using var responce = await httpClient.PostAsJsonAsync<Data.Composition>(requestUri: API_PATH,
            value: new Data.Composition
            {
                Author=composition.Author,
                SongName=composition.SongName,
            });        
        var c = await responce.Content.ReadFromJsonAsync<Data.Composition>();        
        if (c is not null) composition.Number = c.ID;
        return composition;
    }
    /// <summary>
    /// Изменяет существующую композицию
    /// </summary>
    /// <param name="composition"></param>
    /// <returns>true - если все хорошо</returns>
    public async Task<bool> Update(Composition composition)
    {
        using var responce = await httpClient.PutAsJsonAsync<Data.Composition>(requestUri: API_PATH,
            value: new Data.Composition
            {
                ID=composition.Number, 
                Author=composition.Author,
                SongName =composition.SongName,
            });        

        return await responce.Content.ReadFromJsonAsync<bool>();        
    }
    /// <summary>
    /// Метод возвращает enumerator для перебора всех композици каталога.
    /// Композиции отсортированы сначала по автору, потом по названию
    /// </summary>
    /// <returns>перечень композиций</returns>
    public async Task<IEnumerable<Composition>> EnumerateAllCompositions()
    {
        using HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: API_PATH);
        
        using HttpResponseMessage responce = await httpClient.SendAsync(request);
        var result = await  responce.Content.ReadFromJsonAsync<IEnumerable<Data.Composition>>();
        

        return  result is null ? Enumerable.Empty<Composition>() :
            result.Select(c => new Composition
            {
                Author = c.Author ?? "",
                SongName = c.SongName ?? "",
                Number = c.ID
            });
    }
    /// <summary>
    /// Метод удаляет из каталога все композиции, которые удовлетворяют условиям поиска
    /// </summary>
    /// <param name="query">Критерий поиска композиций</param>
    /// <returns>Количество удаленных композиций</returns>
    public async Task<int> Remove(string query)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Delete, requestUri: $"{API_PATH}{query}");
        using HttpResponseMessage responce = await httpClient.SendAsync(request);
        return await responce.Content.ReadFromJsonAsync<int>();        
    }
    /// <summary>
    /// Удаляет композицию с указанным id из каталога 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> Delete(int id)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Delete, requestUri: $"{API_PATH}composition/{id}");
        using HttpResponseMessage responce = await httpClient.SendAsync(request);
        return await responce.Content.ReadFromJsonAsync<bool>();
    }
    /// <summary>
    /// Метод возвращает enumerator для перебора композици каталога, который удовлетворяют условиям поиска
    /// Композиции отсортированы сначала по автору, потом по названию
    /// </summary>    
    /// <param name="query">Критерий поиска</param>
    /// <returns>перечень композиций</returns>
    public async Task<IEnumerable<Composition>> Search(string query)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: $"{API_PATH}?query={query}");

        using HttpResponseMessage responce = await httpClient.SendAsync(request);
        var result = await responce.Content.ReadFromJsonAsync<IEnumerable<Data.Composition>>();
        
        return result is null ? Enumerable.Empty<Composition>() :
            result.Select(c => new Composition
            {
                Author = c.Author ?? "",
                SongName = c.SongName ?? "",
                Number = c.ID
            });
    }
    /// <summary>
    /// Возвращает перечень композиций из каталога, которые удовлетворяют заданным условиям поиска
    /// </summary>
    /// <param name="stype">Где искать</param>
    /// <param name="query">Поисковая строка</param>
    /// <returns></returns>
    public async Task<IEnumerable<Composition>> Search(int stype, string query)
    {
        using HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: $"{API_PATH}{stype}/{query}");

        using HttpResponseMessage responce = await httpClient.SendAsync(request);
        var result = await responce.Content.ReadFromJsonAsync<IEnumerable<Data.Composition>>();
        
        return result is null ? Enumerable.Empty<Composition>() :
            result.Select(c => new Composition
            {
                Author = c.Author ?? "",
                SongName = c.SongName ?? "",
                Number = c.ID
            });
    }
    #endregion
    public void Dispose()
    {
        httpClient?.Dispose();
    }
}
