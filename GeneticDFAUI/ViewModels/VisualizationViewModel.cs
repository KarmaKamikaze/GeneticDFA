using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using GeneticDFA;
using GeneticDFAUI.Views;
using ReactiveUI;

namespace GeneticDFAUI.ViewModels;

public class VisualizationViewModel : ViewModelBase
{
    public VisualizationViewModel(Setup geneticAlgorithmThread)
    {
        GeneticAlgorithmThread = geneticAlgorithmThread;
        SwitchToSettingsWindow = ReactiveCommand.Create(OnSwitchToSettings);
    }

    public Setup GeneticAlgorithmThread { get; set; }
    public List<string> Generations { get; set; } = new List<string>();

    public Bitmap Image { get; set; }

    public ICommand SwitchToSettingsWindow { get; }
    public void OnSwitchToSettings()
    {
        GeneticAlgorithmThread.Kill();
        var app = (ClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        app.MainWindow.Content = new SettingsView()
        {
            DataContext = new SettingsViewModel(),
        };
    }
}
