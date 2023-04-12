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
        List<TraceModel> traces = new List<TraceModel>()
        {
            new TraceModel("11", 2, true),
            new TraceModel("00011", 5, true),
            new TraceModel("110011", 6, true),
            new TraceModel("1011", 4, true),
            new TraceModel("0101101011", 10, true),
            new TraceModel("110", 3, false),
            new TraceModel("01", 1, false),
            new TraceModel("00111", 5, false),
            new TraceModel("1010", 4, false),
            new TraceModel("00000000000111", 14, false),
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

        Population population = new Population(minPopulation, maxPopulation, chromosome);

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
