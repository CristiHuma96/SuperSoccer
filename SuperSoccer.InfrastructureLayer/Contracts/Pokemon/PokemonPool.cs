namespace InfrastructureLayer.Contracts.Pokemon;

public class PokemonPool
{
    public int Count { get; set; }
    public List<PokemonEntry> Results { get; set; } = new();
}