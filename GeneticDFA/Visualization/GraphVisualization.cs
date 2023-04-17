using Rubjerg.Graphviz;

namespace GeneticDFA.Visualization;

public class GraphVisualization
{
    private readonly DFAChromosome _chromosome;
    private readonly int _generationNumber;
    private readonly RootGraph _graph;

    public GraphVisualization(DFAChromosome chromosome, int generationNumber)
    {
        _chromosome = chromosome;
        _generationNumber = generationNumber;
        _graph = ConstructGraph();
    }

    /// <summary>
    /// Constructs a graph from the given chromosome's states and edges.
    /// </summary>
    /// <returns>A graph root object, containing the constructed graph.</returns>
    private RootGraph ConstructGraph()
    {
        // Construct the graph root (this is not a node)
        RootGraph root = RootGraph.CreateNew($"Gen{_generationNumber}", GraphType.Directed);
        // Introduce new input attribute to edges and make the default value empty
        Edge.IntroduceAttribute(root, "label", "");

        // Make each state into a node
        foreach (DFAState state in _chromosome.States)
        {
            // The node names are unique identifiers
            root.GetOrAddNode(state.ID.ToString());
        }

        // Add edges between states
        foreach (DFAEdge edge in _chromosome.Edges)
        {
            Node? source = root.GetNode(edge.Source.ID.ToString());
            Node? target = root.GetNode(edge.Target.ID.ToString());
            // An edge name is only unique between two nodes
            Edge? newEdge = root.GetOrAddEdge(source, target, edge.ID.ToString());
            // Set the input attribute
            newEdge.SetAttribute("label", edge.Input.ToString());
        }

        return root;
    }

    /// <summary>
    /// Saves the graph to a file in DOT format.
    /// </summary>
    public void SaveToDotFile()
    {
        _graph.ToDotFile($"./{_graph.GetName()}.dot");
    }

    /// <summary>
    /// Saves a visualization of the graph to a file in SVG format.
    /// </summary>
    public void SaveToSvgFile()
    {
        _graph.ComputeLayout(); // default is 'dot' layout
        _graph.ToSvgFile($"./{_graph.GetName()}.svg");
    }
}
