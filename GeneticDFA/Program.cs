using System.Reflection;
using GeneticDFA.Utility;
using GeneticDFA.Visualization;
using GeneticSharp;

namespace GeneticDFA;

class Program
{
    static void Main(string[] args)
    {
        const int minPopulation = 1000;
        const int maxPopulation = 1000;
        const int convergenceGenerationNumber = 100;
        const int maximumGenerationNumber = 1000;
        const int fitnessLowerBound = 100;
        const int weightTruePositive = 1;
        const int weightTrueNegative = 1;
        const int weightFalsePositive = 1;
        const int weightFalseNegative = 1;
        const int weightNonDeterministicEdges = 1;
        const int weightMissingDeterministicEdges = 1;
        const int weightSize = 1;
        const double crossoverProbability = 0.5;
        const double mutationProbability = 0.5;
        string testTracePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/traces.json";

        List<TestTrace> traces = DFAUtility.ImportTestTraces(testTracePath);
        List<char> alphabet = DFAUtility.DiscoverAlphabet(traces).ToList();

        EliteSelection selection = new EliteSelection();
        UniformCrossover crossover = new UniformCrossover();
        UniformMutation mutation = new UniformMutation(true);

        // Specific fitness function for the DFA learning problem.
        DFAFitness fitness = new DFAFitness(traces, alphabet, weightTruePositive, weightTrueNegative,
            weightFalsePositive, weightFalseNegative, weightNonDeterministicEdges, weightMissingDeterministicEdges,
            weightSize);
        // Specific chromosome (gene) function for the DFA learning problem.
        DFAChromosome chromosome = new DFAChromosome();

        DFAPopulation population = new DFAPopulation(minPopulation, maxPopulation, chromosome, alphabet);

        OrTermination stoppingCriterion = new OrTermination(new GenerationNumberTermination(maximumGenerationNumber),
            new AndTermination(new FitnessStagnationTermination(convergenceGenerationNumber),
                new FitnessThresholdTermination(fitnessLowerBound)));

        DFAGeneticAlgorithm ga = new DFAGeneticAlgorithm(population, fitness, selection, crossover, mutation,
            stoppingCriterion, crossoverProbability, mutationProbability);

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
