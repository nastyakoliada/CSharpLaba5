using Avalonia.Controls;
using MusicCatalog.UI.ViewModels;
namespace MusicCatalog.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();        
    }

    /// <summary>
    /// При изменении типа музыкального каталога сбрасывает фильтр
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void CatalogTypeChanged(object sender, SelectionChangedEventArgs e)
    {
        if(DataContext is not null)
        {
            MusicCatalogViewModel viewModel = (MusicCatalogViewModel)DataContext;
            viewModel.FilterString=null;
        }
            
    }
}
