using GeneticSharp;

namespace GeneticDFA;

public class DFAChromosome : ChromosomeBase
{
    private readonly int _chromosomeBase;
    public DFAChromosome(int length) : base(length)
    {
        _chromosomeBase = length;
        CreateGenes();
    }

    public List<DFAStateModel> States = new List<DFAStateModel>();
    public List<DFAEdgeModel> Edges = new List<DFAEdgeModel>();
    public int StartStateID;
    
    public override Gene GenerateGene(int geneIndex)
    {
        throw new NotImplementedException("// TODO: Generate a gene base on DFAChromosome representation.");
    }

    public override IChromosome CreateNew()
    {
        return new DFAChromosome(_chromosomeBase);
    }
}
