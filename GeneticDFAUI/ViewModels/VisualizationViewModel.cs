using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using GeneticDFA;
using GeneticDFAUI.Views;
using ReactiveUI;

namespace GeneticDFAUI.ViewModels;

public class VisualizationViewModel : ViewModelBase
{
    private readonly FileSystemWatcher _watcher;
    private readonly Setup _geneticAlgorithmThread;
    private List<string> _generations = new List<string>();

    public VisualizationViewModel(Setup geneticAlgorithmThread)
    {
        _geneticAlgorithmThread = geneticAlgorithmThread;
        SwitchToSettingsWindow = ReactiveCommand.Create(OnSwitchToSettings);

        _watcher = new FileSystemWatcher("./Visualizations/");
        _watcher.Filter = "*.svg";
        _watcher.Created += OnGenerationListCreateUpdate;
        _watcher.EnableRaisingEvents = true;
        _watcher.IncludeSubdirectories = false;
    }

    public List<string> Generations
    {
        get => _generations;
        set => this.RaiseAndSetIfChanged(ref _generations, value);
    }

    public Bitmap Image { get; set; }

    public ICommand SwitchToSettingsWindow { get; }

    public void OnSwitchToSettings()
    {
        _geneticAlgorithmThread.Kill();
        var app = (ClassicDesktopStyleApplicationLifetime) Application.Current!.ApplicationLifetime!;
        app.MainWindow.Content = new SettingsView()
        {
            DataContext = new SettingsViewModel(),
        };
    }

    private void OnGenerationListCreateUpdate(object sender, FileSystemEventArgs e)
    {
        Generations.Add(e.Name!);
        var app = (ClassicDesktopStyleApplicationLifetime) Application.Current!.ApplicationLifetime!;
        ListBox? listbox = app.MainWindow.FindControl<ListBox>("VisualizationList");
        listbox.Items = Generations;
        listbox.InvalidateMeasure(); // Queries to reload listbox
    }
}
