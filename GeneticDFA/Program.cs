using System.Reflection;
using GeneticDFA.Utility;
using GeneticDFA.Visualization;
using GeneticSharp;

namespace GeneticDFA;

class Program
{
    static void Main(string[] args)
    {
        const int minPopulation = 10000;
        const int maxPopulation = 10000;
        const int convergenceGenerationNumber = 100;
        const int maximumGenerationNumber = 1000;
        const int eliteSelectionScalingFactor = 20;
        const int fitnessLowerBound = 100;
        const int numberOfFittestIndividualsAcrossAllGenerations = 500;
        const int weightTruePositive = 10;
        const int weightTrueNegative = 10;
        const double weightFalsePositive = 3;
        const double weightFalseNegative = 3;
        const double weightNonDeterministicEdges = 0.6;
        const double weightMissingDeterministicEdges = 0.5;
        const double weightSize = 0.3;
        const double crossoverProbability = 0.5;
        const double mutationProbability = 0.5;
        const double nonDeterministicBehaviorProbability = 0.5;
        const double changeTargetProbability = 0.1;
        const double changeSourceProbability = 0.1;
        const double changeInputProbability = 0.1;
        const double removeEdgeProbability = 0.1;
        const double addEdgeProbability = 0.2;
        const double addStateProbability = 0.1;
        const double addAcceptStateProbability = 0.1;
        const double removeAcceptStateProbability = 0.1;
        const double mergeStatesProbability = 0.1;


        string testTracePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/traces.json";

        List<TestTrace> traces = DFAUtility.ImportTestTraces(testTracePath);
        List<char> alphabet = DFAUtility.DiscoverAlphabet(traces).ToList();

        EliteSelection selection = new EliteSelection(numberOfFittestIndividualsAcrossAllGenerations);
        UniformCrossover crossover = new UniformCrossover();
        DFAMutation mutation = new DFAMutation(alphabet, nonDeterministicBehaviorProbability, changeTargetProbability,
            changeSourceProbability, removeEdgeProbability, addEdgeProbability, addStateProbability,
            addAcceptStateProbability, removeAcceptStateProbability, mergeStatesProbability, changeInputProbability);

        // Specific fitness function for the DFA learning problem.
        DFAFitness fitness = new DFAFitness(traces, alphabet, weightTruePositive, weightTrueNegative,
            weightFalsePositive, weightFalseNegative, weightNonDeterministicEdges, weightMissingDeterministicEdges,
            weightSize);
        // Specific chromosome (gene) function for the DFA learning problem.
        DFAChromosome chromosome = new DFAChromosome();

        PerformanceGenerationStrategy generationStrategy = new PerformanceGenerationStrategy(10);
        DFAPopulation population =
            new DFAPopulation(minPopulation, maxPopulation, chromosome, alphabet, generationStrategy);

        OrTermination stoppingCriterion = new OrTermination(new GenerationNumberTermination(maximumGenerationNumber),
            new AndTermination(new FitnessStagnationTermination(convergenceGenerationNumber),
                new FitnessThresholdTermination(fitnessLowerBound)));

        DFAGeneticAlgorithm ga = new DFAGeneticAlgorithm(population, fitness, selection, eliteSelectionScalingFactor,
            crossover, mutation, stoppingCriterion, maximumGenerationNumber, crossoverProbability, mutationProbability);

        // Output continuous evaluation of each generation.
        ga.GenerationRan += (s, e) =>
            Console.WriteLine($"Generation {ga.GenerationsNumber}. Best fitness: {ga.BestChromosome.Fitness!.Value}");

        // Output graph visualizations of the fittest chromosome each generation.
        ga.GenerationRan += (s, e) =>
            GraphVisualization.SaveToSvgFile((DFAChromosome) ga.BestChromosome, ga.GenerationsNumber);

        // Begin learning.
        Console.WriteLine("GA is learning the DFA...");
        ga.Start();

        Console.WriteLine();
        Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
        Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");
        Console.ReadKey();
    }
}
