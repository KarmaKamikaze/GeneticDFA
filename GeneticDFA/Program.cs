using System.Reflection;
using GeneticDFA.Utility;
using GeneticDFA.Visualization;
using GeneticSharp;

namespace GeneticDFA;

class Program
{
    static void Main(string[] args)
    {
        SettingsService settingsService = new SettingsService();
        Settings settings = settingsService.LoadSettings();


        string testTracePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/traces.json";

        List<TestTrace> traces = DFAUtility.ImportTestTraces(testTracePath);
        List<char> alphabet = DFAUtility.DiscoverAlphabet(traces).ToList();

        EliteSelection selection = new EliteSelection(settings.NumberOfFittestIndividualsAcrossAllGenerations);
        UniformCrossover crossover = new UniformCrossover();
        DFAMutation mutation = new DFAMutation(alphabet, settings.NonDeterministicBehaviorProbability,
            settings.ChangeTargetProbability, settings.ChangeSourceProbability, settings.RemoveEdgeProbability,
            settings.AddEdgeProbability, settings.AddStateProbability, settings.AddAcceptStateProbability,
            settings.RemoveAcceptStateProbability, settings.MergeStatesProbability, settings.ChangeInputProbability);

        // Specific fitness function for the DFA learning problem.
        DFAFitness fitness = new DFAFitness(traces, alphabet, settings.WeightTruePositive, settings.WeightTrueNegative,
            settings.WeightFalsePositive, settings.WeightFalseNegative, settings.WeightNonDeterministicEdges,
            settings.WeightMissingDeterministicEdges, settings.WeightSize);
        // Specific chromosome (gene) function for the DFA learning problem.
        DFAChromosome chromosome = new DFAChromosome();


        PerformanceGenerationStrategy generationStrategy = new PerformanceGenerationStrategy(1);
        DFAPopulation population =
            new DFAPopulation(settings.MinPopulation, settings.MaxPopulation, chromosome, alphabet, generationStrategy);

        OrTermination stoppingCriterion = new OrTermination(
            new GenerationNumberTermination(settings.MaximumGenerationNumber),
            new AndTermination(new FitnessStagnationTermination(settings.ConvergenceGenerationNumber),
                new FitnessThresholdTermination(settings.FitnessLowerBound)));

        DFAGeneticAlgorithm ga = new DFAGeneticAlgorithm(population, fitness, selection,
            settings.EliteSelectionScalingFactor, crossover, mutation, stoppingCriterion,
            settings.MaximumGenerationNumber, settings.CrossoverProbability, settings.MutationProbability);

        // Output continuous evaluation of each generation.
        ga.GenerationRan += (s, e) =>
            Console.WriteLine($"Generation {ga.GenerationsNumber}. Best fitness: {ga.BestChromosome.Fitness!.Value}");

        // Output graph visualizations of the fittest chromosome each generation.
        ga.GenerationRan += (s, e) =>
            GraphVisualization.SaveToSvgFile((DFAChromosome) ga.BestChromosome, ga.GenerationsNumber);

        ga.TerminationReached += (s, e) => Console.WriteLine("GA has terminated");

        // Begin learning.
        Console.WriteLine("GA is learning the DFA...");
        ga.Start();

        Console.WriteLine();
        Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
        Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");
        Console.ReadKey();
    }
}
