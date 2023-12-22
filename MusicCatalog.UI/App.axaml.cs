using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using MusicCatalog.UI.ViewModels;
using MusicCatalog.UI.Views;

namespace MusicCatalog.UI;
/// <summary>
/// Класс прилодения
/// </summary>
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    /// <summary>
    /// Устанавливаем DataContext MusicCatalogViewModel
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
           
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MusicCatalogViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MusicCatalogViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
