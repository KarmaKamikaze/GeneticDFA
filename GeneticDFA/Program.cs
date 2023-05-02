using System.Reflection;
using GeneticDFA.Utility;
using GeneticDFA.Visualization;
using GeneticSharp;

namespace GeneticDFA;

class Program
{
    static void Main(string[] args)
    {
        const int minPopulation = 3500;
        const int maxPopulation = 3500;
        const int convergenceGenerationNumber = 100;
        const int maximumGenerationNumber = 400;
        const int eliteSelectionScalingFactor = 2;
        int numberOfFittestIndividualsAcrossAllGenerations = Convert.ToInt32(0.05 * minPopulation);
        const int rewardTruePositive = 10;
        const int rewardTrueNegative = 10;
        const double penaltyFalsePositive = 10;
        const double penaltyFalseNegative = 10;
        const double weightNonDeterministicEdges = 2;
        const double weightUnreachableStates = 5;
        const double weightSize = 5;
        const double mutationProbability = 0.5;
        const double crossoverProbability = 1 - mutationProbability;
        const double nonDeterministicBehaviorProbability = 0.5;
        const double changeTargetProbability = 0.11;
        const double changeSourceProbability = 0.11;
        const double changeInputProbability = 0.11;
        const double removeEdgeProbability = 0.11;
        const double addEdgeProbability = 0.12;
        const double addStateProbability = 0.11;
        const double addAcceptStateProbability = 0.11;
        const double removeAcceptStateProbability = 0.11;
        const double mergeStatesProbability = 0.11;


        string testTracePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/traces.json";

        List<TestTrace> traces = DFAUtility.ImportTestTraces(testTracePath);
        List<char> alphabet = DFAUtility.DiscoverAlphabet(traces).ToList();

        double fitnessUpperBound = rewardTruePositive * traces.Count(t => t.IsAccepting) +
                                   rewardTrueNegative * traces.Count(t => !t.IsAccepting);
        double fitnessLowerBound = 0.95 * fitnessUpperBound;

        EliteSelection selection = new EliteSelection(numberOfFittestIndividualsAcrossAllGenerations);
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAMutation mutation = new DFAMutation(alphabet, nonDeterministicBehaviorProbability, changeTargetProbability,
            changeSourceProbability, removeEdgeProbability, addEdgeProbability, addStateProbability,
            addAcceptStateProbability, removeAcceptStateProbability, mergeStatesProbability, changeInputProbability);

        // Specific fitness function for the DFA learning problem.
        DFAFitness fitness = new DFAFitness(traces, alphabet, rewardTruePositive, rewardTrueNegative,
            penaltyFalsePositive, penaltyFalseNegative, weightNonDeterministicEdges, weightUnreachableStates,
            weightSize);
        // Specific chromosome (gene) function for the DFA learning problem.
        DFAChromosome chromosome = new DFAChromosome();


        PerformanceGenerationStrategy generationStrategy = new PerformanceGenerationStrategy(1);
        DFAPopulation population =
            new DFAPopulation(minPopulation, maxPopulation, chromosome, alphabet, generationStrategy);

        OrTermination stoppingCriterion = new OrTermination(new GenerationNumberTermination(maximumGenerationNumber),
            new AndTermination(new FitnessStagnationTermination(convergenceGenerationNumber),
                new FitnessThresholdTermination(fitnessLowerBound)));

        DFAGeneticAlgorithm ga = new DFAGeneticAlgorithm(population, fitness, selection, eliteSelectionScalingFactor,
            crossover, mutation, stoppingCriterion, maximumGenerationNumber, crossoverProbability, mutationProbability);

        // Output continuous evaluation of each generation.
        ga.GenerationRan += (s, e) =>
            Console.WriteLine($"Generation {ga.GenerationsNumber}. Best fitness: {ga.BestChromosome.Fitness!.Value}. " +
                              $"Accuracy: {Math.Round(100 * (ga.BestChromosome.Fitness!.Value / fitnessUpperBound), 2)}%. Time: {DateTime.Now:d} {DateTime.Now:HH:mm:ss}");

        // Output graph visualizations of the fittest chromosome each generation.
        ga.GenerationRan += (s, e) =>
            GraphVisualization.SaveToSvgFile((DFAChromosome) ga.BestChromosome, ga.GenerationsNumber);

        ga.TerminationReached += (s, e) => Console.WriteLine("GA has terminated");

        // Begin learning.
        Console.WriteLine($"GA is learning the DFA. Time started: {DateTime.Now:d} {DateTime.Now:HH:mm:ss}");
        ga.Start();

        Console.WriteLine();
        Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
        Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");
        Console.ReadKey();
    }
}
