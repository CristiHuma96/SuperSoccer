using System.Net.Http.Headers;
using InfrastructureLayer.Adapters;
using InfrastructureLayer.Services;
using InfrastructureLayer.Services.Pokemon;
using InfrastructureLayer.Services.StarWars;
using InfrastructureLayer.Utils;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using SuperSoccer.DomainLayer;

namespace InfrastructureLayer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddMemoryCache(options => options.ExpirationScanFrequency = TimeSpan.FromHours(24));
        
        services.AddTransient<IRandomProvider, RandomProvider>();
        services.AddTransient<IUniverseService, UniverseService>();

        AddPokemonDependencies(services);
        
        AddStarWarsDependencies(services);

        return services;
    }

    private static void AddStarWarsDependencies(IServiceCollection services)
    {
        services.AddHttpClient<IStarWarsDataService, StarWarsDataService>("PokemonApi", client =>
            {
                client.BaseAddress = new Uri("https://pokeapi.co");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(500))
            );

        services.AddTransient<IUniverseAdapter, StarWarsUniverseAdapter>();
    }

    private static void AddPokemonDependencies(IServiceCollection services)
    {
        services.AddHttpClient<IPokemonDataService, PokemonDataService>("PokemonApi", client =>
            {
                client.BaseAddress = new Uri("https://swapi.dev");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(500))
            );
        services.AddTransient<IUniverseAdapter, PokemonUniverseAdapter>();
    }
}