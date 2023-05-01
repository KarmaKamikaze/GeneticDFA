using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using GeneticDFAUI.Views;
using ReactiveUI;

namespace GeneticDFAUI.ViewModels;

public class VisualizationViewModel : ViewModelBase
{
    public VisualizationViewModel()
    {
        SwitchToSettingsWindow = ReactiveCommand.Create(OnSwitchToSettings);
    }

    public List<string> Generations { get; set; } = new List<string>();

    public Bitmap Image { get; set; }

    public ICommand SwitchToSettingsWindow { get; }
    public void OnSwitchToSettings()
    {
        var app = (ClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        app.MainWindow.Content = new SettingsView()
        {
            DataContext = new SettingsViewModel(),
        };
    }
}
