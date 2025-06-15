using SuperSoccer.DomainLayer;

namespace InfrastructureLayer.Services;

public class UniverseService : IUniverseService
{
    private readonly IEnumerable<IUniverseAdapter> _adapters;

    public UniverseService(IEnumerable<IUniverseAdapter> adapters)
    {
        _adapters = adapters;
    }

    public IUniverseAdapter? GetAdapter(string universeName)
        => _adapters.FirstOrDefault(u => u.UniverseName.Equals(universeName, StringComparison.OrdinalIgnoreCase));
}