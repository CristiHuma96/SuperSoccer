using Asp.Versioning;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace SuperSoccer.PresentationLayer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SuperSoccerShowdownController : Controller
{
    private readonly IUniverseService _universeService;

    public SuperSoccerShowdownController(IUniverseService universeService)
    {
        _universeService = universeService;
    }
    
    [HttpGet("{universe}/team")]
    public async Task<IActionResult> GetTeam(string universe)
    {
        var adapter = _universeService.GetAdapter(universe);
        if (adapter is null)
            return NotFound($"Universe '{universe}' not supported.");

        var team = await adapter.GenerateTeamAsync();
        if (team.Players.Count == 0)
        {
            return NoContent();
        }
        return Ok(team);
    }
}