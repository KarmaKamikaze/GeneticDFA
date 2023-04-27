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

    /// <summary>
    /// Selects a mutation method from the roulette wheel using a pointer.
    /// </summary>
    /// <param name="rouletteWheel">A list of doubles that relates to the cumulative probability
    /// of each mutation method.</param>
    /// <param name="getPointer">A function delegate that handles the selection on the wheel.</param>
    /// <returns>The index in the wheel for the selected mutation method.</returns>
    public static int SelectMutationFromWheel(IEnumerable<double> rouletteWheel, Func<double> getPointer)
    {
        double pointer = getPointer();
        // Project the selection from the wheel into a (Value, Index) object.
        var choice = rouletteWheel.Select((value, index) => new
        {
            Value = value,
            Index = index
        }).FirstOrDefault(result => result.Value >= pointer); // Find the closest method to the pointer.

        // Return the index of the closest method to the pointer from the wheel.
        return choice!.Index;
    }

    /// <summary>
    /// Sets up the roulette wheel of mutation methods, spacing the options out according
    /// to their cumulative probabilities.
    /// </summary>
    /// <param name="mutationOperatorProbabilities">A list of all mutation method probabilities.</param>
    /// <param name="rouletteWheel">The roulette wheel collection which will be populated with the
    /// cumulative distribution of mutation methods.</param>
    public static void CalculateCumulativePercentMutation(
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
}
