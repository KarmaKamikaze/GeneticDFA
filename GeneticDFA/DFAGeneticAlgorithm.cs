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
        IMutation mutation)
    {
        ExceptionHelper.ThrowIfNull(nameof (population), (object) population);
        ExceptionHelper.ThrowIfNull(nameof (fitness), (object) fitness);
        ExceptionHelper.ThrowIfNull(nameof (selection), (object) selection);
        ExceptionHelper.ThrowIfNull(nameof (crossover), (object) crossover);
        ExceptionHelper.ThrowIfNull(nameof (mutation), (object) mutation);
        this.Population = population;
        this.Fitness = fitness;
        this.Selection = selection;
        this.Crossover = crossover;
        this.Mutation = mutation;
        this.Reinsertion = (IReinsertion) new ElitistReinsertion();
        this.Termination = (ITermination) new GenerationNumberTermination(1);
        this.CrossoverProbability = 0.75f;
        this.MutationProbability = 0.1f;
        this.TimeEvolving = TimeSpan.Zero;
        this.State = GeneticAlgorithmState.NotStarted;
        this.TaskExecutor = (ITaskExecutor) new LinearTaskExecutor();
        this.OperatorsStrategy = (IOperatorsStrategy) new DefaultOperatorsStrategy();
    }
    
    public event EventHandler GenerationRan;

    public event EventHandler TerminationReached;

    public event EventHandler Stopped;
    
    public IPopulation Population { get; private set; }

    public IFitness Fitness { get; private set; }

    public ISelection Selection { get; set; }

    public ICrossover Crossover { get; set; }

    public float CrossoverProbability { get; set; }

    public IMutation Mutation { get; set; }

    public float MutationProbability { get; set; }

    public IReinsertion Reinsertion { get; set; }

    public ITermination Termination { get; set; }

    public int GenerationsNumber => this.Population.GenerationsNumber;

    public IChromosome BestChromosome => this.Population.BestChromosome;

    public TimeSpan TimeEvolving { get; private set; }

    public GeneticAlgorithmState State
    {
        get => this.m_state;
        private set
        {
            int num = this.Stopped == null || this.m_state == value ? 0 : (value == GeneticAlgorithmState.Stopped ? 1 : 0);
            this.m_state = value;
            if (num == 0)
                return;
            this.Stopped((object) this, EventArgs.Empty);
        }
    }

    public bool IsRunning => this.State == GeneticAlgorithmState.Started || this.State == GeneticAlgorithmState.Resumed;

    public ITaskExecutor TaskExecutor { get; set; }

    
    public void Start()
    {
        lock (this.m_lock)
        {
            this.State = GeneticAlgorithmState.Started;
            this.m_stopwatch = Stopwatch.StartNew();
            this.Population.CreateInitialGeneration();
            this.m_stopwatch.Stop();
            this.TimeEvolving = this.m_stopwatch.Elapsed;
        }
        this.Resume();
    }
    
    public void Resume()
    {
        try
        {
            lock (this.m_lock)
                this.m_stopRequested = false;
            if (this.Population.GenerationsNumber == 0)
                throw new InvalidOperationException("Attempt to resume a genetic algorithm which was not yet started.");
            if (this.Population.GenerationsNumber > 1)
            {
                if (this.Termination.HasReached((IGeneticAlgorithm) this))
                    throw new InvalidOperationException("Attempt to resume a genetic algorithm with a termination ({0}) already reached. Please, specify a new termination or extend the current one.".With((object) this.Termination));
                this.State = GeneticAlgorithmState.Resumed;
            }
            if (this.EndCurrentGeneration())
                return;
            while (!this.m_stopRequested)
            {
                this.m_stopwatch.Restart();
                bool flag = this.EvolveOneGeneration();
                this.m_stopwatch.Stop();
                this.TimeEvolving += this.m_stopwatch.Elapsed;
                if (flag)
                    break;
            }
        }
        catch
        {
            this.State = GeneticAlgorithmState.Stopped;
            throw;
        }
    }
    
    public void Stop()
    {
        if (this.Population.GenerationsNumber == 0)
            throw new InvalidOperationException("Attempt to stop a genetic algorithm which was not yet started.");
        lock (this.m_lock)
            this.m_stopRequested = true;
    }
    
    //Needs proper implementation
    private bool EvolveOneGeneration()
    {
        IList<IChromosome> parents = this.SelectParents();
        IList<IChromosome> chromosomeList = this.Cross(parents);
        this.Mutate(chromosomeList);
        this.Population.CreateNewGeneration(this.Reinsert(chromosomeList, parents));
        return this.EndCurrentGeneration();
    }
    
    
    private bool EndCurrentGeneration()
    {
        this.EvaluateFitness();
        this.Population.EndCurrentGeneration();
        EventHandler generationRan = this.GenerationRan;
        if (generationRan != null)
            generationRan((object) this, EventArgs.Empty);
        if (this.Termination.HasReached((IGeneticAlgorithm) this))
        {
            this.State = GeneticAlgorithmState.TerminationReached;
            EventHandler terminationReached = this.TerminationReached;
            if (terminationReached != null)
                terminationReached((object) this, EventArgs.Empty);
            return true;
        }
        if (this.m_stopRequested)
        {
            this.TaskExecutor.Stop();
            this.State = GeneticAlgorithmState.Stopped;
        }
        return false;
    }
    
    //Possibly needs proper implementation
    private void EvaluateFitness()
    {
        try
        {
            List<IChromosome> list = this.Population.CurrentGeneration.Chromosomes.Where<IChromosome>((Func<IChromosome, bool>) (c => !c.Fitness.HasValue)).ToList<IChromosome>();
            for (int index = 0; index < list.Count; ++index)
            {
                IChromosome c = list[index];
                this.TaskExecutor.Add((Action) (() => this.RunEvaluateFitness((object) c)));
            }
            if (!this.TaskExecutor.Start())
                throw new TimeoutException("The fitness evaluation reached the {0} timeout.".With((object) this.TaskExecutor.Timeout));
        }
        finally
        {
            this.TaskExecutor.Stop();
            this.TaskExecutor.Clear();
        }
        this.Population.CurrentGeneration.Chromosomes = (IList<IChromosome>) this.Population.CurrentGeneration.Chromosomes.OrderByDescending<IChromosome, double>((Func<IChromosome, double>) (c => c.Fitness.Value)).ToList<IChromosome>();
    }
    
    
    private void RunEvaluateFitness(object chromosome)
    {
        IChromosome chromosome1 = chromosome as IChromosome;
        try
        {
            chromosome1.Fitness = new double?(this.Fitness.Evaluate(chromosome1));
        }
        catch (Exception ex)
        {
            throw new FitnessException(this.Fitness, "Error executing Fitness.Evaluate for chromosome: {0}".With((object) ex.Message), ex);
        }
    }
    
    
}
