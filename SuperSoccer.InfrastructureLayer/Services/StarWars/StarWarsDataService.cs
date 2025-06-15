using System.Net.Http.Json;
using InfrastructureLayer.Contracts.StarWars;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SuperSoccer.DomainLayer.Entities;

namespace InfrastructureLayer.Services.StarWars;

public class StarWarsDataService :  IStarWarsDataService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private static string CacheKey => $"{nameof(StarWarsDataService)}_star_wars_list";
    
    public StarWarsDataService(HttpClient httpClient, IMemoryCache cache, IConfiguration config)
    {
        _httpClient = httpClient;
        _cache = cache;
        
        var baseUrl = config["ExternalApis:Pokemon:BaseUrl"];
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public async Task<List<StarWarsEntry>> GetStarWarsPoolAsync(int defaultTeamSize = 5)
    {
        if (_cache.TryGetValue(CacheKey, out StarWarsPool? cached))
            return cached?.Results ?? [];

        var response = await _httpClient.GetFromJsonAsync<StarWarsPool>(
            "https://pokeapi.co/api/v2/pokemon?limit=100000");

        if (response is null || response.Count < defaultTeamSize)
            throw new Exception("Failed to load Pokémon list or not enough entries.");

        _cache.Set(CacheKey, response, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
        });

        return response.Results;
    }

    public async Task<Player?> GetStarWarsDetailsAsync(string url)
    {
        return await _httpClient.GetFromJsonAsync<Player>(url);
    }
}