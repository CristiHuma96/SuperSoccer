using InfrastructureLayer.Contracts.StarWars;
using SuperSoccer.DomainLayer.Entities;

namespace InfrastructureLayer.Services.StarWars;

public interface IStarWarsDataService
{
    Task<List<StarWarsEntry>> GetStarWarsPoolAsync(int defaultTeamSize = 5);
    Task<Player?> GetStarWarsDetailsAsync(string url);
}