using static System.Console;

namespace MusicCatalog.Laba5;
/// <summary>
/// Класс представляет собой взаимеодействие с пользователем по работе с музыкальным каталогом
/// через консоль
/// 
/// Пользователь должен указать тип сериализации и имя каталога.
/// После этого сериализация происходит в файл с именем "Имя каталога".xml или .json.
/// Если указан тип сериализации SQLite, то сериализация происходит в файл "Имя каталога".db.
/// Для создания файла "Имя каталога".db используется шаблон базы данных файл sqlite_template.db, который
/// должен находиться в папке приложения. Проект содержит этот шаблон.
/// Файлы для сериализации располагаются в папке ApplicationData текущего пользователя
/// Если указан тип сериализации WebService, то происходит сериаллизация в <see cref="WEB_SERVICE_URI"/> 
/// </summary>
public class MusicCatalogCommander
{
    /// <summary>
    /// Имя папки, которая будет создана в ApplicationData папке для хранения файлов музыкального кталога
    /// </summary>
    private const string MUSIC_CATALOG_DIRECTORY = "MusicCatalog";
    private const string WEB_SERVICE_URI = "https://localhost:5010/";

    /// <summary>
    /// Показывает справку по работе с программой
    /// </summary>
    public static void ShowNotes()
    {
        WriteLine("""
            Программа Музыкальный каталог.
            Для работы с программой используйте следующие команды:
                Add - добавляет композицию в каталог;
                List - выводит в консоль все композиции из каталога;
                Search - выводит в консоль композиции, удовлетворяющие критерию поиска;
                Remove - удаляет из каталога композиции, удовлетворяющие критерию поиска;
                Quit - завершает работу с каталогом.
            """);

    }
    /// <summary>
    /// Метод запускает работу
    /// </summary>
    public static void Run()
    {
        ShowNotes();
        
        //Создаем каталог и передаем его коммандеру
        MusicCatalogCommander commander = new (CreateMusicCatalog());

        commander.CommandsLoop();
    }
    /// <summary>
    /// Цикл опроса данных от консоли
    /// </summary>
    public void CommandsLoop()
    {
        //Цикл работы с коммандером
        while (!IsRedyToExit)
        {
            try
            {
                WriteLine("\nВведите команду: ");

                if (Commands.TryGetValue(
                    (ReadLine() ?? "").ToUpper(),
                    out Func<Task>? action))
                {
                    action().Wait();
                }
                else
                {
                    WriteLine("Введена неверная команда. Попробуйте снова.");
                }
            }
            catch (Exception e)
            {
                WriteLine("\nОшибка !");
                WriteLine(e.Message);
            }
        }

        if(catalog is not null && catalog is IDisposable)
        {
            ((IDisposable)catalog)?.Dispose();
        }
    }
    
    /// <summary>
    /// Экземпляр музыкального каталога
    /// </summary>
    private  IMusicCatalog catalog = null!;
    /// <summary>
    /// Сопоставление команд пользователя методам класса
    /// </summary>
    private readonly Dictionary<string, Func<Task>> Commands = new Dictionary<string, Func<Task>>();
    /// <summary>
    /// Конструктор по умолчанию. Заполняет сопоставление команд методам класса
    /// </summary>
    public MusicCatalogCommander(IMusicCatalog catalog)
    {
        Commands.Add("ADD", Add);
        Commands.Add("LIST", List);
        Commands.Add("REMOVE", Remove);
        Commands.Add("SEARCH", Search);
        Commands.Add("QUIT", Quit);
        this.catalog = catalog;
    }
    /// <summary>
    /// Возвращает запрашиваемую строку у пользователя
    /// </summary>
    /// <param name="question">Текст того, что пользователь должен ввести</param>
    /// <returns>Введенная пользователем строка</returns>
    private static string ReadString(string question)
    {
        string line;
        do
        {
            WriteLine(question);
        }
        while (string.IsNullOrEmpty(line = ReadLine()!));
        return line;

    }
    #region Методы - команды пользователя
    /// <summary>
    /// Выполнение команды пользователя по занесении композиции в каталог
    /// </summary>
    public async Task Add()
    {

        await catalog.AddComposition(
            new Composition
            {
                Author = ReadString("Имя автора:"),
                SongName = ReadString("Название композиции:"),
            });
    }

    /// <summary>
    /// Выполняет команду вывода полного содержимого каталога
    /// </summary>
    public async Task List()
    {
        PrintSongs("\nСписок всех песен:",await catalog.EnumerateAllCompositions());
    }
    /// <summary>
    /// Метод выводит на консоль перечень композиций
    /// </summary>
    /// <param name="header">Заголовок перечня композиций</param>
    /// <param name="songs">Enumerator для перебора композиций</param>
    private static void PrintSongs(string header,IEnumerable<Composition> songs)
    {
        WriteLine(header);
        foreach (var comp in songs)
        {
            WriteLine($"{comp.Author} - {comp.SongName}");
        }
    }
    /// <summary>
    /// Выполняет команду пользователя на удаление композиций из каталога, удовлетворяющих
    /// заданному критерию поиска
    /// </summary>
    public async Task Remove()
    {
        int res = await catalog.Remove(ReadString("Что удаляем?:"));
        WriteLine($"Удалено {res} песен.");
    }
    /// <summary>
    ///  Выполняет команду пользователя на вывод на консоль перечня композиций, удовлетворяющих
    ///  заданному критерию поиска
    /// </summary>
    public async Task Search()
    {
        PrintSongs("\nРезультат поиска:", await catalog.Search(ReadString("Что ищем ?:")));
    }
    /// <summary>
    /// Выполняет команду пользователя о завершении работы
    /// </summary>
    public  Task Quit()
    {
        IsRedyToExit = true;
        return Task.CompletedTask;
    }
    private bool IsRedyToExit {get; set;}
    #endregion

    #region Методы для создания папки хранения каталога, имения файла музыкального каталога
    /// <summary>
    /// Создает класс для работы с музыкальным каталогам по указанным параметрам
    /// </summary>
    /// <returns></returns>
    private static IMusicCatalog CreateMusicCatalog()
    {
        //Запросим тип каталога
        int type = 0;
        do
        {
            int.TryParse(ReadString("Укажите тип каталога (1-xml, 2-json,3-SQLite,4-WebService)"), out type);
        } while (type < 1 || type > 4);

        //Запросим имя каталога
        string catalogName = string.Empty;
        if (type < 4) {
            catalogName = ReadString("\nУкажите имя музыкального каталога?");

            string mcPath = MusicCatalogPath;

            if (!Path.Exists(mcPath)) Directory.CreateDirectory(mcPath);
        }

        return type switch
        {
            1 => new MusicCatalog(new MCSerializerXml(MusicCatalogFullName(catalogName+".xml"))),
            2 => new MusicCatalog(new MCSerializerJSon(MusicCatalogFullName(catalogName+".json"))),
            3 => new MusicCatalogSQLite(MusicCatalogFullName(catalogName+".db")),
            _ => new MusicCatalogRestClient(WEB_SERVICE_URI)
        };
    }
    /// <summary>
    /// Возвращает полное имя файла музыкального каталога
    /// </summary>
    /// <param name="fileName">Имя файла без пути</param>
    /// <returns>Полное имя файла</returns>
    public static string MusicCatalogFullName(string fileName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    MUSIC_CATALOG_DIRECTORY, fileName);
    }

    /// <summary>
    /// Папка, в которой будет сохраняться музыкальный каталог
    /// </summary>
    private static string MusicCatalogPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                MUSIC_CATALOG_DIRECTORY);
    #endregion

}

