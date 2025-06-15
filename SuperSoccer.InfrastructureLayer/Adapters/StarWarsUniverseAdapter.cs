using InfrastructureLayer.Contracts.StarWars;
using InfrastructureLayer.Services.StarWars;
using InfrastructureLayer.Utils;
using Microsoft.Extensions.Logging;
using SuperSoccer.DomainLayer;
using SuperSoccer.DomainLayer.Entities;

namespace InfrastructureLayer.Adapters;

public class StarWarsUniverseAdapter: IUniverseAdapter
{
    private readonly IStarWarsDataService _dataService;
    private readonly ILogger<StarWarsUniverseAdapter> _logger;
    private readonly IRandomProvider _randomProvider;
    public string UniverseName => "starwars";
    
    
    public StarWarsUniverseAdapter(IStarWarsDataService dataService, ILogger<StarWarsUniverseAdapter> logger, IRandomProvider randomProvider)
    {
        _dataService = dataService;
        _dataService = dataService;
        _logger = logger;
        _randomProvider = randomProvider;
    }
    
    public async Task<Team> GenerateTeamAsync(int defaultTeamSize = 5)
    {
        var pool = await _dataService.GetStarWarsPoolAsync();

        if (pool.Count == 0)
        {
            return Team.EmptyTeam();
        }
        
        var picks = pool.OrderBy(_ => _randomProvider.Next()).Take(defaultTeamSize);
        var tasks = GetStarWarsDetailsInParallel(picks);
        var results = await Task.WhenAll(tasks);

        return new Team(results.Where(p => p is not null).ToList()!);
    }
    
    private IEnumerable<Task<Player?>> GetStarWarsDetailsInParallel(IEnumerable<StarWarsEntry> picks)
    {
        var tasks = picks.Select(async p =>
        {
            try
            {
                return await _dataService.GetStarWarsDetailsAsync(p.Url);
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