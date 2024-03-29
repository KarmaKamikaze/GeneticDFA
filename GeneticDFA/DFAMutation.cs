﻿using GeneticSharp;

namespace GeneticDFA;

public class DFAMutation : MutationBase
{
    private readonly IRandomization _rnd = RandomizationProvider.Current;

    private readonly double _nonDeterministicBehaviorProbability;

    private readonly List<double> _mutationOperatorRouletteWheel = new List<double>();
    private readonly List<char> _alphabet;

    public DFAMutation(List<char> alphabet, double nonDeterministicBehaviorProbability, double changeTargetProbability,
        double changeSourceProbability, double removeEdgeProbability, double addEdgeProbability,
        double addStateProbability, double addAcceptStateProbability, double removeAcceptStateProbability,
        double mergeStatesProbability, double changeInputProbability)
    {
        _alphabet = alphabet;
        _nonDeterministicBehaviorProbability = nonDeterministicBehaviorProbability;

        // Setup the probabilities in a list
        List<double> mutationOperatorProbabilities = new List<double>()
        {
            changeInputProbability,
            changeTargetProbability,
            changeSourceProbability,
            removeEdgeProbability,
            addEdgeProbability,
            addStateProbability,
            addAcceptStateProbability,
            removeAcceptStateProbability,
            mergeStatesProbability
        };
        DFARouletteWheelSelection.CalculateCumulativePercentMutation(mutationOperatorProbabilities,
            _mutationOperatorRouletteWheel);
    }

