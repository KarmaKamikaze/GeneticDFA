using System.Diagnostics;
using GeneticSharp;

namespace GeneticDFA;

public class DFAGeneticAlgorithm: IGeneticAlgorithm
{
    private bool m_stopRequested;
    private readonly object m_lock = new object();
    private GeneticAlgorithmState m_state;
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
        State = GeneticAlgorithmState.NotStarted;
        TaskExecutor = new LinearTaskExecutor();
    }
    
    public event EventHandler GenerationRan;

    public event EventHandler TerminationReached;

    public event EventHandler Stopped;

    private IPopulation Population { get; }

    private IFitness Fitness { get; }

    private ISelection Selection { get; }

    private ICrossover Crossover { get; }

    private double CrossoverProbability { get; }

    private IMutation Mutation { get; }

    private double MutationProbability { get; }

    private IReinsertion Reinsertion { get; }

    public ITermination Termination { get; init; }

    public int GenerationsNumber => Population.GenerationsNumber;

    public IChromosome BestChromosome => Population.BestChromosome;

    public TimeSpan TimeEvolving { get; private set; }

    private GeneticAlgorithmState State
    {
        get => m_state;
        set
        {
            int num = m_state == value ? 0 : (value == GeneticAlgorithmState.Stopped ? 1 : 0);
            m_state = value;
            if (num == 0)
                return;
            Stopped(this, EventArgs.Empty);
        }
    }

    public bool IsRunning => State is GeneticAlgorithmState.Started or GeneticAlgorithmState.Resumed;

    public ITaskExecutor TaskExecutor { get; set; }

    
    public void Start()
    {
        lock (m_lock)
        {
            State = GeneticAlgorithmState.Started;
            m_stopwatch = Stopwatch.StartNew();
            Population.CreateInitialGeneration();
            m_stopwatch.Stop();
            TimeEvolving = m_stopwatch.Elapsed;
        }
        Resume();
    }
    
    public void Resume()
    {
        try
        {
            lock (m_lock)
                m_stopRequested = false;
            if (Population.GenerationsNumber == 0)
                throw new InvalidOperationException("Attempt to resume a genetic algorithm which was not yet started.");
            if (Population.GenerationsNumber > 1)
            {
                if (Termination.HasReached(this))
                    throw new InvalidOperationException("Attempt to resume a genetic algorithm with a " +
                                                        "termination ({0}) already reached. " +
                                                        "Please, specify a new termination or extend the current one."
                                                            .With(Termination));
                State = GeneticAlgorithmState.Resumed;
            }
            if (EndCurrentGeneration())
                return;
            while (!m_stopRequested)
            {
                m_stopwatch.Restart();
                bool flag = EvolveOneGeneration();
                m_stopwatch.Stop();
                TimeEvolving += m_stopwatch.Elapsed;
                if (flag)
                    break;
            }
        }
        catch
        {
            State = GeneticAlgorithmState.Stopped;
            throw;
        }
    }
    
    public void Stop()
    {
        if (Population.GenerationsNumber == 0)
            throw new InvalidOperationException("Attempt to stop a genetic algorithm which was not yet started.");
        lock (m_lock)
            m_stopRequested = true;
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
            State = GeneticAlgorithmState.TerminationReached;
            EventHandler terminationReached = TerminationReached;
            terminationReached(this, EventArgs.Empty);
            return true;
        }
        if (m_stopRequested)
        {
            TaskExecutor.Stop();
            State = GeneticAlgorithmState.Stopped;
        }
        return false;
    }
    
    //Possibly needs proper implementation
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
