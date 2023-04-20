using System;
using System.Diagnostics;
using GeneticSharp;

namespace GeneticDFA;

public class DFAGeneticAlgorithm : IGeneticAlgorithm
{
    private Stopwatch m_stopwatch;

    public DFAGeneticAlgorithm(
        IPopulation population,
        IFitness fitness,
        ISelection selection,
        ICrossover crossover,
        IMutation mutation,
        ITermination termination,
        int maxGenerationNumber,
        double crossoverProbability,
        double mutationProbability)
    {
        ExceptionHelper.ThrowIfNull(nameof(population), population);
        ExceptionHelper.ThrowIfNull(nameof(fitness), fitness);
        ExceptionHelper.ThrowIfNull(nameof(selection), selection);
        ExceptionHelper.ThrowIfNull(nameof(crossover), crossover);
        ExceptionHelper.ThrowIfNull(nameof(mutation), mutation);
        Population = population;
        Fitness = fitness;
        Selection = selection;
        Crossover = crossover;
        Mutation = mutation;
        Reinsertion = new ElitistReinsertion();
        Termination = termination;
        MaxGenerationNumber = maxGenerationNumber;
        CrossoverProbability = crossoverProbability;
        MutationProbability = mutationProbability;
        TimeEvolving = TimeSpan.Zero;
        TaskExecutor = new ParallelTaskExecutor();
    }

    public event EventHandler GenerationRan;

    public event EventHandler TerminationReached;

    private IPopulation Population { get; }

    private IFitness Fitness { get; }

    private ISelection Selection { get; }

    private ICrossover Crossover { get; }

    private double CrossoverProbability { get; }

    private IMutation Mutation { get; }

    private double MutationProbability { get; }

    private IReinsertion Reinsertion { get; }

    private ITermination Termination { get; }

    public int GenerationsNumber => Population.GenerationsNumber;

    public int MaxGenerationNumber { get; }

    public IChromosome BestChromosome => Population.BestChromosome;

    public TimeSpan TimeEvolving { get; private set; }

    private ITaskExecutor TaskExecutor { get; }

    private IRandomization _rnd = RandomizationProvider.Current;

    public void Start()
    {
        m_stopwatch = Stopwatch.StartNew();
        Population.CreateInitialGeneration();
        m_stopwatch.Stop();
        TimeEvolving = m_stopwatch.Elapsed;

        if (EndCurrentGeneration())
            return;

        while (true)
        {
            m_stopwatch.Restart();
            bool flag = EvolveOneGeneration();
            m_stopwatch.Stop();
            TimeEvolving += m_stopwatch.Elapsed;
            if (flag)
                break;
        }
    }

    // Needs proper implementation
    private bool EvolveOneGeneration()
    {
        IList<IChromosome> selectedElites =
            Selection.SelectChromosomes(SelectionScale(), Population.CurrentGeneration);
        IList<IChromosome> selectedForModification = DFARouletteWheelSelection.SelectChromosomes(Population.MaxSize, selectedElites);
        IList<IChromosome> newPopulation = new List<IChromosome>();
        for (int i = 0; i < selectedForModification.Count; i++)
        {
            if (_rnd.GetDouble(0, 1) < MutationProbability)
            {
                IChromosome clone = selectedForModification[i].Clone();
                Mutation.Mutate(clone, 0);
                newPopulation.Add(clone);
            }
            else
            {
                // Choose i and i + 1 for crossover - increment i an additional time
            }

        }

        Population.CreateNewGeneration(Reinsertion.SelectChromosomes(Population, newPopulation, selectedElites));
        return EndCurrentGeneration();

        /*
        IList<IChromosome> parents = SelectParents();
        IList<IChromosome> chromosomeList = Cross(parents);
        Mutate(chromosomeList);
        Population.CreateNewGeneration(Reinsert(chromosomeList, parents));
        return EndCurrentGeneration(); */
    }

    /// <summary>
    /// Calculates a scaling factor, used to determine how many selections must be considered for mutation
    /// and crossover.
    /// </summary>
    /// <returns>The scaling factor.</returns>
    private int SelectionScale()
    {
        double scale = Population.MaxSize * (GenerationsNumber / (MaxGenerationNumber/2));
        if (scale < 1)
            return Convert.ToInt32(scale);
        return 1;
    }

    private bool EndCurrentGeneration()
    {
        EvaluateFitness();
        Population.EndCurrentGeneration();
        EventHandler generationRan = GenerationRan;
        generationRan(this, EventArgs.Empty);
        if (Termination.HasReached(this))
        {
            EventHandler terminationReached = TerminationReached;
            terminationReached(this, EventArgs.Empty);
            return true;
        }

        return false;
    }

    // Possibly needs proper implementation since it only evaluates fitness on chromosomes without a value assigned?
    // Hinting that the fitness of chromosomes are reset after a mutation or crossover.
    // Depends on how we are going to adjust fitness after mutation and crossover. Probably best to set it to null.
    private void EvaluateFitness()
    {
        try
        {
            List<IChromosome> list = Population.CurrentGeneration.Chromosomes
                .Where<IChromosome>((c => !c.Fitness.HasValue)).ToList();
            foreach (IChromosome c in list)
            {
                TaskExecutor.Add((Action) (() => RunEvaluateFitness(c)));
            }

            if (!TaskExecutor.Start())
                throw new TimeoutException(
                    "The fitness evaluation reached the {0} timeout.".With(TaskExecutor.Timeout));
        }
        finally
        {
            TaskExecutor.Stop();
            TaskExecutor.Clear();
        }

        // Scuffed, but the setter on the Chromosomes list is internal :skull:
        List<IChromosome> tempList = Population.CurrentGeneration.Chromosomes
            .OrderByDescending<IChromosome, double>((c => c.Fitness!.Value)).ToList();
        Population.CurrentGeneration.Chromosomes.Clear();
        foreach (IChromosome chromosome in tempList)
        {
            Population.CurrentGeneration.Chromosomes.Add(chromosome);
        }
    }

    // Could be changed to just take a IChromosome, but for some reason original implementation is this. Maybe for efficiency?
    private void RunEvaluateFitness(object chromosome)
    {
        IChromosome chromosome1 = (chromosome as IChromosome)!;
        try
        {
            chromosome1.Fitness = Fitness.Evaluate(chromosome1);
        }
        catch (Exception ex)
        {
            throw new FitnessException(Fitness, "Error executing Fitness.Evaluate for chromosome: {0}".With(ex.Message),
                ex);
        }
    }
}
