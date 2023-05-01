using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using GeneticDFA.Utility;
using GeneticDFAUI.Views;
using ReactiveUI;

namespace GeneticDFAUI.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private int _minPopulation = 10000;
    private int _maxPopulation = 10000;
    private int _convergenceGenerationNumber = 30;
    private int _maximumGenerationNumber = 100;
    private int _eliteSelectionScalingFactor = 2;
    private int _weightTruePositive = 10;
    private int _weightTrueNegative = 10;
    private double _weightFalsePositive = 10;
    private double _weightFalseNegative = 10;
    private double _weightNonDeterministicEdges = 0.5;
    private double _weightMissingDeterministicEdges = 0.5;
    private double _weightSize = 0.5;
    private double _mutationProbability = 0.5;
    private double _nonDeterministicBehaviorProbability = 0.5;
    private double _changeTargetProbability = 0.1;
    private double _changeSourceProbability = 0.1;
    private double _changeInputProbability = 0.1;
    private double _removeEdgeProbability = 0.1;
    private double _addEdgeProbability = 0.2;
    private double _addStateProbability = 0.1;
    private double _addAcceptStateProbability = 0.1;
    private double _removeAcceptStateProbability = 0.1;
    private double _mergeStatesProbability = 0.1;

    public SettingsViewModel()
    {
        _settingsService = new SettingsService();
        LoadSettings();
        RunCommand = ReactiveCommand.Create(OnRunAlgorithm);
        ResetCommand = ReactiveCommand.Create(OnReset);
    }

    public int MinPopulation
    {
        get => _minPopulation;
        set => this.RaiseAndSetIfChanged(ref _minPopulation, value);
    }

    public int MaxPopulation
    {
        get => _maxPopulation;
        set => this.RaiseAndSetIfChanged(ref _maxPopulation, value);
    }

    public int ConvergenceGenerationNumber
    {
        get => _convergenceGenerationNumber;
        set => this.RaiseAndSetIfChanged(ref _convergenceGenerationNumber, value);
    }

    public int MaximumGenerationNumber
    {
        get => _maximumGenerationNumber;
        set => this.RaiseAndSetIfChanged(ref _maximumGenerationNumber, value);
    }

    public int EliteSelectionScalingFactor
    {
        get => _eliteSelectionScalingFactor;
        set => this.RaiseAndSetIfChanged(ref _eliteSelectionScalingFactor, value);
    }

    public int WeightTruePositive
    {
        get => _weightTruePositive;
        set => this.RaiseAndSetIfChanged(ref _weightTruePositive, value);
    }

    public int WeightTrueNegative
    {
        get => _weightTrueNegative;
        set => this.RaiseAndSetIfChanged(ref _weightTrueNegative, value);
    }

    public double WeightFalsePositive
    {
        get => _weightFalsePositive;
        set => this.RaiseAndSetIfChanged(ref _weightFalsePositive, value);
    }

    public double WeightFalseNegative
    {
        get => _weightFalseNegative;
        set => this.RaiseAndSetIfChanged(ref _weightFalseNegative, value);
    }

    public double WeightNonDeterministicEdges
    {
        get => _weightNonDeterministicEdges;
        set => this.RaiseAndSetIfChanged(ref _weightNonDeterministicEdges, value);
    }

    public double WeightMissingDeterministicEdges
    {
        get => _weightMissingDeterministicEdges;
        set => this.RaiseAndSetIfChanged(ref _weightMissingDeterministicEdges, value);
    }

    public double WeightSize
    {
        get => _weightSize;
        set => this.RaiseAndSetIfChanged(ref _weightSize, value);
    }

    public double MutationProbability
    {
        get => _mutationProbability;
        set => this.RaiseAndSetIfChanged(ref _mutationProbability, value);
    }

    public double NonDeterministicBehaviorProbability
    {
        get => _nonDeterministicBehaviorProbability;
        set => this.RaiseAndSetIfChanged(ref _nonDeterministicBehaviorProbability, value);
    }

    public double ChangeTargetProbability
    {
        get => _changeTargetProbability;
        set => this.RaiseAndSetIfChanged(ref _changeTargetProbability, value);
    }

    public double ChangeSourceProbability
    {
        get => _changeSourceProbability;
        set => this.RaiseAndSetIfChanged(ref _changeSourceProbability, value);
    }

    public double ChangeInputProbability
    {
        get => _changeInputProbability;
        set => this.RaiseAndSetIfChanged(ref _changeInputProbability, value);
    }

    public double RemoveEdgeProbability
    {
        get => _removeEdgeProbability;
        set => this.RaiseAndSetIfChanged(ref _removeEdgeProbability, value);
    }

    public double AddEdgeProbability
    {
        get => _addEdgeProbability;
        set => this.RaiseAndSetIfChanged(ref _addEdgeProbability, value);
    }

    public double AddStateProbability
    {
        get => _addStateProbability;
        set => this.RaiseAndSetIfChanged(ref _addStateProbability, value);
    }

    public double AddAcceptStateProbability
    {
        get => _addAcceptStateProbability;
        set => this.RaiseAndSetIfChanged(ref _addAcceptStateProbability, value);
    }

    public double RemoveAcceptStateProbability
    {
        get => _removeAcceptStateProbability;
        set => this.RaiseAndSetIfChanged(ref _removeAcceptStateProbability, value);
    }

    public double MergeStatesProbability
    {
        get => _mergeStatesProbability;
        set => this.RaiseAndSetIfChanged(ref _mergeStatesProbability, value);
    }

    public ICommand RunCommand { get; }
    public ICommand ResetCommand { get; }

    private void OnRunAlgorithm()
    {
        SaveSettings();
        // App is always available at runtime
        var app = (ClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        app.MainWindow.Content = new VisualizationView()
        {
            DataContext = new VisualizationViewModel(),
        };
    }

    private void OnReset()
    {
        LoadSettings();
    }

    private void SaveSettings()
    {
        Settings settings = new Settings
        {
            MinPopulation = this.MinPopulation,
            MaxPopulation = this.MaxPopulation,
            ConvergenceGenerationNumber = this.ConvergenceGenerationNumber,
            MaximumGenerationNumber = this.MaximumGenerationNumber,
            EliteSelectionScalingFactor = this.EliteSelectionScalingFactor,
            WeightTruePositive = this.WeightTruePositive,
            WeightTrueNegative = this.WeightTrueNegative,
            WeightFalsePositive = this.WeightFalsePositive,
            WeightFalseNegative = this.WeightFalseNegative,
            WeightNonDeterministicEdges = this.WeightNonDeterministicEdges,
            WeightMissingDeterministicEdges = this.WeightMissingDeterministicEdges,
            WeightSize = this.WeightSize,
            MutationProbability = this.MutationProbability,
            NonDeterministicBehaviorProbability = this.NonDeterministicBehaviorProbability,
            ChangeTargetProbability = this.ChangeTargetProbability,
            ChangeSourceProbability = this.ChangeSourceProbability,
            ChangeInputProbability = this.ChangeInputProbability,
            RemoveEdgeProbability = this.RemoveEdgeProbability,
            AddEdgeProbability = this.AddEdgeProbability,
            AddStateProbability = this.AddStateProbability,
            AddAcceptStateProbability = this.AddAcceptStateProbability,
            RemoveAcceptStateProbability = this.RemoveAcceptStateProbability,
            MergeStatesProbability = this.MergeStatesProbability
        };

        _settingsService.SaveSettings(settings);
    }

    private void LoadSettings()
    {
        Settings settings = _settingsService.LoadSettings();

        MinPopulation = settings.MinPopulation;
        MaxPopulation = settings.MaxPopulation;
        ConvergenceGenerationNumber = settings.ConvergenceGenerationNumber;
        MaximumGenerationNumber = settings.MaximumGenerationNumber;
        EliteSelectionScalingFactor = settings.EliteSelectionScalingFactor;
        WeightTruePositive = settings.WeightTruePositive;
        WeightTrueNegative = settings.WeightTrueNegative;
        WeightFalsePositive = settings.WeightFalsePositive;
        WeightFalseNegative = settings.WeightFalseNegative;
        WeightNonDeterministicEdges = settings.WeightNonDeterministicEdges;
        WeightMissingDeterministicEdges = settings.WeightMissingDeterministicEdges;
        WeightSize = settings.WeightSize;
        MutationProbability = settings.MutationProbability;
        NonDeterministicBehaviorProbability = settings.NonDeterministicBehaviorProbability;
        ChangeTargetProbability = settings.ChangeTargetProbability;
        ChangeSourceProbability = settings.ChangeSourceProbability;
        ChangeInputProbability = settings.ChangeInputProbability;
        RemoveEdgeProbability = settings.RemoveEdgeProbability;
        AddEdgeProbability = settings.AddEdgeProbability;
        AddStateProbability = settings.AddStateProbability;
        AddAcceptStateProbability = settings.AddAcceptStateProbability;
        RemoveAcceptStateProbability = settings.RemoveAcceptStateProbability;
        MergeStatesProbability = settings.MergeStatesProbability;
    }
}
