using InfrastructureLayer.Contracts.Pokemon;
using InfrastructureLayer.Services;
using InfrastructureLayer.Services.Pokemon;
using InfrastructureLayer.Utils;
using Microsoft.Extensions.Logging;
using SuperSoccer.DomainLayer;
using SuperSoccer.DomainLayer.Entities;

namespace InfrastructureLayer.Adapters;

public class PokemonUniverseAdapter : IUniverseAdapter
{
    private readonly IPokemonDataService _dataService;
    private readonly ILogger<PokemonUniverseAdapter> _logger;
    private readonly IRandomProvider _randomProvider;

    public string UniverseName => "pokemon";

    public PokemonUniverseAdapter(IPokemonDataService dataService, ILogger<PokemonUniverseAdapter> logger, IRandomProvider randomProvider)
    {
        _dataService = dataService;
        _logger = logger;
        _randomProvider = randomProvider;
    }

    public async Task<Team> GenerateTeamAsync(int defaultTeamSize = 5)
    {
        var pool = await _dataService.GetPokemonPoolAsync();

        if (pool.Count == 0)
        {
            return Team.EmptyTeam();
        }
        
        var picks = pool.OrderBy(_ => _randomProvider.Next()).Take(defaultTeamSize);
        var tasks = GetPokemonDetailsInParallel(picks);
        var results = await Task.WhenAll(tasks);

        return new Team(results.Where(p => p is not null).ToList()!);
    }

    private IEnumerable<Task<Player?>> GetPokemonDetailsInParallel(IEnumerable<PokemonEntry> picks)
    {
        var tasks = picks.Select(async p =>
        {
            try
            {
                return await _dataService.GetPokemonDetailsAsync(p.Url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch details for {Url}", p.Url);
                throw;
            }
        });
        return tasks;
    }
}