using Avalonia.Controls;
using System.Collections.ObjectModel;
using MusicCatalog.Laba5;
using ReactiveUI;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace MusicCatalog.UI.ViewModels;

/// <summary>
/// ViewModel для View работы с композициями
/// </summary>
public class MusicCatalogViewModel: ViewModelBase
{
    /// <summary>
    /// Композиции, с которыми работает View
    /// </summary>
    private ObservableCollection<CompositionViewModel> Compositions { get; } = new();
    /// <summary>
    /// Текущая отмеченная композиция
    /// </summary>
    private CompositionViewModel? _selectedComposition;
    /// <summary>
    /// Свойство для установки и получения отмеченной композиции
    /// </summary>
    public CompositionViewModel? SelectedComposition
    {
        get => _selectedComposition;
        set => this.RaiseAndSetIfChanged(ref _selectedComposition, value);
    }
    /// <summary>
    /// Взаимодействие для получения новой композиции
    /// </summary>
    public Interaction<CompositionViewModel, CompositionViewModel?> ShowAddDialog { get; }
    /// <summary>
    /// Взаимодействие для редкатирования композиции
    /// </summary>
    public Interaction<CompositionViewModel, CompositionViewModel?> ShowEditDialog { get; }
    /// <summary>
    /// Командf от View - добавить композицию
    /// </summary>
    public ICommand AddCompositionCommand { get; }
    /// <summary>
    /// Редактировать композицию
    /// </summary>
    public ICommand EditCompositionCommand { get; }
    /// <summary>
    /// Удалить композицию
    /// </summary>
    public ICommand DeleteCompositionCommand { get; }
    /// <summary>
    /// Присоединится к каталогу
    /// </summary>
    public ICommand ConnectTo { get; }
    /// <summary>
    /// Реактивная команда - применить фильтр
    /// </summary>
    public ReactiveCommand<Unit,Unit> ApplyFilter { get; }

    /// <summary>
    /// Команда вызывается при закрытии окна
    /// </summary>
    public ReactiveCommand<Unit, Unit> AppClosing { get; }

    /// <summary>
    /// Выбранный тип каталога
    /// </summary>
    public ComboBoxItem? CatalogType { get; set; }
    /// <summary>
    /// Указанное имя катлога или строка подключения к веб сервису
    /// </summary>
    public string? CatalogName { get; set; }
    
    /// <summary>
    /// Выбранный тип фильтра
    /// </summary>
    private ComboBoxItem? _selectedFilterType;
    /// <summary>
    /// Свойство для привязки к выбранному фильтру
    /// </summary>
    public ComboBoxItem? SelectedFilterType
    {
        get => _selectedFilterType;
        set => this.RaiseAndSetIfChanged(ref _selectedFilterType, value);
    }
    private string? _filterString;
    /// <summary>
    /// Строка фильтра
    /// </summary>
    public string? FilterString { 
        get => _filterString; 
        set => this.RaiseAndSetIfChanged(ref _filterString,value); }

