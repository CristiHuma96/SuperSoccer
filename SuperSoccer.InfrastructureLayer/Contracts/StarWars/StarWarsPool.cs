namespace InfrastructureLayer.Contracts.StarWars;

public class StarWarsPool
{
    public int Count { get; set; }
    public List<StarWarsEntry> Results { get; set; } = new();
}