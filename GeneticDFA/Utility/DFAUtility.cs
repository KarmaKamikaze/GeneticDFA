using Newtonsoft.Json;

namespace GeneticDFA.Utility;

public static class DFAUtility
{
    /// <summary>
    /// Imports test traces from a json file.
    /// </summary>
    /// <param name="filepath">The path to a json file containing the test traces.</param>
    /// <returns>A list of deserialized test traces.</returns>
    /// <exception cref="InvalidOperationException">An error occurs if no test trace file was found or if it is empty.</exception>
    public static List<TestTrace> ImportTestTraces(string filepath)
    {
        using StreamReader data = File.OpenText(filepath);
        string json = data.ReadToEnd();
        dynamic traces = JsonConvert.DeserializeObject(json) ??
                         throw new InvalidOperationException("No trace file or trace file is empty.");

        List<TestTrace> result = new List<TestTrace>();
        foreach (var trace in traces.PASSED)
        {
            result.Add(new TestTrace((string) trace, true));
        }

        foreach (var trace in traces.FAILED)
        {
            result.Add(new TestTrace((string) trace, false));
        }

        return result;
    }

    /// <summary>
    /// Goes through the test traces and finds all unique characters.
    /// </summary>
    /// <param name="testTraces">A list of test traces, from which we will discover the alphabet.</param>
    /// <returns>A list of characters, which constitutes our alphabet for the particular traces.</returns>
    public static IEnumerable<char> DiscoverAlphabet(List<TestTrace> testTraces)
    {
        string uniqueCharacters = String.Empty;
        // We only want to extract the alphabet from passing traces
        foreach (TestTrace trace in testTraces.Where(t => t.IsAccepting).ToList())
        {
            // Concatenate all unique singular trace characters (will contain duplicates)
            uniqueCharacters += String.Join("", trace.Trace.Distinct());
        }

        // Remove duplicates and return
        return uniqueCharacters.Distinct();
    }
}
