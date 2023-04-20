using GeneticSharp;

namespace GeneticDFA;

public class DFAMutation : MutationBase
{
    private readonly IRandomization _rnd = RandomizationProvider.Current;

    private readonly double _nonDeterministicBehaviorProbability;

    private readonly List<double> _mutationOperatorRouletteWheel = new List<double>();
    private readonly List<char> _alphabet;

    public DFAMutation(List<char> alphabet, double nonDeterministicBehaviorProbability, double changeTargetProbability, double changeSourceProbability, double removeEdgeProbability, double addEdgeProbability, double addStateProbability, double addAcceptStateProbability, double removeAcceptStateProbability, double mergeStatesProbability, double changeInputProbability)
    {
        _alphabet = alphabet;
        _nonDeterministicBehaviorProbability = nonDeterministicBehaviorProbability;

        List<double> mutationOperatorProbabilities = new List<double>()
        { changeTargetProbability, changeSourceProbability, changeInputProbability,
            removeEdgeProbability,
            addEdgeProbability,
            addStateProbability,
            addAcceptStateProbability,
            removeAcceptStateProbability,
            mergeStatesProbability
        };
        CalculateCumulativePercentMutation(mutationOperatorProbabilities, _mutationOperatorRouletteWheel);
    }

    private static int SelectMutationFromWheel(IEnumerable<double> rouletteWheel, Func<double> getPointer)
    {
        double pointer = getPointer();
        var choice = rouletteWheel.Select((value, index) => new
        {
            Value = value,
            Index = index
        }).FirstOrDefault(result => result.Value >= pointer);

        return choice!.Index;
    }

    private static void CalculateCumulativePercentMutation(
      IList<double> mutationOperatorProbabilities,
      ICollection<double> rouletteWheel)
    {
        double num1 = mutationOperatorProbabilities.Sum();
        double num2 = 0.0;
        foreach (double t in mutationOperatorProbabilities)
        {
            num2 += t / num1;
            rouletteWheel.Add(num2);
        }
    }

    protected override void PerformMutate(IChromosome chromosome, float probability)
    {
        DFAChromosome _chromosome = (DFAChromosome) chromosome;
        bool mutationApplied = false;
        Dictionary<MutationOperator, bool> mutationOperatorTried = new Dictionary<MutationOperator, bool>();
        foreach (MutationOperator op in (MutationOperator[]) Enum.GetValues(typeof(MutationOperator)))
        {
            mutationOperatorTried.Add(op, false);
        }

        bool focusOnNonDeterministicBehavior = _rnd.GetDouble(0, 1) < _nonDeterministicBehaviorProbability;

        while (!mutationApplied)
        {
            switch((MutationOperator) SelectMutationFromWheel(_mutationOperatorRouletteWheel, () => _rnd.GetDouble()))
            {
                case MutationOperator.ChangeInputProbability:
                    if (!mutationOperatorTried[MutationOperator.ChangeInputProbability])
                    {
                        mutationApplied = EdgeModification(_chromosome, focusOnNonDeterministicBehavior, ChangeInput);
                        if (mutationApplied)
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                        mutationOperatorTried[MutationOperator.ChangeInputProbability] = true;
                    }
                    break;
                case MutationOperator.ChangeTargetProbability:
                    if (!mutationOperatorTried[MutationOperator.ChangeTargetProbability])
                    {
                        mutationApplied = EdgeModification(_chromosome, focusOnNonDeterministicBehavior, ChangeTarget);
                        if (mutationApplied)
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                        mutationOperatorTried[MutationOperator.ChangeTargetProbability] = true;
                    }
                    break;
                case MutationOperator.ChangeSourceProbability:
                    if (!mutationOperatorTried[MutationOperator.ChangeSourceProbability])
                    {
                        mutationApplied = EdgeModification(_chromosome, focusOnNonDeterministicBehavior, ChangeSource);
                        if (mutationApplied)
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                        mutationOperatorTried[MutationOperator.ChangeSourceProbability] = true;
                    }
                    break;
                case MutationOperator.RemoveEdgeProbability:
                    if (!mutationOperatorTried[MutationOperator.RemoveEdgeProbability])
                    {
                        mutationApplied = RemoveEdge(_chromosome, focusOnNonDeterministicBehavior);
                        if (mutationApplied)
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                        mutationOperatorTried[MutationOperator.RemoveEdgeProbability] = true;
                    }
                    break;
                case MutationOperator.AddEdgeProbability:
                    if (!mutationOperatorTried[MutationOperator.AddEdgeProbability])
                    {
                        mutationApplied = AddEdge(_chromosome);
                        if (mutationApplied)
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                        mutationOperatorTried[MutationOperator.AddEdgeProbability] = true;
                    }
                    break;
                case MutationOperator.AddStateProbability:
                    if (!mutationOperatorTried[MutationOperator.AddStateProbability])
                    {
                        mutationApplied = AddState(_chromosome);
                        if (mutationApplied)
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                        mutationOperatorTried[MutationOperator.AddStateProbability] = true;
                    }
                    break;
                case MutationOperator.AddAcceptStateProbability:
                    if (!mutationOperatorTried[MutationOperator.AddAcceptStateProbability])
                    {
                        mutationApplied = AddAcceptState(_chromosome);
                        mutationOperatorTried[MutationOperator.AddAcceptStateProbability] = true;
                    }
                    break;
                case MutationOperator.RemoveAcceptStateProbability:
                    if (!mutationOperatorTried[MutationOperator.RemoveAcceptStateProbability])
                    {
                        mutationApplied = RemoveAcceptState(_chromosome);
                        mutationOperatorTried[MutationOperator.RemoveAcceptStateProbability] = true;
                    }
                    break;
                case MutationOperator.MergeStatesProbability:
                    if (!mutationOperatorTried[MutationOperator.MergeStatesProbability])
                    {
                        mutationApplied = MergeStates(_chromosome, focusOnNonDeterministicBehavior);
                        if (mutationApplied)
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                        mutationOperatorTried[MutationOperator.MergeStatesProbability] = true;
                    }
                    break;
                default:
                    throw new ArgumentException("Probability selection error.");
            }
        }
    }