    /// <summary>
    /// Конструктор
    /// </summary>
    public MusicCatalogViewModel() {

        
        ShowAddDialog = new Interaction<CompositionViewModel, CompositionViewModel?>();
        ShowEditDialog = new Interaction<CompositionViewModel, CompositionViewModel?>();
        //Определяем действия для команд
        AddCompositionCommand = ReactiveCommand.CreateFromTask(async () =>
        {                        
            //Передаем диалогу пустую композицию для заполнения
            var result = await ShowAddDialog.Handle(new CompositionViewModel());
            // Если есть реультат, то заносим его в каталог
            if(result is not null) await SaveNewComposition(result);
        });

        EditCompositionCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            // Если есть отмеченная композиция, то
            if (SelectedComposition is not null)
            {
                //Создаем копию текущей композиции для ее редактирования
                var toEdit = new CompositionViewModel();           
                
                CompositionViewModel.Copy(SelectedComposition!, toEdit);
                var result = await ShowAddDialog.Handle(toEdit!);
                //Если получили результат, то сохраняем его в редактироуемую композицию и в каталог
                if (result is not null)
                {
                    await SaveChangedCompositin(result,SelectedComposition!);
                    
                }
            }
        });
        //Направляем соответствующие команды на соответствующие методы
        DeleteCompositionCommand = ReactiveCommand.CreateFromTask(DeleteSelectedComposition);
        ConnectTo = ReactiveCommand.CreateFromTask(ConnectToCatalog);
        ApplyFilter = ReactiveCommand.CreateFromTask(ReadCatalogWithFilter);
        AppClosing = ReactiveCommand.Create(OnAppClosing);
    }
    /// <summary>
    /// Подключенный каталог
    /// </summary>
    private IMusicCatalog? _connectedMusicCatalog;
    /// <summary>
    /// Свойство для получения и сохранения каталога
    /// </summary>
    public IMusicCatalog? ConnectedMusicCatalog { 
        get => _connectedMusicCatalog;
        set {
            //Освободим ресурсы старого каталога при смене
            if (_connectedMusicCatalog is not null && 
                _connectedMusicCatalog != value && 
                _connectedMusicCatalog is IDisposable) ((IDisposable)_connectedMusicCatalog).Dispose();

            this.RaiseAndSetIfChanged(ref _connectedMusicCatalog, value); 
        }
    }
    /// <summary>
    /// Читает каталог с применением фильтра
    /// </summary>
    /// <returns></returns>
    public async Task ReadCatalogWithFilter()
    {
        //Получаем тип фильтра
        int stype = Convert.ToInt32(SelectedFilterType?.DataContext ?? 0);
        //Если без фильтра, то просто читаем весь каталог
        if (stype == 0)await ReadCatalog();
        //Иначе вызываем метод чтения с применением фильтра
        else await ReadCatalog(async mc => await mc.Search(stype, FilterString!));
    }
    /// <summary>
    /// Метод подключения нужного каталога
    /// Варианты
    /// <para>
    /// <list type="bullet">
    /// <item> WebService: подключение к веб сервису. В решении нужно указать адрес https://localhost:5010</item>
    /// <item> SQLite: к бд SQLite. Нужно указать имя каталога. в папке AppData\Roaming будет создан файл с расширением db, если его там нет</item>
    /// <item> Файл XML: в файле xml, если файла нет, то он будет создан в папке AppData\Roaming</item>
    /// <item> Файл JSON: аналогично xml</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <returns></returns>
    private async Task ConnectToCatalog()
    {
        ConnectedMusicCatalog = CatalogType!.Content!.ToString() switch
        {
            "WebService" => new MusicCatalogRestClient(CatalogName!),
            "SQLite" => new MusicCatalogSQLite(MusicCatalogCommander.MusicCatalogFullName(CatalogName+".db")),
            "Файл XML" => new Laba5.MusicCatalog(new MCSerializerXml(MusicCatalogCommander.MusicCatalogFullName(CatalogName + ".xml"))),
            _ => new Laba5.MusicCatalog(new MCSerializerJSon(MusicCatalogCommander.MusicCatalogFullName(CatalogName + ".json"))),
        };
        await ReadCatalog();
    }
    /// <summary>
    /// Метод чтения из каталога. В качестве параметра указывается делега для чтения. Если делегат не указан. То читается весь каталог.
    /// Разные делегаты используются для различной фильтрации
    /// </summary>
    /// <param name="getFunction"></param>
    /// <returns></returns>
    private async Task ReadCatalog(Func<IMusicCatalog,Task<IEnumerable<Composition>>> getFunction = null!)
    {
        //Очищаем текущие песни в списке
        Compositions.Clear();
        if (ConnectedMusicCatalog is null) return;
        //Если нет делегата, что устаналвиаем его на чтение всеех песен
        getFunction ??= async mc => await mc.EnumerateAllCompositions();

        foreach (var catalog in await getFunction(ConnectedMusicCatalog!)) {
            Compositions.Add(new CompositionViewModel
            {
                Number = catalog.Number,
                Author= catalog.Author,
                SongName= catalog.SongName,
            });
        }
    }

    /// <summary>
    /// Метод удаляем текущую композицию
    /// </summary>
    /// <returns></returns>
    private async Task DeleteSelectedComposition()
    {
        //Если есть текущая композиция
        if (SelectedComposition is not null)
        {
            //Вызываем удаление композиции с номером, Если вдруг номера нет, то передаем на удаление -1, что вызовет удаление 0 композиций
            // и возврат false
            if(await ConnectedMusicCatalog!.Delete(SelectedComposition.Number ?? -1))
            {
                // Если композиция удалилась, то убираем ее из отображаемого списка
                Compositions.Remove(SelectedComposition!);
            }
        }
    }
    /// <summary>
    /// Метод сохраняет измененную композицию
    /// </summary>
    /// <param name="edited">ОТреактированный вариант композиции</param>
    /// <param name="toUpdate">Оригинальный вариант до редактирования</param>
    /// <returns></returns>
    private async Task SaveChangedCompositin(CompositionViewModel edited,  CompositionViewModel toUpdate)
    {
        // Пробуем сохранить изменения
        var result = await ConnectedMusicCatalog!.Update(new Composition
        {
            Author = edited.Author!,
            SongName = edited.SongName!,
            Number = edited.Number ?? 1,
        })!;        
        //Если удалось, то меняем оригинал
        if(result) CompositionViewModel.Copy(edited, toUpdate);

    }
    /// <summary>
    /// Метод сохраняет новую композицию
    /// </summary>
    /// <param name="newC">Композиция для сохранения</param>
    /// <returns></returns>
    private async Task SaveNewComposition(CompositionViewModel newC)
    {
        //Вызываем сохранения композиции, в ответ получаем сохраненную композицию
        var stored = await ConnectedMusicCatalog!.AddComposition(new Composition
        {
            Author = newC.Author!,
            SongName = newC.SongName!,
        })! ;
        // Если получили сохраненную композицию, то запомним ее номер, который был назначен при занесении в каталог
        newC.Number=stored?.Number;
        //Добавляем композицию в отображаемый список
        Compositions.Add(newC);
    }
    /// <summary>
    /// При закрытии главного окна, если есть Disposable каталог, то очищаем его
    /// </summary>
    private void OnAppClosing()
    {        
        if (_connectedMusicCatalog is not null && _connectedMusicCatalog is IDisposable) 
            ((IDisposable)_connectedMusicCatalog).Dispose();
    }

}
