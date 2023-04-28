using GeneticSharp;

namespace GeneticDFATests;

public class TestRandomization : IRandomization
{
    public int GetInt(int min, int max)
    {
        return min;
    }

    public int[] GetInts(int length, int min, int max)
    {
        throw new System.NotImplementedException();
    }

    public int[] GetUniqueInts(int length, int min, int max)
    {
        throw new System.NotImplementedException();
    }

    public float GetFloat()
    {
        throw new System.NotImplementedException();
    }

    public float GetFloat(float min, float max)
    {
        throw new System.NotImplementedException();
    }

    public double GetDouble()
    {
        return 0.5;
    }

    public double GetDouble(double min, double max)
    {
        return min;
    }
}
