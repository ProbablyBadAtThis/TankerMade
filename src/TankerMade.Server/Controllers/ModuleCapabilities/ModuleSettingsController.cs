using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.ModuleSettings;
using TankerMade.Contracts.Services;
using TankerMade.Server.Controllers;
using TankerMade.Server.Services.ModuleCapabilities;

namespace TankerMade.Server.Controllers.ModuleCapabilities;

[ApiController]
[Authorize]
[Route("api/modules/{moduleKey}/capabilities/settings")]
public class ModuleSettingsController : ControllerBase
{
    private readonly IModuleService _moduleService;
    private readonly IModuleSettingsCapabilityResolver _resolver;

    public ModuleSettingsController(IModuleService moduleService, IModuleSettingsCapabilityResolver resolver)
    {
        _moduleService = moduleService;
        _resolver = resolver;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ModuleSettingItemDto>>> GetAll(string moduleKey, [FromQuery] string? category = null)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        return Ok(await gate.Handler!.GetAllAsync(gate.UserId!.Value, category));
    }

    [HttpPost]
    public async Task<ActionResult<ModuleSettingItemDto>> Upsert(string moduleKey, UpsertModuleSettingItemRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        return Ok(await gate.Handler!.UpsertAsync(request, gate.UserId!.Value));
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(string moduleKey, DeleteModuleSettingItemRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed) return gate.Result!;
        return await gate.Handler!.DeleteAsync(request, gate.UserId!.Value)
            ? NoContent()
            : NotFound();
    }

    private async Task<GateResult> ResolveGateAsync(string moduleKey)
    {
        var userId = User.GetUserId();
        if (userId == null) return GateResult.Unauthorized();
        if (!await _moduleService.IsActiveAsync(moduleKey, userId.Value)) return GateResult.Forbid();

        var handler = _resolver.Resolve(moduleKey);
        if (handler == null) return GateResult.NotImplemented($"Module settings capability handler was not found for '{moduleKey}'.");

        return GateResult.Allowed(userId.Value, handler);
    }

    private sealed class GateResult
    {
        public bool IsAllowed { get; init; }
        public Guid? UserId { get; init; }
        public Contracts.Services.ModuleCapabilities.IModuleSettingsCapabilityHandler? Handler { get; init; }
        public ActionResult? Result { get; init; }

        public static GateResult Allowed(Guid userId, Contracts.Services.ModuleCapabilities.IModuleSettingsCapabilityHandler handler)
            => new() { IsAllowed = true, UserId = userId, Handler = handler };

        public static GateResult Unauthorized() => new() { Result = new UnauthorizedResult() };
        public static GateResult Forbid() => new() { Result = new ForbidResult() };
        public static GateResult NotImplemented(string message)
            => new() { Result = new ObjectResult(message) { StatusCode = StatusCodes.Status501NotImplemented } };
    }
}
