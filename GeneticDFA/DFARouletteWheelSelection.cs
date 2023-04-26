using GeneticSharp;

namespace GeneticDFA;

public class DFARouletteWheelSelection : RouletteWheelSelection
{
    public static IList<IChromosome> SelectChromosomes(int numberOfSelections, IList<IChromosome> chromosomes)
    {
        List<double> rouletteWheel = new List<double>();
        IRandomization rnd = RandomizationProvider.Current;
        CalculateCumulativePercentFitness(chromosomes, rouletteWheel);
        return SelectFromWheel(numberOfSelections, chromosomes, rouletteWheel, () => rnd.GetDouble());
    }
}
