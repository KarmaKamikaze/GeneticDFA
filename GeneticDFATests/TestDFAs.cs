using System.Collections.Generic;
using GeneticDFA;

namespace GeneticDFATests;

public class TestDFAs
{
    public static readonly List<DFAStateModel> SmallDFAStates = new List<DFAStateModel>()
    {
        new DFAStateModel(1, false),
        new DFAStateModel(2, false),
        new DFAStateModel(3, true)
    };

    public static readonly List<DFAEdgeModel> SmallDFAEdges = new List<DFAEdgeModel>()
    {
        new DFAEdgeModel(1, SmallDFAStates[0], SmallDFAStates[0], '0'),
        new DFAEdgeModel(2, SmallDFAStates[0], SmallDFAStates[1], '1'),
        new DFAEdgeModel(3, SmallDFAStates[1], SmallDFAStates[0], '0'),
        new DFAEdgeModel(4, SmallDFAStates[1], SmallDFAStates[2], '1'),
        new DFAEdgeModel(5, SmallDFAStates[2], SmallDFAStates[0], '1'),
        new DFAEdgeModel(6, SmallDFAStates[2], SmallDFAStates[0], '0'),
    };

    public static readonly DFAChromosome
        SmallDFA = new DFAChromosome(SmallDFAStates, SmallDFAEdges, SmallDFAStates[0]);
    
    public static readonly List<DFAStateModel> NFAStates = new List<DFAStateModel>()
    {
        new DFAStateModel(1, false),
        new DFAStateModel(2, false),
        new DFAStateModel(3, true),
        new DFAStateModel(4, false),
    };
    
    public static readonly List<DFAEdgeModel> NFAEdges = new List<DFAEdgeModel>()
    {
        new DFAEdgeModel(1, NFAStates[0], NFAStates[1], '1'),
        new DFAEdgeModel(2, NFAStates[0], NFAStates[3], '0'),
        new DFAEdgeModel(3, NFAStates[1], NFAStates[1], '0'),
        new DFAEdgeModel(4, NFAStates[1], NFAStates[2], '0'),
        new DFAEdgeModel(5, NFAStates[2], NFAStates[3], '1'),
        new DFAEdgeModel(7, NFAStates[3], NFAStates[0], '1'),
        new DFAEdgeModel(8, NFAStates[3], NFAStates[1], '0'),
        new DFAEdgeModel(9, NFAStates[3], NFAStates[2], '1'),
    }; 
    
    public static readonly DFAChromosome NFA = new DFAChromosome(NFAStates, NFAEdges, NFAStates[0]);

}
