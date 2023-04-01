using GeneticSharp;

namespace GeneticDFA;

class Program
{
    static void Main(string[] args)
    {
        const int chromosomeBase = 10;
        const int minPopulation = 200;
        const int maxPopulation = 800;
        const int convergenceGenerationNumber = 100;
        const int maximumGenerationNumber = 1000;
        const int fitnessLowerBound = 100;
        
        EliteSelection selection = new EliteSelection();
        UniformCrossover crossover = new UniformCrossover();
        UniformMutation mutation = new UniformMutation(true);

        // Specific fitness function for the DFA learning problem.
        DFAFitness fitness = new DFAFitness();
        // Specific chromosome (gene) function for the DFA learning problem.
        DFAChromosome chromosome = new DFAChromosome(chromosomeBase);

        Population population = new Population(minPopulation, maxPopulation, chromosome);

        
        OrTermination stoppingCriterion = new OrTermination(new GenerationNumberTermination(maximumGenerationNumber), 
            new AndTermination(new FitnessStagnationTermination(convergenceGenerationNumber), new FitnessThresholdTermination(fitnessLowerBound)));
        
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
        ga.Start();

        Console.WriteLine();
        Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
        Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");
        Console.ReadKey();
    }
}
