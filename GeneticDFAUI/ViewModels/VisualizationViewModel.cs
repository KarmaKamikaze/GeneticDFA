using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using DynamicData;
using GeneticDFA;
using GeneticDFAUI.Services;
using GeneticDFAUI.Views;
using ReactiveUI;

namespace GeneticDFAUI.ViewModels;

public class VisualizationViewModel : ViewModelBase
{
    private readonly Peekaboo _watcher;
    private readonly Setup _geneticAlgorithmThread;
    private bool _gaIsRunning;
    private string? _selectedImage;
    private ObservableCollection<string> _generations = new ObservableCollection<string>();
    private Bitmap? _image;

    public VisualizationViewModel(Setup geneticAlgorithmThread)
    {
        _gaIsRunning = true;
        _geneticAlgorithmThread = geneticAlgorithmThread;
        SwitchToSettingsWindow = ReactiveCommand.Create(OnSwitchToSettings);
        StopGa = ReactiveCommand.Create(OnStopGa);

        _watcher = new Peekaboo("./Visualizations/", "*.png");
        _watcher.IncludeSubDirectories = false;
        _watcher.FileCreated += OnGenerationListCreateUpdate;
        _watcher.StartScanning(10000);
    }

    public string? SelectedImage
    {
        get => _selectedImage;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedImage, value);
            LoadImage(value);
        }
    }

    public ObservableCollection<string> Generations
    {
        get => _generations;
        set => this.RaiseAndSetIfChanged(ref _generations, value);
    }

    public Bitmap? Image
    {
        get => _image;
        private set => this.RaiseAndSetIfChanged(ref _image, value);
    }

    public bool GaIsRunning
    {
        get => _gaIsRunning;
        set => this.RaiseAndSetIfChanged(ref _gaIsRunning, value);
    }

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
        GaIsRunning = false;
    }

    private void OnGenerationListCreateUpdate(ObservableCollection<string> fileNames)
    {
        Generations.AddRange(fileNames);
    }

    private void LoadImage(string? fileName)
    {
        string path = $"./Visualizations/{fileName}.png";
        if (!File.Exists(path))
            throw new ArgumentException($"The following path is not valid: {path}");

        using FileStream imageStream = File.OpenRead(path);
        Image = Bitmap.DecodeToWidth(imageStream, 550);
    }
}
