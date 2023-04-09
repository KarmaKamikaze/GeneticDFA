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
        const int weightFalsePositive = -1;
        const int weightFalseNegative = -1;
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
            new TraceModel("0", 1, false),
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
        TestFitness(fitness);
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


    private static void TestFitness(DFAFitness fitness)
    {
        DFAChromosome chromosome = new DFAChromosome();
        
        
        List<DFAStateModel> states = new List<DFAStateModel>()
        {
            new DFAStateModel(1, false),
            new DFAStateModel(2, false),
            new DFAStateModel(3, true),
            new DFAStateModel(4, false),
        };
        
        List<DFAEdgeModel> edges = new List<DFAEdgeModel>()
        {
            new DFAEdgeModel(1, states[0], states[1], '1'),
            new DFAEdgeModel(2, states[0], states[3], '0'),
            new DFAEdgeModel(3, states[1], states[1], '0'),
            new DFAEdgeModel(4, states[1], states[2], '0'),
            new DFAEdgeModel(5, states[2], states[3], '1'),
            new DFAEdgeModel(6, states[2], states[3], '0'),
            new DFAEdgeModel(7, states[3], states[0], '1'),
            new DFAEdgeModel(8, states[3], states[1], '0'),
            new DFAEdgeModel(9, states[3], states[2], '1'),
        };
        
        
        //SmallDFA
        List<DFAStateModel> statesSmallDFA = new List<DFAStateModel>()
        {
            new DFAStateModel(1, false),
            new DFAStateModel(2, false),
            new DFAStateModel(3, true),
        };
        
        List<DFAEdgeModel> edgesSmallDFA = new List<DFAEdgeModel>()
        {
            new DFAEdgeModel(1, states[0], states[0], '0'),
            new DFAEdgeModel(2, states[0], states[1], '1'),
            new DFAEdgeModel(3, states[1], states[0], '0'),
            new DFAEdgeModel(4, states[1], states[2], '1'),
            new DFAEdgeModel(5, states[2], states[0], '1'),
            new DFAEdgeModel(6, states[2], states[0], '0'),
        };
        
        chromosome.States.AddRange(states);
        chromosome.Edges.AddRange(edges);
        //chromosome.StartState = states[0];
        
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        for (int i = 0; i < 1; i++)
        {
            Console.WriteLine(fitness.Evaluate(chromosome));
        }
        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        int elapsedTime = ts.Milliseconds;
        Console.WriteLine("RunTime " + elapsedTime);
        
        
    }
}
