using System.Reactive;
using ReactiveUI;

namespace MusicCatalog.UI.ViewModels;
/// <summary>
/// View model окна редактирования композиции
/// </summary>
public class CompositionViewModel : ViewModelBase
{
    /// <summary>
    /// Номер композиции
    /// </summary>
    private int? _number;
    /// <summary>
    /// Автор композиции
    /// </summary>
    private string? _author;
    /// <summary>
    /// Название композиции
    /// </summary>
    private string? _songName;

    /// <summary>
    /// Действие при нажатии клавиши Ввод
    /// </summary>
    public ReactiveCommand<Unit, CompositionViewModel> SaveNewCompositionCommand { get; }
    /// <summary>
    /// Действие при нажатии клавиши Отмена
    /// </summary>
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    /// <summary>
    /// Конструктор, в котором опрпеделяются действия при нажатии клавиш
    /// </summary>
    public CompositionViewModel()
    {
        //При нажатии Ввод возвращается отредактированная композиция
        SaveNewCompositionCommand = ReactiveCommand.Create(() =>
        {
            return this;
        });

        //При нажатии Отмена ничего не возвращается
        CancelCommand = ReactiveCommand.Create(() => { });
    }
    /// <summary>
    /// Свойство номер композиции для связи с View
    /// </summary>
    public int? Number
    {
        get => _number;
        set => this.RaiseAndSetIfChanged(ref _number, value);
    }
    /// <summary>
    /// Свойство Автор для связи с View
    /// </summary>
    public string? Author
    {
        get => _author;
        set => this.RaiseAndSetIfChanged(ref _author, value);
    }

    /// <summary>
    /// Свойство название композиции для связи с view
    /// </summary>

    public string? SongName
    {
        get => _songName;
        set => this.RaiseAndSetIfChanged(ref _songName, value);
    }
    /// <summary>
    /// Метод копирует одну композицию в другую
    /// </summary>
    /// <param name="src"></param>
    /// <param name="dest"></param>
    public static void Copy(CompositionViewModel src, CompositionViewModel dest)
    {
        dest.Number = src.Number;
        dest.Author = src.Author;
        dest.SongName = src.SongName;
    }    
}
