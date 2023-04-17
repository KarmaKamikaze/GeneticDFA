using Rubjerg.Graphviz;

namespace GeneticDFA.Visualization;

public static class GraphVisualization
{
    /// <summary>
    /// Constructs a graph from the given chromosome's states and edges.
    /// </summary>
    /// <returns>A graph root object, containing the constructed graph.</returns>
    private static RootGraph ConstructGraph(DFAChromosome chromosome, int generationNumber)
    {
        // Construct the graph root (this is not a node)
        RootGraph root = RootGraph.CreateNew($"Gen{generationNumber}", GraphType.Directed);
        // Introduce new input attribute to edges and make the default value empty
        Edge.IntroduceAttribute(root, "label", "");

        // Make each state into a node
        foreach (DFAState state in chromosome.States)
        {
            // The node names are unique identifiers
            root.GetOrAddNode(state.ID.ToString());
        }

        // Add edges between states
        foreach (DFAEdge edge in chromosome.Edges)
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
    public static void SaveToDotFile(DFAChromosome chromosome, int generationNumber)
    {
        RootGraph graph = ConstructGraph(chromosome, generationNumber);
        graph.ToDotFile($"./{graph.GetName()}.dot");
    }

    /// <summary>
    /// Saves a visualization of the graph to a file in SVG format.
    /// </summary>
    public static void SaveToSvgFile(DFAChromosome chromosome, int generationNumber)
    {
        RootGraph graph = ConstructGraph(chromosome, generationNumber);
        graph.ComputeLayout(); // default is 'dot' layout
        graph.ToSvgFile($"./{graph.GetName()}.svg");
    }
}
