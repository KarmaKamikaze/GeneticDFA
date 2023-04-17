using System.Diagnostics;
using GeneticSharp;

namespace GeneticDFA;

public class DFAGeneticAlgorithm: IGeneticAlgorithm
{
    private Stopwatch m_stopwatch;
    
    public DFAGeneticAlgorithm(
        IPopulation population,
        IFitness fitness,
        ISelection selection,
        ICrossover crossover,
        IMutation mutation,
        ITermination termination,
        double crossoverProbability,
        double mutationProbability)
    {
        ExceptionHelper.ThrowIfNull(nameof (population), population);
        ExceptionHelper.ThrowIfNull(nameof (fitness), fitness);
        ExceptionHelper.ThrowIfNull(nameof (selection), selection);
        ExceptionHelper.ThrowIfNull(nameof (crossover), crossover);
        ExceptionHelper.ThrowIfNull(nameof (mutation), mutation);
        Population = population;
        Fitness = fitness;
        Selection = selection;
        Crossover = crossover;
        Mutation = mutation;
        Reinsertion = new ElitistReinsertion();
        Termination = termination;
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

    public IChromosome BestChromosome => Population.BestChromosome;

    public TimeSpan TimeEvolving { get; private set; }

    private ITaskExecutor TaskExecutor { get; }
    
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

    //Needs proper implementation
    private bool EvolveOneGeneration()
    {
        throw new NotImplementedException();
        
        /*
        IList<IChromosome> parents = SelectParents();
        IList<IChromosome> chromosomeList = Cross(parents);
        Mutate(chromosomeList);
        Population.CreateNewGeneration(Reinsert(chromosomeList, parents));
        return EndCurrentGeneration();*/
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
    
    //Possibly needs proper implementation since it only evaluates fitness on chromosomes without a value assigned?
    //Hinting that the fitness of chromosomes are reset after a mutation or crossover.
    private void EvaluateFitness()
    {
        try
        {
            List<IChromosome> list = Population.CurrentGeneration.Chromosomes.Where<IChromosome>((c => !c.Fitness.HasValue)).ToList();
            foreach (IChromosome c in list)
            {
                TaskExecutor.Add((Action) (() => RunEvaluateFitness(c)));
            }
            if (!TaskExecutor.Start())
                throw new TimeoutException("The fitness evaluation reached the {0} timeout.".With(TaskExecutor.Timeout));
        }
        finally
        {
            TaskExecutor.Stop();
            TaskExecutor.Clear();
        }
        List<IChromosome> tempList = Population.CurrentGeneration.Chromosomes.OrderByDescending<IChromosome, double>((c => c.Fitness.Value)).ToList();
        Population.CurrentGeneration.Chromosomes.Clear();
        foreach (IChromosome chromosome in tempList)
        {
            Population.CurrentGeneration.Chromosomes.Add(chromosome);
        }
    }
    
    //Could be changed to just take a IChromosome, but for some reason original implementation is this. Maybe for efficiency?
    private void RunEvaluateFitness(object chromosome)
    {
        IChromosome chromosome1 = chromosome as IChromosome;
        try
        {
            chromosome1.Fitness = Fitness.Evaluate(chromosome1);
        }
        catch (Exception ex)
        {
            throw new FitnessException(Fitness, "Error executing Fitness.Evaluate for chromosome: {0}".With(ex.Message), ex);
        }
    }
    
    
}
