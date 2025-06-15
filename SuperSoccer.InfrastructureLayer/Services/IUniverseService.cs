using SuperSoccer.DomainLayer;

namespace InfrastructureLayer.Services;

public interface IUniverseService
{ 
    IUniverseAdapter? GetAdapter(string universeName);
}