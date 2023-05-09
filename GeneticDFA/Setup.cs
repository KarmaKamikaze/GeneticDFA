using System.Reflection;
using GeneticDFA.Utility;
using GeneticDFA.Visualization;
using GeneticSharp;

namespace GeneticDFA;

public class Setup
{
    private ThreadTermination ThreadAbort { get; set; } = new ThreadTermination();

    private DFAGeneticAlgorithm Prepare()
    {
        SettingsService settingsService = new SettingsService();
        Settings settings = settingsService.LoadSettings();

        string testTracePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/traces.json";

        List<TestTrace> traces = DFAUtility.ImportTestTraces(testTracePath);
        List<char> alphabet = DFAUtility.DiscoverAlphabet(traces).ToList();

        double fitnessUpperBound = settings.RewardTruePositive * traces.Count(t => t.IsAccepting) +
                                   settings.RewardTrueNegative * traces.Count(t => !t.IsAccepting);
        double fitnessLowerBound = settings.FitnessLowerBoundPercentage * fitnessUpperBound;

        EliteSelection selection = new EliteSelection(settings.NumberOfFittestIndividualsAcrossAllGenerations);
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAMutation mutation = new DFAMutation(alphabet, settings.NonDeterministicBehaviorProbability,
            settings.ChangeTargetProbability, settings.ChangeSourceProbability, settings.RemoveEdgeProbability,
            settings.AddEdgeProbability, settings.AddStateProbability, settings.AddAcceptStateProbability,
            settings.RemoveAcceptStateProbability, settings.MergeStatesProbability, settings.ChangeInputProbability);

        // Specific fitness function for the DFA learning problem.
        DFAFitness fitness = new DFAFitness(traces, alphabet, settings.RewardTruePositive, settings.RewardTrueNegative,
            settings.PenaltyFalsePositive, settings.PenaltyFalseNegative, settings.WeightNonDeterministicEdges,
            settings.WeightUnreachableStates, settings.WeightSize);
        // Specific chromosome (gene) function for the DFA learning problem.
        DFAChromosome chromosome = new DFAChromosome();


        PerformanceGenerationStrategy generationStrategy = new PerformanceGenerationStrategy(1);
        DFAPopulation population =
            new DFAPopulation(settings.MinPopulation, settings.MaxPopulation, chromosome, alphabet, generationStrategy);

        OrTermination stoppingCriterion = new OrTermination(
            new GenerationNumberTermination(settings.MaximumGenerationNumber),
            new AndTermination(new FitnessStagnationTermination(settings.ConvergenceGenerationNumber),
                new FitnessThresholdTermination(fitnessLowerBound)), ThreadAbort);

        DFAGeneticAlgorithm ga = new DFAGeneticAlgorithm(population, fitness, selection,
            settings.EliteSelectionScalingFactor, crossover, mutation, stoppingCriterion,
            settings.MaximumGenerationNumber, settings.CrossoverProbability, settings.MutationProbability);

        // Output continuous evaluation of each generation.
        ga.GenerationRan += (s, e) =>
            Console.WriteLine($"Generation {ga.GenerationsNumber}. Best fitness: {ga.BestChromosome.Fitness!.Value}. " +
                              $"Accuracy: {Math.Round(100 * (ga.BestChromosome.Fitness!.Value / fitnessUpperBound), 2)}%." +
                              $" Time: {DateTime.Now:d} {DateTime.Now:HH:mm:ss}");

        ga.TerminationReached += (s, e) => Console.WriteLine("GA has terminated");

        return ga;
    }

    public void TerminalRun()
    {
        DFAGeneticAlgorithm ga = Prepare();

        // Output graph visualizations of the fittest chromosome each generation.
        ga.GenerationRan += (s, e) =>
            GraphVisualization.SaveToSvgFile((DFAChromosome) ga.BestChromosome, ga.GenerationsNumber);

        GraphVisualization.DeleteFolderRecursive("./Visualizations/", true); // Clean up previous runs
        Directory.CreateDirectory("./Visualizations/");

        // Begin learning.
        Console.WriteLine($"GA is learning the DFA. Time started: {DateTime.Now:d} {DateTime.Now:HH:mm:ss}");
        ga.Start();

        Console.WriteLine();
        Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
        Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");
        Console.ReadKey();
    }

    public void ProcessRun()
    {
        DFAGeneticAlgorithm ga = Prepare();

        // Output graph visualizations of the fittest chromosome each generation.
        ga.GenerationRan += (s, e) =>
            GraphVisualization.SaveToPngFile((DFAChromosome) ga.BestChromosome, ga.GenerationsNumber);

        GraphVisualization.DeleteFolderRecursive("./Visualizations/", true); // Clean up previous runs
        Directory.CreateDirectory("./Visualizations/");
        ga.Start();
    }

    public void Kill()
    {
        ThreadAbort.KillThread = true;
    }
}
