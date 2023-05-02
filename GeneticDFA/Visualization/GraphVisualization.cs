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
        // Introduce shape specification attribute to states to indicate accept states and start state
        Node.IntroduceAttribute(root, "shape", "circle");
        // Introduce new input attribute to edges and make the default value empty
        Edge.IntroduceAttribute(root, "label", "");

        // Make each state into a node
        foreach (DFAState state in chromosome.States)
        {
            // The node names are unique identifiers
            Node? newNode = root.GetOrAddNode(state.Id.ToString());

            if (state.IsAccept)
                newNode.SetAttribute("shape", "doublecircle");
            if (state.Id == chromosome.StartState!.Id)
            {
                Node? startState = root.GetOrAddNode("S");
                startState.SetAttribute("shape", "point");
                root.GetOrAddEdge(startState, newNode, "");
            }
        }

        // Add edges between states
        foreach (DFAEdge edge in chromosome.Edges)
        {
            Node? source = root.GetNode(edge.Source.Id.ToString());
            Node? target = root.GetNode(edge.Target.Id.ToString());
            // An edge name is only unique between two nodes
            Edge? newEdge = root.GetOrAddEdge(source, target, edge.Id.ToString());
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
        graph.ToSvgFile($"./Visualizations/{graph.GetName()}.svg");
    }

    /// <summary>
    /// Deletes a folder and all files within if specified.
    /// </summary>
    /// <param name="path">The path to the folder which must be deleted.</param>
    /// <param name="recursive">Determines if all files inside will be recursively deleted.</param>
    public static void DeleteFolderRecursive(string path, bool recursive)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, recursive);
    }
}
