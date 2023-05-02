using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using GeneticDFA;
using GeneticDFAUI.Views;
using ReactiveUI;

namespace GeneticDFAUI.ViewModels;

public class VisualizationViewModel : ViewModelBase
{
    private readonly FileSystemWatcher _watcher;

    public VisualizationViewModel(Setup geneticAlgorithmThread)
    {
        GeneticAlgorithmThread = geneticAlgorithmThread;
        SwitchToSettingsWindow = ReactiveCommand.Create(OnSwitchToSettings);

        _watcher = new FileSystemWatcher("./Visualizations/");
        _watcher.EnableRaisingEvents = true;
        _watcher.Created += SetGenerationList;
    }
    public Setup GeneticAlgorithmThread { get; }
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

    private void SetGenerationList(object sender, FileSystemEventArgs e)
    {
        Generations.Add(e.Name!);
    }
}
