using Microsoft.AspNetCore.Mvc;
using Moq;
using MusicCatalog.WebService.Controllers;
using MusicCatalog.WebService.Repositories;
using MusicCatalog.Data;

namespace MusicCatalog.Test.XUnit;

/// <summary>
/// Тестирование работы контроллера <see cref="MusicCatalogController"/>
/// </summary>
public class RestApiTesting
{
    /// <summary>
    /// Делегаты для подмены методов репозитория
    /// </summary>
    private static Func<int,Composition?> singleComposition = SingleComposition;
    private static Func<string, IEnumerable<Composition>> queryCompositions = QueryCompositions;
    private static Func<string, int> queryCompositionsCount = QueryCompositionsCount;

    /// <summary>
    /// Сами методы подменного репозитория
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private static Composition? SingleComposition(int id) => MCompositions().SingleOrDefault(c => c.ID == id);
    private static IEnumerable<Composition> QueryCompositions(string query) =>
        MCompositions().Where(c => (c.Author?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
        || (c.SongName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));
    private static int QueryCompositionsCount(string query) =>QueryCompositions(query).Count();

    private static IEnumerable<Composition> MCompositions()
    {
        var r = new List<Composition>();
        r.Add(new Composition()
        {
            ID = 1,
            Author = "Pol McCartney",
            SongName = "Girl"
        });
        r.Add(new Composition()
        {
            ID = 2,
            Author = "Billie Elish",
            SongName = "The cat",            
        });
        r.Add(new Composition()
        {
            ID = 3,
            Author = "Suzi Quattro",
            SongName = "Drive",            
        });
        return r;
    }
    /// <summary>
    /// Проверка на удаление композиции
    /// </summary>
    [Fact]
    public async void TestDeleteComposition()
    {
        var mocRepository = new Mock<IMusicCatalogRepository>();
        mocRepository.Setup(r => r.RemoveAsync(It.IsAny<string>())).ReturnsAsync(queryCompositionsCount);

        var controller = new MusicCatalogController(mocRepository.Object);

        // Act
        var result =await controller.DeleteCompositions("Suzi");
        
        //Assert
        var model = Assert.IsAssignableFrom<IActionResult>(result);

        var actionResult = Assert.IsType<OkObjectResult>(model);
       

        Assert.Equal(1, actionResult.Value);
    }
    /// <summary>
    /// Проверка на получение списка композиций по запросу
    /// </summary>
    [Fact]
    public async void TestGetCompositionsQuery()
    {
        var mocRepository = new Mock<IMusicCatalogRepository>();
        mocRepository.Setup(r => r.SearchAsync(It.IsAny<string>())).ReturnsAsync(queryCompositions);

        var controller = new MusicCatalogController(mocRepository.Object);

        // Act
        var result = await controller.GetCompositions("Suzi");        
        //Assert
        var model = Assert.IsAssignableFrom<IEnumerable<Composition>>(result);
       
        Assert.Equal(1, model?.Count());
    }

    /// <summary>
    /// Проверка на получение всех композиций
    /// </summary>
    [Fact]
    public async void TestGetCompositions()
    {
        var mocRepository = new Mock<IMusicCatalogRepository>();
        mocRepository.Setup(r => r.EnumerateAllCompositionsAsync()).ReturnsAsync(MCompositions());

        var controller = new MusicCatalogController(mocRepository.Object);

        // Act
        var result = await controller.GetCompositions(null);
        
        //Assert
        var model = Assert.IsAssignableFrom<IEnumerable<Composition>>(result);
     
        Assert.Equal(3,model?.Count());
    }
    /// <summary>
    /// Проверка на плохой ответ BadRequest
    /// </summary>
    [Fact]
    public async void TestGetCompositionBAD() {
        
        var mocRepository = new Mock<IMusicCatalogRepository>();
        mocRepository.Setup(r => r.GetCompositionAsync(It.IsAny<int>())).ReturnsAsync(singleComposition);

        var controller = new MusicCatalogController(mocRepository.Object);
        //act
        var result = await controller.GetComposition(null);
        
        //assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    /// <summary>
    /// Проверка на ответ NotFound
    /// </summary>
    [Fact]
    public async void TestGetCompositionNotFound()
    {
        int id = 0;
        var mocRepository = new Mock<IMusicCatalogRepository>();
        mocRepository.Setup(r => r.GetCompositionAsync(It.IsAny<int>())).ReturnsAsync(singleComposition);

        var controller = new MusicCatalogController(mocRepository.Object);
        //act
        var result = await controller.GetComposition(id);
        
        //assert
        Assert.IsType<NotFoundResult>(result);
    }
    /// <summary>
    /// Проверка на получение композиции по id
    /// </summary>
    [Fact]
    public async void TestGetComposition()
    {
        int id = 2;
        var mocRepository = new Mock<IMusicCatalogRepository>();
        mocRepository.Setup(r => r.GetCompositionAsync(It.IsAny<int>())).ReturnsAsync(singleComposition);

        var controller = new MusicCatalogController(mocRepository.Object);
        //act
        var result = await controller.GetComposition(id);
        
        //assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var composition = Assert.IsAssignableFrom<Composition?>(actionResult.Value);
        Assert.NotNull(composition);
        Assert.Equal(id, composition.ID);
    }
    /// <summary>
    /// Проверка на занесение композиции
    /// </summary>
    [Fact]
    public async void TestAddComposition()
    {
        Composition added = new Composition
        {
            ID = 4,
            Author="Nexxx",
            SongName="Trouble"
        };

        var mocRepository = new Mock<IMusicCatalogRepository>();
        mocRepository.Setup(r => r.AddCompositionAsync(It.IsAny<Composition>()))
            .ReturnsAsync(added);
        var controller = new MusicCatalogController(mocRepository.Object);
        //act
        var result = await controller.StoreComposition(added);
        
        //assert
        var actionResult = Assert.IsType<CreatedAtRouteResult>(result);
        var composition = Assert.IsType<Composition>(actionResult.Value);
        Assert.NotNull(composition);
        Assert.Equal(4, composition.ID);
    }
}