    protected override void PerformMutate(IChromosome chromosome, float probability)
    {
        DFAChromosome _chromosome = (DFAChromosome) chromosome;
        bool mutationApplied = false; // Flag used to determine whether a mutation has successfully been applied
        // Dictionary used to check whether a specific mutation operator has been tried.
        // Thus we avoid executing the operator again
        Dictionary<MutationOperator, bool> mutationOperatorTried = new Dictionary<MutationOperator, bool>();
        foreach (MutationOperator op in (MutationOperator[]) Enum.GetValues(typeof(MutationOperator)))
        {
            mutationOperatorTried.Add(op, false);
        }

        bool focusOnNonDeterministicBehavior = _rnd.GetDouble(0, 1) < _nonDeterministicBehaviorProbability;

        while (!mutationApplied)
        {
            switch ((MutationOperator) DFARouletteWheelSelection.SelectMutationFromWheel(_mutationOperatorRouletteWheel,
                        () => _rnd.GetDouble()))
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
                        {
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                            _chromosome.ReachableStates = DFAChromosomeHelper.FindReachableStates(_chromosome);
                        }

                        mutationOperatorTried[MutationOperator.ChangeTargetProbability] = true;
                    }

                    break;
                case MutationOperator.ChangeSourceProbability:
                    if (!mutationOperatorTried[MutationOperator.ChangeSourceProbability])
                    {
                        mutationApplied = EdgeModification(_chromosome, focusOnNonDeterministicBehavior, ChangeSource);
                        if (mutationApplied)
                        {
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                            _chromosome.ReachableStates = DFAChromosomeHelper.FindReachableStates(_chromosome);
                        }

                        mutationOperatorTried[MutationOperator.ChangeSourceProbability] = true;
                    }

                    break;
                case MutationOperator.RemoveEdgeProbability:
                    if (!mutationOperatorTried[MutationOperator.RemoveEdgeProbability])
                    {
                        mutationApplied = RemoveEdge(_chromosome, focusOnNonDeterministicBehavior);
                        if (mutationApplied)
                        {
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                            _chromosome.ReachableStates = DFAChromosomeHelper.FindReachableStates(_chromosome);
                        }

                        mutationOperatorTried[MutationOperator.RemoveEdgeProbability] = true;
                    }

                    break;
                case MutationOperator.AddEdgeProbability:
                    if (!mutationOperatorTried[MutationOperator.AddEdgeProbability])
                    {
                        mutationApplied = AddEdge(_chromosome);
                        if (mutationApplied)
                        {
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                            _chromosome.ReachableStates = DFAChromosomeHelper.FindReachableStates(_chromosome);
                        }

                        mutationOperatorTried[MutationOperator.AddEdgeProbability] = true;
                    }

                    break;
                case MutationOperator.AddStateProbability:
                    if (!mutationOperatorTried[MutationOperator.AddStateProbability])
                    {
                        mutationApplied = AddState(_chromosome);
                        if (mutationApplied)
                        {
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                            _chromosome.ReachableStates = DFAChromosomeHelper.FindReachableStates(_chromosome);
                        }

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
                        {
                            DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(_chromosome);
                            _chromosome.ReachableStates = DFAChromosomeHelper.FindReachableStates(_chromosome);
                        }

                        mutationOperatorTried[MutationOperator.MergeStatesProbability] = true;
                    }

                    break;
                default:
                    throw new ArgumentException("Probability selection error.");
            }
        }

        _chromosome.SetNewId();
    }

    // Randomly shuffles a list.
    // This allows for randomness while making sure that the same element can not be chosen more than once
    private void ShuffleList<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = _rnd.GetInt(0, list.Count);
            (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
        }
    }

    // Used for modifying edges based on its delegate parameter
    private bool EdgeModification(DFAChromosome chromosome, bool nonDeterminism,
        Func<DFAChromosome, DFAEdge, bool> mutationOperator)
    {
        List<DFAEdge> possibleEdgesToSelect = ChooseSetOfEdges(chromosome, nonDeterminism);
        ShuffleList(possibleEdgesToSelect);

        // Attempt to mutate each edge chosen based on the nonDeterminism flag
        foreach (DFAEdge edge in possibleEdgesToSelect)
        {
            if (mutationOperator(chromosome, edge))
            {
                return true;
            }
        }

        // If no mutation has occured, we try on the edges not selected earlier.
        // However, this only happens if the edges selected earlier were not all edges
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

    // Method used as delegate parameter to EdgeModification.
    private bool ChangeSource(DFAChromosome chromosome, DFAEdge edge)
    {
        // First find the sources that the edge could be changed to without causing duplicates
        List<DFAEdge> edgesWithSameTargetAndInput =
            chromosome.Edges.Where(e => e.Input == edge.Input && e.Target == edge.Target).ToList();
        List<DFAState> possibleSources = chromosome.States
            .Where(s => edgesWithSameTargetAndInput.All(e => e.Source.Id != s.Id)).ToList();

        // If no sources was found, return false and attempt changing source on another edge
        if (possibleSources.Count == 0)
            return false;
        DFAState source = possibleSources[_rnd.GetInt(0, possibleSources.Count)];
        edge.Source = source;
        return true;
    }

    // Method used as delegate parameter to EdgeModification.
    private bool ChangeTarget(DFAChromosome chromosome, DFAEdge edge)
    {
        // First find the targets that the edge could be changed to without causing duplicates
        List<DFAEdge> edgesWithSameSourceAndInput =
            chromosome.Edges.Where(e => e.Source == edge.Source && e.Input == edge.Input).ToList();
        List<DFAState> possibleTargets = chromosome.States
            .Where(s => edgesWithSameSourceAndInput.All(e => e.Target.Id != s.Id)).ToList();

        // If no targets was found, return false and attempt changing target on another edge
        if (possibleTargets.Count == 0)
            return false;
        DFAState target = possibleTargets[_rnd.GetInt(0, possibleTargets.Count)];
        edge.Target = target;
        return true;
    }

    // Method used as delegate parameter to EdgeModification.
    private bool ChangeInput(DFAChromosome chromosome, DFAEdge edge)
    {
        // First find the inputs that the edge could be changed to without causing duplicates
        List<DFAEdge> edgesWithSameSourceAndTarget =
            chromosome.Edges.Where(e => e.Source == edge.Source && e.Target == edge.Target).ToList();
        List<char> possibleInputs = _alphabet.Where(i => edgesWithSameSourceAndTarget.All(e => e.Input != i)).ToList();

        // If no inputs was found, return false and attempt changing input on another edge
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
        // Only use reachable states as sources,
        // since there is no benefit to adding an outgoing edge from an unreachable state
        // If a state has outgoing edges to each state with each input symbol, it is not a valid source
        List<DFAState> possibleSources = chromosome.ReachableStates.Where(s =>
            chromosome.Edges.Count(e => e.Source == s) < chromosome.States.Count * _alphabet.Count).ToList();
        if (possibleSources.Count == 0)
            return false;
        DFAState source = possibleSources[_rnd.GetInt(0, possibleSources.Count)];
        List<DFAEdge> existingEdgesWithCurrentSource = chromosome.Edges.Where(e => e.Source == source).ToList();
        // Possible inputs are inputs where the state does not have a transition to every state.
        // This list is never empty, since we know that the source chosen is valid.
        // Thus there must be an input where the state does not have all possible edges
        List<char> possibleInputs = _alphabet.Where(i => existingEdgesWithCurrentSource.Count(
            e => e.Input == i) < chromosome.States.Count).ToList();
        char input = possibleInputs[_rnd.GetInt(0, possibleInputs.Count)];
        List<DFAEdge> existingEdgesWithCurrentSourceAndInput =
            existingEdgesWithCurrentSource.Where(e => e.Input == input).ToList();

        // Possible targets is also never empty due to same reasoning as for possible inputs
        List<DFAState> possibleTargets = chromosome.States
            .Where(s => existingEdgesWithCurrentSourceAndInput.All(e => e.Target != s)).ToList();
        DFAState target = possibleTargets[_rnd.GetInt(0, possibleTargets.Count)];

        chromosome.Edges.Add(new DFAEdge(chromosome.NextEdgeId, source, target, input));
        chromosome.NextEdgeId++;
        return true;
    }

    // Helper method used to choose the set of states to focus mutation on based on the nondeterminism flag
    private static List<DFAState> ChooseSetOfStates(DFAChromosome chromosome, bool nonDeterminism)
    {
        List<DFAEdge> edges = ChooseSetOfEdges(chromosome, nonDeterminism);
        return edges.Count == 0
            ? chromosome.States
            : chromosome.States.Where(s => edges.Any(e => e.Source == s)).ToList();
    }

    // Helper method used to choose the set of edges to focus mutation on based on the nondeterminism flag
    private static List<DFAEdge> ChooseSetOfEdges(DFAChromosome chromosome, bool nonDeterminism)
    {
        List<DFAEdge> edges;
        if (nonDeterminism)
            edges = chromosome.NonDeterministicEdges;
        else
            edges = chromosome.Edges.Where(e => !chromosome.NonDeterministicEdges.Contains(e)).ToList();

        // If the list is empty, we simply choose all edges.
        if (edges.Count == 0)
            edges = chromosome.Edges;

        return edges;
    }

    private bool AddAcceptState(DFAChromosome chromosome)
    {
        // As there must be at least one accept state, we can not add another one, if there is only one state
        if (chromosome.States.Count == 1)
            return false;

        // Only use reachable states, since there is no benefit to making an unreachable state an accept state
        List<DFAState> nonAcceptStates = chromosome.ReachableStates.Where(s => s.IsAccept == false).ToList();
        // If there is no states that can be made an accept state, return false
        if (nonAcceptStates.Count == 0)
            return false;
        nonAcceptStates[_rnd.GetInt(0, nonAcceptStates.Count)].IsAccept = true;

        return true;
    }

    private bool RemoveAcceptState(DFAChromosome chromosome)
    {
        // Since we must have at least one accept state, we return false if there is only one state
        if (chromosome.States.Count == 1)
            return false;

        List<DFAState> acceptStates = chromosome.States.Where(s => s.IsAccept).ToList();
        // If there is only one accept state, we first add an accept state before removing the original accept state
        if (acceptStates.Count == 1)
        {
            List<DFAState> nonAcceptStates = chromosome.ReachableStates.Where(s => s.IsAccept == false).ToList();
            if (nonAcceptStates.Count == 0)
                return false;
            nonAcceptStates[_rnd.GetInt(0, nonAcceptStates.Count)].IsAccept = true;
        }

        acceptStates[_rnd.GetInt(0, acceptStates.Count)].IsAccept = false;

        return true;
    }

    private bool AddState(DFAChromosome chromosome)
    {
        // Create new state
        DFAState newState = new DFAState(chromosome.NextStateId++, false);
        // Connect an exiting (source) state with the new (target) state.
        // Only use reachable states to ensure reachability
        DFAState firstSource = chromosome.ReachableStates[_rnd.GetInt(0, chromosome.ReachableStates.Count)];
        chromosome.Edges.Add(new DFAEdge(chromosome.NextEdgeId++, firstSource, newState,
            _alphabet[_rnd.GetInt(0, _alphabet.Count)]));

        // Add new state to collection. Adding the state at this point ensures that our first edge cannot self-loop and
        // that there will always be an incoming connection. Hereafter, the second connection can be a self-loop.
        chromosome.States.Add(newState);
        chromosome.ReachableStates.Add(newState);

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
            List<DFAState> possibleStates = chromosome.ReachableStates.Where(s => s.Id != firstSource.Id).ToList();
            DFAState secondSource = possibleStates[_rnd.GetInt(0, possibleStates.Count)];
            chromosome.Edges.Add(new DFAEdge(chromosome.NextEdgeId++, secondSource, newState,
                _alphabet[_rnd.GetInt(0, _alphabet.Count)]));
        }

        return true;
    }

    private bool MergeStates(DFAChromosome chromosome, bool nonDeterminism)
    {
        // Merging is not possible if there is less than two states
        if (chromosome.States.Count < 2)
            return false;

        List<DFAState> states = ChooseSetOfStates(chromosome, nonDeterminism);

        // Choose two states to merge. state2 will be merged into state1
        DFAState state1 = states[_rnd.GetInt(0, states.Count)];
        DFAState state2 =
            chromosome.States.Where(s => s != state1).ToList()[_rnd.GetInt(0, chromosome.States.Count - 1)];

        // Modify the properties of state1 based on state2
        state1.IsAccept = state1.IsAccept || state2.IsAccept;
        if (chromosome.StartState == state2)
            chromosome.StartState = state1;

        // Change the target of ingoing edges of state2 to be state1
        List<DFAEdge> ingoingEdgesState2 = chromosome.Edges.Where(e => e.Target == state2).ToList();

        foreach (DFAEdge edge in ingoingEdgesState2)
        {
            edge.Target = state1;
        }

        // Change the source of outgoing edges of state2 to be state1
        List<DFAEdge> outgoingEdgesState2 = chromosome.Edges.Where(e => e.Source == state2).ToList();

        foreach (DFAEdge edge in outgoingEdgesState2)
        {
            edge.Source = state1;
        }

        // Sort the edges so that duplicates are grouped
        chromosome.Edges.Sort(delegate(DFAEdge edge1, DFAEdge edge2)
        {
            int areSourcesEqual = edge1.Source.Id.CompareTo(edge2.Source.Id);
            if (areSourcesEqual != 0)
                return areSourcesEqual;
            int isInputEqual = edge1.Input.CompareTo(edge2.Input);
            return isInputEqual != 0 ? isInputEqual : edge1.Target.Id.CompareTo(edge2.Target.Id);
        });

        // Remove duplicates. Iterates backwards since removing an element from a list causes the elements ahead of
        // the list to move in order to fill the empty space.
        // By iterating backwards, only elements we have already analysed are moved.
        for (int i = chromosome.Edges.Count - 1; i >= 1; i--)
        {
            if (chromosome.Edges[i].Source.Id == chromosome.Edges[i - 1].Source.Id
                && chromosome.Edges[i].Input == chromosome.Edges[i - 1].Input
                && chromosome.Edges[i].Target.Id == chromosome.Edges[i - 1].Target.Id)
                chromosome.Edges.Remove(chromosome.Edges[i]);
        }

        chromosome.States.Remove(state2);

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
