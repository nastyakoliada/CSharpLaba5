using Avalonia;
using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using MusicCatalog.UI.ViewModels;
using Avalonia.Interactivity;

namespace MusicCatalog.UI.Views;

public partial class CompositionWindow : ReactiveWindow<CompositionViewModel>
{
    public CompositionWindow()
    {
        InitializeComponent();
        this.WhenActivated(action => {
            action(ViewModel!.SaveNewCompositionCommand.Subscribe(Close));
            
        });
        
    }

    public void OnCancel(object sender, RoutedEventArgs e)
    {
        Close();
    }
}