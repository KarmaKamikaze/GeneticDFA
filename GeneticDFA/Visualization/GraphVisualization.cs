using System.Data;
using Rubjerg.Graphviz;

namespace GeneticDFA.Visualization;

public class GraphVisualization
{
    private readonly DFAChromosome _chromosome;
    private readonly int _generationNumber;
    private List<Node> _nodes = new();
    private List<Edge> _edges = new();

    public GraphVisualization(DFAChromosome chromosome, int generationNumber)
    {
        _chromosome = chromosome;
        _generationNumber = generationNumber;
    }

    /// <summary>
    /// Constructs a graph from the given chromosome's states and edges.
    /// </summary>
    private void ConstructGraph()
    {
        // Construct the graph root (this is not a node)
        RootGraph root = RootGraph.CreateNew($"Gen {_generationNumber}", GraphType.Directed);

        // Make each state into a node
        foreach (DFAState state in _chromosome.States)
        {
            // The node names are unique identifiers
            _nodes.Add(root.GetOrAddNode(state.ID.ToString()));
        }
    }
}
