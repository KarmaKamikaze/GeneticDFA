using GeneticSharp;

namespace GeneticDFA;

public class DFAPopulation : Population
{
    public DFAPopulation(int minSize, int maxSize, IChromosome adamChromosome, List<char> alphabet) : base(minSize, maxSize, adamChromosome)
    {
        Alphabet = alphabet;
    }

    private List<char> Alphabet { get; }
    private readonly IRandomization _rnd = RandomizationProvider.Current;
    
    public override void CreateInitialGeneration()
    {
        Generations = new List<Generation>();
        GenerationsNumber = 0;
        List<IChromosome> chromosomes = new List<IChromosome>();
        for (int index = 0; index < MinSize; ++index)
        {
            DFAChromosome chromosome = (DFAChromosome) AdamChromosome.CreateNew();
            if (chromosome == null)
                throw new InvalidOperationException("Adam chromosome's 'CreateNew' method created a null chromosome.");
            InitializeChromosomeStates(chromosome);
            InitializeChromosomeEdges(chromosome);
            chromosome.FindAndAssignNonDeterministicEdges();
            chromosomes.Add(chromosome);
        }
        CreateNewGeneration(chromosomes);
    }

    private void InitializeChromosomeStates(DFAChromosome chromosome)
    {
        int numberOfAcceptStates = _rnd.GetInt(1, Alphabet.Count+1);
        for (int i = 0; i < numberOfAcceptStates; i++)
        {
            chromosome.States.Add(new DFAState(chromosome.NextStateID, true));
            chromosome.NextStateID++;
        }
        for (int i = numberOfAcceptStates; i < Alphabet.Count; i++)
        {
            chromosome.States.Add(new DFAState(chromosome.NextStateID, false));
            chromosome.NextStateID++;
        }

        int startStateIndex = _rnd.GetInt(0, chromosome.States.Count);
        chromosome.StartState = chromosome.States[startStateIndex];
    }

    private void InitializeChromosomeEdges(DFAChromosome chromosome)
    {
        List<DFAEdge> edges = new List<DFAEdge>();
        int edgesToAdd = (int) Math.Pow(Alphabet.Count, 2);
        int uniqueEdgesAdded = 0;
        while (uniqueEdgesAdded < edgesToAdd)
        {
            DFAState source = chromosome.States[_rnd.GetInt(0, chromosome.States.Count)];
            List<DFAEdge> existingEdgesWithCurrentSource = edges.Where(e => e.Source == source).ToList();
            
            List<char> possibleInputs = Alphabet.Where(i => existingEdgesWithCurrentSource.Count(
                e=> e.Input == i) < chromosome.States.Count).ToList();
            char input = possibleInputs[_rnd.GetInt(0, possibleInputs.Count)];
            List<DFAEdge> existingEdgesWithCurrentSourceAndInput =
                existingEdgesWithCurrentSource.Where(e => e.Input == input).ToList();

            List<DFAState> possibleTargets = chromosome.States.Where(s => existingEdgesWithCurrentSourceAndInput.All(
                e => e.Target != s)).ToList();
            DFAState target = possibleTargets[_rnd.GetInt(0, possibleTargets.Count)];
            
            edges.Add(new DFAEdge(chromosome.NextEdgeID,source, target, input));
            chromosome.NextEdgeID++;
            uniqueEdgesAdded++;
        }

        chromosome.Edges.AddRange(edges);
        chromosome.FixUnreachability(Alphabet);
    }


    //Copied from source code, but without gene validation, since we do not use the gene properties on chromosomes
    public override void CreateNewGeneration(IList<IChromosome> chromosomes)
    {
        ExceptionHelper.ThrowIfNull(nameof (chromosomes), chromosomes);
        CurrentGeneration = new Generation(++GenerationsNumber, chromosomes);
        Generations.Add(CurrentGeneration);
        GenerationStrategy.RegisterNewGeneration( this);
    }
    
    
    
}
