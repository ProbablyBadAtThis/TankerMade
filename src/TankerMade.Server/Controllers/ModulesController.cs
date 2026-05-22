using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.Modules;
using TankerMade.Contracts.Services;

namespace TankerMade.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/modules")]
public class ModulesController : ControllerBase
{
    private readonly IModuleService _moduleService;

    public ModulesController(IModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ModuleDto>>> GetAvailable()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        return Ok(await _moduleService.GetAvailableModulesAsync(userId.Value));
    }

    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyList<ModuleDto>>> GetActive()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        return Ok(await _moduleService.GetActiveModulesAsync(userId.Value));
    }

    [HttpPost("activate")]
    public async Task<ActionResult<ModuleDto>> Activate(ModuleActivationRequest request)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var module = await _moduleService.ActivateAsync(request.ModuleKey, userId.Value);
        return module == null ? NotFound() : Ok(module);
    }

    [HttpPost("deactivate")]
    public async Task<ActionResult> Deactivate(ModuleActivationRequest request)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var deactivated = await _moduleService.DeactivateAsync(request.ModuleKey, userId.Value);
        return deactivated ? NoContent() : NotFound();
    }
}
