using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using GeneticDFA.Utility;
using GeneticDFAUI.Views;
using ReactiveUI;
using System.Threading;
using GeneticDFA;

namespace GeneticDFAUI.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private int _minPopulation = 3500;
    private int _maxPopulation = 3500;
    private int _convergenceGenerationNumber = 100;
    private int _maximumGenerationNumber = 400;
    private double _eliteCarryOverPercentage = 0.05;
    private int _eliteSelectionScalingFactor = 2;
    private int _rewardTruePositive = 10;
    private int _rewardTrueNegative = 10;
    private double _penaltyFalsePositive = 10;
    private double _penaltyFalseNegative = 10;
    private double _weightNonDeterministicEdges = 1;
    private double _weightUnreachableStates = 1;
    private double _weightSize = 1;
    private double _fitnessLowerBoundPercentage = 0.95;
    private double _mutationProbability = 0.75;
    private double _nonDeterministicBehaviorProbability = 0.65;
    private double _changeTargetProbability = 0.11;
    private double _changeSourceProbability = 0.11;
    private double _changeInputProbability = 0.11;
    private double _removeEdgeProbability = 0.11;
    private double _addEdgeProbability = 0.12;
    private double _addStateProbability = 0.11;
    private double _addAcceptStateProbability = 0.11;
    private double _removeAcceptStateProbability = 0.11;
    private double _mergeStatesProbability = 0.11;

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

    public double EliteCarryOverPercentage
    {
        get => _eliteCarryOverPercentage;
        set => this.RaiseAndSetIfChanged(ref _eliteCarryOverPercentage, value);
    }

    public int EliteSelectionScalingFactor
    {
        get => _eliteSelectionScalingFactor;
        set => this.RaiseAndSetIfChanged(ref _eliteSelectionScalingFactor, value);
    }

    public int RewardTruePositive
    {
        get => _rewardTruePositive;
        set => this.RaiseAndSetIfChanged(ref _rewardTruePositive, value);
    }

    public int RewardTrueNegative
    {
        get => _rewardTrueNegative;
        set => this.RaiseAndSetIfChanged(ref _rewardTrueNegative, value);
    }

    public double PenaltyFalsePositive
    {
        get => _penaltyFalsePositive;
        set => this.RaiseAndSetIfChanged(ref _penaltyFalsePositive, value);
    }

    public double PenaltyFalseNegative
    {
        get => _penaltyFalseNegative;
        set => this.RaiseAndSetIfChanged(ref _penaltyFalseNegative, value);
    }

    public double WeightNonDeterministicEdges
    {
        get => _weightNonDeterministicEdges;
        set => this.RaiseAndSetIfChanged(ref _weightNonDeterministicEdges, value);
    }

    public double WeightUnreachableStates
    {
        get => _weightUnreachableStates;
        set => this.RaiseAndSetIfChanged(ref _weightUnreachableStates, value);
    }

    public double WeightSize
    {
        get => _weightSize;
        set => this.RaiseAndSetIfChanged(ref _weightSize, value);
    }

    public double FitnessLowerBoundPercentage
    {
        get => _fitnessLowerBoundPercentage;
        set => this.RaiseAndSetIfChanged(ref _fitnessLowerBoundPercentage, value);
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
        Setup ga = new Setup();
        Thread thread = StartGeneticAlgorithm(ga);
        // App is always available at runtime
        var app = (ClassicDesktopStyleApplicationLifetime) Application.Current!.ApplicationLifetime!;
        app.MainWindow.Content = new VisualizationView()
        {
            DataContext = new VisualizationViewModel(ga, thread),
        };
    }

    private Thread StartGeneticAlgorithm(Setup ga)
    {
        Thread thread = new Thread(ga.ProcessRun)
        {
            IsBackground = true
        };
        thread.Start();

        return thread;
    }

    private void OnReset()
    {
        MinPopulation = 3500;
        MaxPopulation = 3500;
        ConvergenceGenerationNumber = 100;
        MaximumGenerationNumber = 400;
        EliteCarryOverPercentage = 0.05;
        EliteSelectionScalingFactor = 2;
        RewardTruePositive = 10;
        RewardTrueNegative = 10;
        PenaltyFalsePositive = 10;
        PenaltyFalseNegative = 10;
        WeightNonDeterministicEdges = 1;
        WeightUnreachableStates = 1;
        WeightSize = 1;
        FitnessLowerBoundPercentage = 0.95;
        MutationProbability = 0.75;
        NonDeterministicBehaviorProbability = 0.65;
        ChangeTargetProbability = 0.11;
        ChangeSourceProbability = 0.11;
        ChangeInputProbability = 0.11;
        RemoveEdgeProbability = 0.11;
        AddEdgeProbability = 0.12;
        AddStateProbability = 0.11;
        AddAcceptStateProbability = 0.11;
        RemoveAcceptStateProbability = 0.11;
        MergeStatesProbability = 0.11;
    }

    private void SaveSettings()
    {
        Settings settings = new Settings
        {
            MinPopulation = this.MinPopulation,
            MaxPopulation = this.MaxPopulation,
            ConvergenceGenerationNumber = this.ConvergenceGenerationNumber,
            MaximumGenerationNumber = this.MaximumGenerationNumber,
            EliteCarryOverPercentage = this.EliteCarryOverPercentage,
            EliteSelectionScalingFactor = this.EliteSelectionScalingFactor,
            RewardTruePositive = this.RewardTruePositive,
            RewardTrueNegative = this.RewardTrueNegative,
            PenaltyFalsePositive = this.PenaltyFalsePositive,
            PenaltyFalseNegative = this.PenaltyFalseNegative,
            WeightNonDeterministicEdges = this.WeightNonDeterministicEdges,
            WeightUnreachableStates = this.WeightUnreachableStates,
            WeightSize = this.WeightSize,
            FitnessLowerBoundPercentage = this.FitnessLowerBoundPercentage,
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
        EliteCarryOverPercentage = settings.EliteCarryOverPercentage;
        EliteSelectionScalingFactor = settings.EliteSelectionScalingFactor;
        RewardTruePositive = settings.RewardTruePositive;
        RewardTrueNegative = settings.RewardTrueNegative;
        PenaltyFalsePositive = settings.PenaltyFalsePositive;
        PenaltyFalseNegative = settings.PenaltyFalseNegative;
        WeightNonDeterministicEdges = settings.WeightNonDeterministicEdges;
        WeightUnreachableStates = settings.WeightUnreachableStates;
        WeightSize = settings.WeightSize;
        FitnessLowerBoundPercentage = settings.FitnessLowerBoundPercentage;
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
