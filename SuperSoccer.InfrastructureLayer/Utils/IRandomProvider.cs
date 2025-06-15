namespace InfrastructureLayer.Utils;

public interface IRandomProvider
{
    int Next(int min = 0, int max = int.MaxValue);
}