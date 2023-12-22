using Avalonia.ReactiveUI;
using ReactiveUI;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MusicCatalog.UI.ViewModels;
using System.Threading.Tasks;
using System.Reactive;
using MusicCatalog.Laba5;

namespace MusicCatalog.UI.Views;

/// <summary>
/// View музыкального каталога
/// </summary>
public partial class MainView : ReactiveUserControl<MusicCatalogViewModel>
{
    public MainView()
    {
        InitializeComponent();
        //Регистрируем обработчики взаимодействий для открытия дилогового окна редактирования композиции
        this.WhenActivated(action => {
            //Для добавления новой композиции
            action(ViewModel!.ShowAddDialog.RegisterHandler(DoGetCompositionAsync));
            // Для редктирования существующей композиции
            action(ViewModel!.ShowEditDialog.RegisterHandler(DoGetCompositionAsync));
        });
        
    }
    
    /// <summary>
    /// Обработчик взаимоджействий по добавлению новой композиции или редактированию существующей
    /// </summary>
    /// <param name="interaction">Задействованное Interaction</param>
    /// <returns></returns>
    private async Task DoGetCompositionAsync(InteractionContext<CompositionViewModel,
                                        CompositionViewModel?> interaction)
    {
        //Если компонент еще не полностью готов, то ничего не делаем
        if (!IsLoaded) return;
        //Создаем диалоговое окно
        var dialog = new CompositionWindow();
        //Указываем его DataContext
        dialog.DataContext = interaction.Input;
        // Показываем окно и получаем ответ (CompositionViewModel)
        var result = await dialog.ShowDialog<CompositionViewModel?>((Window)Window.GetTopLevel(this)!);
        // Записываем этот ответ во взаимодействие, как out[ut
        interaction.SetOutput(result);
    }

       
    /// <summary>
    /// Обработка установки фильтра. Так как у flyout не оказалось DataContext, то пришлось обработать сдесь.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnFilterSet(object? sender, RoutedEventArgs e)
    {
        
        btnFilter.Flyout!.Hide();
        if (DataContext is not null)
        {
            MusicCatalogViewModel mcm = (MusicCatalogViewModel)DataContext;
            await mcm.ReadCatalogWithFilter();
        }
    }
}
