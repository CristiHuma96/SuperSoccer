using InfrastructureLayer.Contracts.Pokemon;
using SuperSoccer.DomainLayer.Entities;

namespace InfrastructureLayer.Services.Pokemon;

public interface IPokemonDataService
{
    Task<List<PokemonEntry>> GetPokemonPoolAsync(int defaultTeamSize = 5);
    Task<Player?> GetPokemonDetailsAsync(string url);
}