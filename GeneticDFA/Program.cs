using System.Diagnostics;
using GeneticSharp;

namespace GeneticDFA;

class Program
{
    static void Main(string[] args)
    {
        const int minPopulation = 200;
        const int maxPopulation = 800;
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
        
        //Sample traces of SmallDFA
        List<TestTrace> traces = new List<TestTrace>()
        {
            new TestTrace("11", true),
            new TestTrace("00011", true),
            new TestTrace("110011", true),
            new TestTrace("1011", true),
            new TestTrace("0101101011", true),
            new TestTrace("110", false),
            new TestTrace("01", false),
            new TestTrace("00111", false),
            new TestTrace("1010", false),
            new TestTrace("00000000000111", false),
        };
        List<char> alphabet = new List<char>() {'1', '0'};
        
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

        GeneticAlgorithm ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
        {
            // Terminate when stopping criterion is met.
            Termination = stoppingCriterion
        };

        // Output continuous evaluation of each generation.
        ga.GenerationRan += (s, e) =>
            Console.WriteLine($"Generation {ga.GenerationsNumber}. Best fitness: {ga.BestChromosome.Fitness!.Value}");

        // Begin learning.
        Console.WriteLine("GA is learning the DFA...");
        /*ga.Start();

        Console.WriteLine();
        Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
        Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");
        Console.ReadKey();*/
    }
    
}
