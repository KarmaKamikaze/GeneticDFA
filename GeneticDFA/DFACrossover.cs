using GeneticSharp;

namespace GeneticDFA;

public class DFACrossover : CrossoverBase
{
    public DFACrossover(int parentsNumber, int childrenNumber, int minChromosomeLength, List<char> alphabet) : base(
        parentsNumber, childrenNumber, minChromosomeLength)
    {
        Alphabet = alphabet;
    }

    private readonly IRandomization _rnd = RandomizationProvider.Current;
    private List<char> Alphabet { get; }

    protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
    {
        DFAChromosome parent1 = (DFAChromosome) parents[0];
        DFAChromosome parent2 = (DFAChromosome) parents[1];

        // Check if the parent chromosomes have enough states and edges to perform crossover.
        if (parent1.States.Count < 2 || parent2.States.Count < 2 || parent1.Edges.Count < 1 || parent2.Edges.Count < 1)
            return new List<IChromosome>() { parents[0].Clone(), parents[1].Clone() };

        // Create two new child chromosomes using the CreateChild helper function.
        DFAChromosome child1 = CreateChild((DFAChromosome) parent1.Clone(), (DFAChromosome) parent2.Clone());
        DFAChromosome child2 = CreateChild((DFAChromosome) parent2.Clone(), (DFAChromosome) parent1.Clone());

        // Fix any unreachability in the child chromosomes using the FixUnreachability method.
        DFAChromosomeHelper.FixUnreachability(child1, Alphabet);
        DFAChromosomeHelper.FixUnreachability(child2, Alphabet);

        DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(child1);
        DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(child2);

        child1.SetNewId();
        child2.SetNewId();

        return new List<IChromosome>() { child1, child2 };
    }

    /// <summary>
    /// Creates a child DFA chromosome by performing crossover on two parent chromosomes.
    /// </summary>
    /// <param name="parent1">The first parent chromosome.</param>
    /// <param name="parent2">The second parent chromosome.</param>
    /// <returns>A new DFA chromosome representing the child chromosome resulting from crossover.</returns>
    private DFAChromosome CreateChild(DFAChromosome parent1, DFAChromosome parent2)
    {
        // Select half of the consecutive states from each parent chromosome using breadth-first search.
        List<DFAState> selectedStatesP1 = SelectStatesBreadthFirst(parent1);
        List<DFAState> selectedStatesP2 = SelectStatesBreadthFirst(parent2);

        // Find the states from the second parent that were not selected by breadth first search.
        List<DFAState> notSelectedStatesP2 = parent2.States.Where(s => !selectedStatesP2.Contains(s)).ToList();

        // Create the child chromosome.
        DFAChromosome child = new DFAChromosome();
        child.States.AddRange(selectedStatesP1);
        child.States.AddRange(notSelectedStatesP2);
        child.StartState = parent1.StartState;

        // Update the Ids of states to ensure uniqueness.
        foreach (DFAState state in child.States)
        {
            state.Id = child.NextStateId++;
        }

        // Find the edges from parent 1 that should be copied over to the child
        List<DFAEdge> edgesFromP1 = parent1.Edges.Where(e => selectedStatesP1.Contains(e.Source)).ToList();
        for (int i = edgesFromP1.Count - 1; i >= 0; i--)
        {
            // If the target state of an edge is not in the selected states from parent 1,
            // randomly select a target state from the not selected states from parent 2.
            if (!selectedStatesP1.Contains(edgesFromP1[i].Target))
            {
                int i1 = i;
                List<DFAState> possibleTargets = notSelectedStatesP2.Where(s =>
                        !edgesFromP1.Any(e =>
                            e.Source == edgesFromP1[i1].Source && e.Input == edgesFromP1[i1].Input && e.Target == s))
                    .ToList();
                if (possibleTargets.Count == 0)
                    edgesFromP1.Remove(edgesFromP1[i]);
                else
                    edgesFromP1[i].Target = possibleTargets[_rnd.GetInt(0, possibleTargets.Count)];
            }
        }

        // Find the edges from parent 2 that should be copied over to the child (Opposite of the above)
        List<DFAEdge> edgesFromP2 = parent2.Edges.Where(e => notSelectedStatesP2.Contains(e.Source)).ToList();
        for (int i = edgesFromP2.Count - 1; i >= 0; i--)
        {
            // If the target state of an edge is not in the not selected states from parent 2,
            // randomly select a target state from the selected states from parent 1.
            if (!notSelectedStatesP2.Contains(edgesFromP2[i].Target))
            {
                int i1 = i;
                List<DFAState> possibleTargets = selectedStatesP1.Where(s =>
                        !edgesFromP2.Any(e =>
                            e.Source == edgesFromP2[i1].Source && e.Input == edgesFromP2[i1].Input && e.Target == s))
                    .ToList();
                if (possibleTargets.Count == 0)
                    edgesFromP2.Remove(edgesFromP2[i]);
                else
                    edgesFromP2[i].Target = possibleTargets[_rnd.GetInt(0, possibleTargets.Count)];
            }
        }

        child.Edges.AddRange(edgesFromP1);
        child.Edges.AddRange(edgesFromP2);

        // Update the IDs of edges to ensure uniqueness
        foreach (DFAEdge edge in child.Edges)
        {
            edge.Id = child.NextEdgeId++;
        }

        // If none of the states are marked as accept states in the child chromosome,
        // randomly select a state to mark as an accept state.
        if (!child.States.Any(s => s.IsAccept))
            child.States[_rnd.GetInt(0, child.States.Count)].IsAccept = true;

        return child;
    }

    /// <summary>
    /// Utilizes breadth first search to select half of the consecutive states of a chromosome.
    /// </summary>
    /// <param name="chromosome">A DFA Chromosome, whose states will be searched and selected.</param>
    /// <returns>A list, containing half the chromosome's consecutive states.</returns>
    private List<DFAState> SelectStatesBreadthFirst(DFAChromosome chromosome)
    {
        List<DFAState> selectedStates = new List<DFAState>() { chromosome.StartState! };
        Queue<DFAState> queue = new Queue<DFAState>();
        queue.Enqueue(chromosome.StartState!);

        // Return the start state if there are 3 or less states, since we can in those cases at most select one state.
        if (chromosome.States.Count <= 3)
            return selectedStates;

        for (int i = 0; i < chromosome.States.Count && queue.Count != 0; i++)
        {
            DFAState state = queue.Dequeue();

            foreach (DFAEdge edge in chromosome.Edges)
            {
                if (edge.Source.Id == state.Id)
                {
                    if (!selectedStates.Contains(edge.Target))
                    {
                        queue.Enqueue(edge.Target);
                        selectedStates.Add(edge.Target);
                        // Return once half the state space has been selected.
                        if (selectedStates.Count >= Math.Floor((double) chromosome.States.Count / 2))
                            return selectedStates;
                    }
                }
            }
        }

        return selectedStates;
    }
}
