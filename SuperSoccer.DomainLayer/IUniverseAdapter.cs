using SuperSoccer.DomainLayer.Entities;

namespace SuperSoccer.DomainLayer;

public interface IUniverseAdapter
{
    string UniverseName { get; }

    Task<Team> GenerateTeamAsync(int defaultTeamSize = 5);
}