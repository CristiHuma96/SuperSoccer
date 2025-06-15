namespace InfrastructureLayer.Utils;

public class RandomProvider : IRandomProvider
{
    private readonly Random _random = new();

    public int Next(int min, int max) => _random.Next(min, max);
}