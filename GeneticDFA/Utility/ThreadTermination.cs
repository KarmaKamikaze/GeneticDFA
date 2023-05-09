using GeneticSharp;

namespace GeneticDFA.Utility;

public class ThreadTermination : TerminationBase
{
    public bool KillThread { get; set; }

    protected override bool PerformHasReached(IGeneticAlgorithm geneticAlgorithm)
    {
        return KillThread;
    }
}
