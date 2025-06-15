using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfrastructureLayer.Adapters;
using InfrastructureLayer.Contracts.Pokemon;
using InfrastructureLayer.Services.Pokemon;
using InfrastructureLayer.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using SuperSoccer.DomainLayer.Entities;
using Xunit;

namespace SuperSoccer.InfrastructureLayer.Tests;

public class PokemonUniverseAdapterTests
{
    private readonly Mock<IPokemonDataService> _mockDataService;
    private readonly Mock<ILogger<PokemonUniverseAdapter>> _mockLogger;
    private readonly Mock<IRandomProvider> _mockRandomProvider;
    private readonly PokemonUniverseAdapter _adapter;

    public PokemonUniverseAdapterTests()
    {
        _mockDataService = new Mock<IPokemonDataService>();
        _mockLogger = new Mock<ILogger<PokemonUniverseAdapter>>();
        _mockRandomProvider = new Mock<IRandomProvider>();

        _adapter = new PokemonUniverseAdapter(
            _mockDataService.Object,
            _mockLogger.Object,
            _mockRandomProvider.Object
        );
    }
    
    [Fact]
    public async Task GenerateTeamAsync_NoPokemonAvailable_ReturnsEmptyTeam()
    {
        // Arrange
        _mockDataService.Setup(ds => ds.GetPokemonPoolAsync(It.IsAny<int>()))
            .ReturnsAsync([]);

        // Act
        var result = await _adapter.GenerateTeamAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Players);
    }

    
    [Fact]
    public async Task GenerateTeamAsync_PokemonAvailable_ReturnsExpectedTeamSize()
    {
        // Arrange
        var pokemonPool = new List<PokemonEntry>
        {
            new() { Url = "https://pokeapi.co/api/v2/pokemon/1" },
            new() { Url = "https://pokeapi.co/api/v2/pokemon/2" },
            new() { Url = "https://pokeapi.co/api/v2/pokemon/3" },
            new() { Url = "https://pokeapi.co/api/v2/pokemon/4" },
            new() { Url = "https://pokeapi.co/api/v2/pokemon/5" }
        };

        _mockDataService.Setup(ds => ds.GetPokemonPoolAsync(It.IsAny<int>())).ReturnsAsync(pokemonPool);
        _mockRandomProvider.Setup(rp => rp.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(1); 

        _mockDataService.Setup(ds => ds.GetPokemonDetailsAsync(It.IsAny<string>()))
            .ReturnsAsync(new Player ("Pikachu", 100, 10 ));

        // Act
        var result = await _adapter.GenerateTeamAsync(3);

        // Assert
        Assert.Equal(3, result.Players.Count);
    }
    
    [Fact]
    public async Task GenerateTeamAsync_FailureDuringFetch_LogsError()
    {
        // Arrange
        var pokemonPool = new List<PokemonEntry>
        {
            new() { Url = "https://pokeapi.co/api/v2/pokemon/1" }
        };

        _mockDataService.Setup(ds => ds.GetPokemonPoolAsync(It.IsAny<int>())).ReturnsAsync(pokemonPool);
    
        _mockDataService.Setup(ds => ds.GetPokemonDetailsAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("API failed"));

        // Act
        await Assert.ThrowsAsync<Exception>(async () =>
            await _adapter.GenerateTeamAsync());

        // Assert
        _mockLogger.Verify(x =>
                x.Log(
                    It.Is<LogLevel>(level => level == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to fetch details for")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
