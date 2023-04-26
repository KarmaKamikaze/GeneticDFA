using System.Diagnostics;
using GeneticSharp;

namespace GeneticDFA;

public class DFAGeneticAlgorithm : IGeneticAlgorithm
{
    private Stopwatch m_stopwatch;
    private readonly object m_lock = new object();
    
    public DFAGeneticAlgorithm(
        IPopulation population,
        IFitness fitness,
        ISelection selection,
        int eliteSelectionScalingFactor,
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
        EliteSelectionScalingFactor = eliteSelectionScalingFactor;
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

    private int EliteSelectionScalingFactor { get; }

    private ICrossover Crossover { get; }

    private double CrossoverProbability { get; }

    private IMutation Mutation { get; }

    private double MutationProbability { get; }

    private IReinsertion Reinsertion { get; }

    private ITermination Termination { get; }

    public int GenerationsNumber => Population.GenerationsNumber;

    private int MaxGenerationNumber { get; }

    public IChromosome BestChromosome => Population.BestChromosome;

    public TimeSpan TimeEvolving { get; private set; }

    private ITaskExecutor TaskExecutor { get; }

    private readonly IRandomization _rnd = RandomizationProvider.Current;

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
        // First use Elitist selection to include elites
        IList<IChromosome> selectedElites =
            Selection.SelectChromosomes(SelectionScale(), Population.CurrentGeneration);
        // Use roulette wheel selection on the selected elites. This can cause elites to be mutated multiple times
        IList<IChromosome> selectedForModification = DFARouletteWheelSelection.SelectChromosomes(Population.MaxSize, selectedElites);
        IList<IChromosome> newPopulation = new List<IChromosome>();

        try
        {
            for (int i = 0; i < selectedForModification.Count; i++)
            {
                if (_rnd.GetDouble(0, 1) < MutationProbability)
                {
                    int i1 = i;
                    TaskExecutor.Add((Action) (() =>
                    {
                        IChromosome clone = selectedForModification[i1].Clone();
                        Mutation.Mutate(clone, 0);
                        clone.Fitness = null;
                        // Use mutex since otherwise two threads will attempt to access the shared list at the same time
                        lock (m_lock)
                        {
                            newPopulation.Add(clone);
                        }
                    }));
                }
                else
                { 
                    // Replace with crossover
                    int i1 = i;
                    TaskExecutor.Add((Action) (() =>
                    {
                        IChromosome clone = selectedForModification[i1].Clone();
                        Mutation.Mutate(clone, 0);
                        clone.Fitness = null;
                        lock (m_lock)
                        {
                            newPopulation.Add(clone);
                        }
                    }));
                }

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

        Population.CreateNewGeneration(newPopulation);
        return EndCurrentGeneration();
        
    }

    /// <summary>
    /// Calculates a scaling factor, used to determine how many selections must be considered for mutation
    /// and crossover.
    /// </summary>
    /// <returns>The scaling factor.</returns>
    private int SelectionScale()
    {
        double scale = (GenerationsNumber / ( (double) MaxGenerationNumber/EliteSelectionScalingFactor));
        if (scale < 1)
        {
            return Population.MaxSize * scale > 2 ? Convert.ToInt32(Population.MaxSize * scale) : 3;
        }

        return Population.MaxSize;
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
    
    private void EvaluateFitness()
    {
        try
        {
            List<IChromosome> list = Population.CurrentGeneration.Chromosomes
                .Where<IChromosome>(c => !c.Fitness.HasValue).ToList();
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

        List<IChromosome> tempList = Population.CurrentGeneration.Chromosomes
            .OrderByDescending<IChromosome, double>((c => c.Fitness!.Value)).ToList();
        Population.CurrentGeneration.Chromosomes.Clear();
        foreach (IChromosome chromosome in tempList)
        {
            Population.CurrentGeneration.Chromosomes.Add(chromosome);
        }
    }

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
