using GeneticSharp;

namespace GeneticDFA;

public class DFACrossover : CrossoverBase
{
    public DFACrossover(int parentsNumber, int childrenNumber, int minChromosomeLength) : base(parentsNumber, childrenNumber, minChromosomeLength)
    {
    }

    protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
    {
        throw new NotImplementedException();
    }
}
