using GeneticSharp;

namespace GeneticDFA;

public class DFAPopulation : Population
{
    public DFAPopulation(int minSize, int maxSize, IChromosome adamChromosome, List<char> alphabet) : base(minSize,
        maxSize, adamChromosome)
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
            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(chromosome);
            chromosomes.Add(chromosome);
        }

        CreateNewGeneration(chromosomes);
    }

    private void InitializeChromosomeStates(DFAChromosome chromosome)
    {
        // There should be at least 1 accept state
        // Note that the max value parameter of GetInt() is not included in the range of possible values
        int numberOfAcceptStates = _rnd.GetInt(1, Alphabet.Count + 1);
        for (int i = 0; i < numberOfAcceptStates; i++)
        {
            chromosome.States.Add(new DFAState(chromosome.NextStateId, true));
            chromosome.NextStateId++;
        }

        for (int i = numberOfAcceptStates; i < Alphabet.Count; i++)
        {
            chromosome.States.Add(new DFAState(chromosome.NextStateId, false));
            chromosome.NextStateId++;
        }

        /* The following causes the start state to not always be the first state in the list,
        but that property is not upheld anyways due to the state merge mutation operator
        This approach is faster than the opposite. I.e. first assigning the start state to the first state,
        and then iterating through the list of states and assigning accept states based on randomness.
        That would cause possible reiteration.
        Furthermore, there would be bias towards making the start state an accept state,
        since the iteration of the states would start with the first element.
        The actual implementation ensures that there is no bias, and it is also faster than the aforementioned approach,
        since we do not have possible reiteration.
        The downside is readability, since the start state is not always the first state. */
        int startStateIndex = _rnd.GetInt(0, chromosome.States.Count);
        chromosome.StartState = chromosome.States[startStateIndex];
    }

    private void InitializeChromosomeEdges(DFAChromosome chromosome)
    {
        List<DFAEdge> edges = new List<DFAEdge>();
        int edgesToAdd = (int) Math.Pow(Alphabet.Count, 2);
        int uniqueEdgesAdded = 0;

        // This loop always terminates. When the alphabet size is x, the chromosome will have x states.
        // For each state there are x^2 possible edges where that state is the source.
        // Thus there are x^3 possible unique edges. Since edgesToAdd is x^2, the loop will always terminate.
        while (uniqueEdgesAdded < edgesToAdd)
        {
            // Note that we do not check whether the source chosen is a valid source (as we do with input and target),
            // based on the same reasoning as for the loop termination. I.e. each state has x^2 unique edges,
            // so if the source is not valid, all x^2 of its edges must already be created,
            // but in that case the loop should have terminated beforehand.
            DFAState source = chromosome.States[_rnd.GetInt(0, chromosome.States.Count)];
            List<DFAEdge> existingEdgesWithCurrentSource = edges.Where(e => e.Source == source).ToList();

            // Find the inputs that can be used to create a new unique edge with the chosen source
            List<char> possibleInputs = Alphabet.Where(i => existingEdgesWithCurrentSource.Count(
                e => e.Input == i) < chromosome.States.Count).ToList();
            char input = possibleInputs[_rnd.GetInt(0, possibleInputs.Count)];
            List<DFAEdge> existingEdgesWithCurrentSourceAndInput =
                existingEdgesWithCurrentSource.Where(e => e.Input == input).ToList();

            // Find the targets that can be used to create a new unique edge with the chosen source and input
            List<DFAState> possibleTargets = chromosome.States.Where(s => existingEdgesWithCurrentSourceAndInput.All(
                e => e.Target != s)).ToList();
            DFAState target = possibleTargets[_rnd.GetInt(0, possibleTargets.Count)];

            edges.Add(new DFAEdge(chromosome.NextEdgeId, source, target, input));
            chromosome.NextEdgeId++;
            uniqueEdgesAdded++;
        }

        chromosome.Edges.AddRange(edges);
        DFAChromosomeHelper.FixUnreachability(chromosome, Alphabet);
    }


    // Copied from source code, but without gene validation, since we do not use the gene properties on chromosomes
    public override void CreateNewGeneration(IList<IChromosome> chromosomes)
    {
        ExceptionHelper.ThrowIfNull(nameof(chromosomes), chromosomes);
        CurrentGeneration = new Generation(++GenerationsNumber, chromosomes);
        Generations.Add(CurrentGeneration);
        GenerationStrategy.RegisterNewGeneration(this);
    }
}
