using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using GeneticDFA;
using GeneticDFAUI.Services;
using GeneticDFAUI.Views;
using ReactiveUI;

namespace GeneticDFAUI.ViewModels;

public class VisualizationViewModel : ViewModelBase
{
    private readonly Peekaboo _watcher;
    private readonly Setup _geneticAlgorithmThread;
    private List<string> _generations = new List<string>();

    public VisualizationViewModel(Setup geneticAlgorithmThread)
    {
        _geneticAlgorithmThread = geneticAlgorithmThread;
        SwitchToSettingsWindow = ReactiveCommand.Create(OnSwitchToSettings);
        StopGa = ReactiveCommand.Create(OnStopGa);

        _watcher = new Peekaboo("./Visualizations/", "*.svg");
        _watcher.IncludeSubDirectories = false;
        _watcher.FileCreated += OnGenerationListCreateUpdate;
        _watcher.StartScanning(10000);
    }

    public List<string> Generations
    {
        get => _generations;
        set => this.RaiseAndSetIfChanged(ref _generations, value);
    }

    public Bitmap Image { get; set; }

    public ICommand SwitchToSettingsWindow { get; }
    public ICommand StopGa { get; }

    private void OnSwitchToSettings()
    {
        _geneticAlgorithmThread.Kill();
        var app = (ClassicDesktopStyleApplicationLifetime) Application.Current!.ApplicationLifetime!;
        app.MainWindow.Content = new SettingsView()
        {
            DataContext = new SettingsViewModel(),
        };
    }

    private void OnStopGa()
    {
        _geneticAlgorithmThread.Kill();
    }

    private void OnGenerationListCreateUpdate(List<string> fileNames)
    {
        Generations = fileNames;
    }
}
