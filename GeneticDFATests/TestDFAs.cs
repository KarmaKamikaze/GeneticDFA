using System.Collections.Generic;
using GeneticDFA;

namespace GeneticDFATests;

public class TestDFAs
{
    public static readonly List<DFAState> SmallDFAStates = new List<DFAState>()
    {
        new DFAState(1, false),
        new DFAState(2, false),
        new DFAState(3, true)
    };

    public static readonly List<DFAEdge> SmallDFAEdges = new List<DFAEdge>()
    {
        new DFAEdge(1, SmallDFAStates[0], SmallDFAStates[0], '0'),
        new DFAEdge(2, SmallDFAStates[0], SmallDFAStates[1], '1'),
        new DFAEdge(3, SmallDFAStates[1], SmallDFAStates[0], '0'),
        new DFAEdge(4, SmallDFAStates[1], SmallDFAStates[2], '1'),
        new DFAEdge(5, SmallDFAStates[2], SmallDFAStates[0], '1'),
        new DFAEdge(6, SmallDFAStates[2], SmallDFAStates[0], '0'),
    };

    public static readonly DFAChromosome
        SmallDFA = new DFAChromosome(SmallDFAStates, SmallDFAEdges, SmallDFAStates[0]);

    public static readonly List<DFAState> NFAStates = new List<DFAState>()
    {
        new DFAState(1, false),
        new DFAState(2, false),
        new DFAState(3, true),
        new DFAState(4, false),
        new DFAState(5, false),
    };

    public static readonly List<DFAEdge> NFAEdges = new List<DFAEdge>()
    {
        new DFAEdge(1, NFAStates[0], NFAStates[1], '1'),
        new DFAEdge(2, NFAStates[0], NFAStates[3], '0'),
        new DFAEdge(3, NFAStates[1], NFAStates[1], '0'),
        new DFAEdge(4, NFAStates[1], NFAStates[2], '0'),
        new DFAEdge(5, NFAStates[2], NFAStates[3], '1'),
        new DFAEdge(6, NFAStates[3], NFAStates[0], '1'),
        new DFAEdge(7, NFAStates[3], NFAStates[1], '0'),
        new DFAEdge(8, NFAStates[3], NFAStates[2], '1'),
    };

    public static readonly DFAChromosome NFA = new DFAChromosome(NFAStates, NFAEdges, NFAStates[0]);
}