    private void ShuffleList<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = _rnd.GetInt(0, list.Count);
            (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
        }
    }

    private bool EdgeModification(DFAChromosome chromosome, bool nonDeterminism,
        Func<DFAChromosome, DFAEdge, bool> mutationOperator)
    {
        List<DFAEdge> possibleEdgesToSelect = ChooseSetOfEdges(chromosome, nonDeterminism);
        ShuffleList(possibleEdgesToSelect);

        foreach (DFAEdge edge in possibleEdgesToSelect)
        {
            if (mutationOperator(chromosome, edge))
            {
                return true;
            }
        }

        if (possibleEdgesToSelect.Count != chromosome.Edges.Count)
        {
            possibleEdgesToSelect = ChooseSetOfEdges(chromosome, !nonDeterminism);
            ShuffleList(possibleEdgesToSelect);

            foreach (DFAEdge edge in possibleEdgesToSelect)
            {
                if (mutationOperator(chromosome, edge))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool ChangeSource(DFAChromosome chromosome, DFAEdge edge)
    {
        List<DFAEdge> edgesWithSameTargetAndInput =
            chromosome.Edges.Where(e => e.Input == edge.Input && e.Target == edge.Target).ToList();
        List<DFAState> possibleSources = chromosome.States.Where(s => edgesWithSameTargetAndInput.All(e => e.Source != s)).ToList();
        if (possibleSources.Count == 0)
            return false;
        DFAState source = possibleSources[_rnd.GetInt(0, possibleSources.Count)];
        edge.Source = source;
        return true;
    }

    private bool ChangeTarget(DFAChromosome chromosome, DFAEdge edge)
    {
        List<DFAEdge> edgesWithSameSourceAndInput =
            chromosome.Edges.Where(e => e.Source == edge.Source && e.Input == edge.Input).ToList();
        List<DFAState> possibleTargets = chromosome.States.Where(s => edgesWithSameSourceAndInput.All(e => e.Target != s)).ToList();
        if (possibleTargets.Count == 0)
            return false;
        DFAState target = possibleTargets[_rnd.GetInt(0, possibleTargets.Count)];
        edge.Target = target;
        return true;
    }

    private bool ChangeInput(DFAChromosome chromosome, DFAEdge edge)
    {
        List<DFAEdge> edgesWithSameSourceAndTarget =
            chromosome.Edges.Where(e => e.Source == edge.Source && e.Target == edge.Target).ToList();
        List<char> possibleInputs = _alphabet.Where(i => edgesWithSameSourceAndTarget.All(e => e.Input != i)).ToList();
        if (possibleInputs.Count == 0)
            return false;
        char input = possibleInputs[_rnd.GetInt(0, possibleInputs.Count)];
        edge.Input = input;
        return true;
    }


    private bool RemoveEdge(DFAChromosome chromosome, bool nonDeterminism)
    {
        List<DFAEdge> possibleEdges = ChooseSetOfEdges(chromosome, nonDeterminism);
        if (possibleEdges.Count == 0)
            return false;
        DFAEdge edge = possibleEdges[_rnd.GetInt(0, possibleEdges.Count)];
        chromosome.Edges.Remove(edge);
        return true;
    }

    private bool AddEdge(DFAChromosome chromosome)
    {
        List<DFAState> possibleStates = chromosome.States.Where(s => chromosome.Edges.Count(e => e.Source == s) < chromosome.States.Count*_alphabet.Count).ToList();
        if (possibleStates.Count == 0)
            return false;
        DFAState source = possibleStates[_rnd.GetInt(0, possibleStates.Count)];
        List<DFAEdge> existingEdgesWithCurrentSource = chromosome.Edges.Where(e => e.Source == source).ToList();

        List<char> possibleInputs = _alphabet.Where(i => existingEdgesWithCurrentSource.Count(
            e => e.Input == i) < chromosome.States.Count).ToList();
        char input = possibleInputs[_rnd.GetInt(0, possibleInputs.Count)];
        List<DFAEdge> existingEdgesWithCurrentSourceAndInput =
            existingEdgesWithCurrentSource.Where(e => e.Input == input).ToList();

        List<DFAState> possibleTargets = chromosome.States
            .Where(s => existingEdgesWithCurrentSourceAndInput.All(e => e.Target != s)).ToList();
        DFAState target = possibleTargets[_rnd.GetInt(0, possibleTargets.Count)];

        chromosome.Edges.Add(new DFAEdge(chromosome.NextEdgeId, source, target, input));
        chromosome.NextEdgeId++;
        return true;
    }


    private static List<DFAState> ChooseSetOfStates(DFAChromosome chromosome, bool nonDeterminism)
    {
        List<DFAEdge> edges = ChooseSetOfEdges(chromosome, nonDeterminism);
        return edges.Count == 0 ? chromosome.States : chromosome.States.Where(s => edges.Any(e => e.Source == s)).ToList();
    }

    private static List<DFAEdge> ChooseSetOfEdges(DFAChromosome chromosome, bool nonDeterminism)
    {
        List<DFAEdge> edges;
        if (nonDeterminism)
            edges = chromosome.NonDeterministicEdges;
        else
            edges = chromosome.Edges.Where(e => !chromosome.NonDeterministicEdges.Contains(e)).ToList();

        if (edges.Count == 0)
            edges = chromosome.Edges;

        return edges;
    }

    private bool AddAcceptState(DFAChromosome chromosome)
    {
        if (chromosome.States.Count == 1)
            return false;

        List<DFAState> nonAcceptStates = chromosome.States.Where(s => s.IsAccept == false).ToList();
        if (nonAcceptStates.Count == 0)
            return false;
        nonAcceptStates[_rnd.GetInt(0, nonAcceptStates.Count)].IsAccept = true;

        return true;
    }

    private bool RemoveAcceptState(DFAChromosome chromosome)
    {
        if (chromosome.States.Count == 1)
            return false;

        List<DFAState> acceptStates = chromosome.States.Where(s => s.IsAccept).ToList();
        if(acceptStates.Count == 1)
        {
            List<DFAState> nonAcceptStates = chromosome.States.Where(s => s.IsAccept == false).ToList();
            nonAcceptStates[_rnd.GetInt(0, nonAcceptStates.Count)].IsAccept = true;
        }

        acceptStates[_rnd.GetInt(0, acceptStates.Count)].IsAccept = false;

        return true;
    }

    private bool AddState(DFAChromosome chromosome)
    {
        // Create new state
        DFAState newState = new DFAState(chromosome.NextStateId++, false);
        // Connect an exiting (source) state with the new (target) state
        DFAState firstSource = chromosome.States[_rnd.GetInt(0, chromosome.States.Count)];
        chromosome.Edges.Add(new DFAEdge(chromosome.NextEdgeId++, firstSource, newState,
            _alphabet[_rnd.GetInt(0, _alphabet.Count)]));

        // Add new state to collection. Adding the state at this point ensures that our first edge cannot self-loop and
        // that there will always be an incoming connection. Hereafter, the second connection can be a self-loop.
        chromosome.States.Add(newState);

        // Decide if newState will have a possible outgoing or another incoming edge
        bool outgoingEdge = _rnd.GetInt(0, 1) != 0;
        if (outgoingEdge)
        {
            chromosome.Edges.Add(new DFAEdge(chromosome.NextEdgeId++,
                newState, chromosome.States[_rnd.GetInt(0, chromosome.States.Count)],
                _alphabet[_rnd.GetInt(0, _alphabet.Count)]));
        }
        else
        {
            // Find another state than firstSource
            List<DFAState> possibleStates = chromosome.States.Where(s => s.Id != firstSource.Id).ToList();
            DFAState secondSource = possibleStates[_rnd.GetInt(0, possibleStates.Count)];
            chromosome.Edges.Add(new DFAEdge(chromosome.NextEdgeId++, secondSource, newState,
                _alphabet[_rnd.GetInt(0, _alphabet.Count)]));
        }

        return true;
    }

    private bool MergeStates(DFAChromosome chromosome, bool nonDeterminism)
    {
        if (chromosome.States.Count < 2)
            return false;

        List<DFAState> states = ChooseSetOfStates(chromosome, nonDeterminism);
        DFAState state1 = states[_rnd.GetInt(0, states.Count)];
        DFAState state2 = chromosome.States.Where(s => s != state1).ToList()[_rnd.GetInt(0, chromosome.States.Count-1)];

        state1.IsAccept = state1.IsAccept || state2.IsAccept;
        if (chromosome.StartState == state2)
            chromosome.StartState = state1;

        List<DFAEdge> ingoingEdgesState2 = chromosome.Edges.Where(e => e.Target == state2).ToList();

        foreach (DFAEdge edge in ingoingEdgesState2)
        {
            edge.Target = state1;
        }

        List<DFAEdge> outgoingEdgesState2 = chromosome.Edges.Where(e => e.Source == state2).ToList();

        foreach (DFAEdge edge in outgoingEdgesState2)
        {
            edge.Source = state1;
        }

        chromosome.Edges.Sort(delegate(DFAEdge edge1, DFAEdge edge2)
        {
            int areSourcesEqual = edge1.Source.Id.CompareTo(edge2.Source.Id);
            if (areSourcesEqual != 0)
                return areSourcesEqual;
            int isInputEqual = edge1.Input.CompareTo(edge2.Input);
            return isInputEqual != 0 ? isInputEqual : edge1.Target.Id.CompareTo(edge2.Target.Id);
        });

        for (int i = 0; i < chromosome.Edges.Count-1; i++)
        {
            if (chromosome.Edges[i].Source.Id == chromosome.Edges[i + 1].Source.Id
                && chromosome.Edges[i].Input == chromosome.Edges[i + 1].Input
                && chromosome.Edges[i].Target.Id == chromosome.Edges[i + 1].Target.Id)
                chromosome.Edges.Remove(chromosome.Edges[i]);
        }

        return true;
    }

    private enum MutationOperator
    {
        ChangeInputProbability,
        ChangeTargetProbability,
        ChangeSourceProbability,
        RemoveEdgeProbability,
        AddEdgeProbability,
        AddStateProbability,
        AddAcceptStateProbability,
        RemoveAcceptStateProbability,
        MergeStatesProbability
    }
}
