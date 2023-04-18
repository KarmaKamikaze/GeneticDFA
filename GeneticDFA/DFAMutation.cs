using GeneticSharp;

namespace GeneticDFA;

public class DFAMutation : MutationBase
{
    private readonly IRandomization _rnd = RandomizationProvider.Current;

    private double _changeInputProbability;
    private double _nonDeterministicBehaviorProbability;
    private double _changeTargetProbability;
    private double _changeSourceProbability;
    private double _removeEdgeProbability;
    private double _addEdgeProbability;
    private double _addStateProbability;
    private double _addAcceptStateProbability;
    private double _removeAcceptStateProbability;
    private double _mergeStatesProbability;
    private List<double> _mutationOperatorRouletteWheel = new List<double>();
    private readonly List<char> _alphabet = new List<char>();

    public DFAMutation(List<char> alphabet, double nonDeterministicBehaviorProbability, double changeTargetProbability, double changeSourceProbability, double removeEdgeProbability, double addEdgeProbability, double addStateProbability, double addAcceptStateProbability, double removeAcceptStateProbability, double mergeStatesProbability, double changeInputProbability)
    {
        _alphabet = alphabet;
        _nonDeterministicBehaviorProbability = nonDeterministicBehaviorProbability;
        _changeTargetProbability = changeTargetProbability;
        _changeSourceProbability = changeSourceProbability;
        _removeEdgeProbability = removeEdgeProbability;
        _addEdgeProbability = addEdgeProbability;
        _addStateProbability = addStateProbability;
        _addAcceptStateProbability = addAcceptStateProbability;
        _removeAcceptStateProbability = removeAcceptStateProbability;
        _mergeStatesProbability = mergeStatesProbability;
        _changeInputProbability = changeInputProbability;

        List<double> mutationOperatorProbabilities = new List<double>()
        {nonDeterministicBehaviorProbability, changeTargetProbability, changeTargetProbability, removeEdgeProbability,
            addEdgeProbability,
            addStateProbability,
            addAcceptStateProbability,
            removeAcceptStateProbability,
            mergeStatesProbability
        };
        CalculateCumulativePercentMutation(mutationOperatorProbabilities, _mutationOperatorRouletteWheel);

    }

    private int SelectMutationFromWheel(IList<double> rouletteWheel, Func<double> getPointer)
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
        bool focusOnNonDeterministicBehavior = _rnd.GetDouble(0, 1) < _nonDeterministicBehaviorProbability;

        DFAChromosome clone = (DFAChromosome) chromosome.Clone();

        switch(SelectMutationFromWheel(_mutationOperatorRouletteWheel, () => _rnd.GetDouble()))
        {
            case MutationOperator.ChangeInputProbability:
                break;
            case MutationOperator.ChangeTargetProbability:
                break;
            case MutationOperator.ChangeSourceProbability:
                break;
            case MutationOperator.RemoveEdgeProbability:
                break;
            case MutationOperator.AddEdgeProbability:
                break;
            case MutationOperator.AddStateProbability:
                break;
            case MutationOperator.AddAcceptStateProbability:
                break;
            case MutationOperator.RemoveAcceptStateProbability:
                break;
            case MutationOperator.MergeStatesProbability:
                break;
            default:
                throw new ArgumentException("Probability selection error.");
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
        List<DFAEdge> possibleEdgesToSelect = FindPossibleEdges(chromosome, nonDeterminism);
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
            possibleEdgesToSelect = FindPossibleEdges(chromosome, !nonDeterminism);
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
        List<DFAEdge> possibleEdges = FindPossibleEdges(chromosome, nonDeterminism);
        DFAEdge edge = possibleEdges[_rnd.GetInt(0, possibleEdges.Count)];
        chromosome.Edges.Remove(edge);
        return true;
    }

    private bool AddEdge(DFAChromosome chromosome, bool nonDeterminism)
    {
        return true;
    }

    private static List<DFAEdge> FindPossibleEdges(DFAChromosome chromosome, bool nonDeterminism)
    {
        List<DFAEdge> possibleEdgesToSelect;
        if (nonDeterminism)
            possibleEdgesToSelect = chromosome.NonDeterministicEdges;
        else
            possibleEdgesToSelect = chromosome.Edges.Where(e => !chromosome.NonDeterministicEdges.Contains(e)).ToList();

        if (possibleEdgesToSelect.Count == 0)
            possibleEdgesToSelect = chromosome.Edges;

        return possibleEdgesToSelect;
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
