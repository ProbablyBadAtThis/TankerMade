using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.ModuleInventory;
using TankerMade.Contracts.Services;
using TankerMade.Server.Controllers;
using TankerMade.Server.Services.ModuleCapabilities;

namespace TankerMade.Server.Controllers.ModuleCapabilities;

[ApiController]
[Authorize]
[Route("api/modules/{moduleKey}/capabilities/inventory")]
public class ModuleInventoryController : ControllerBase
{
    private readonly IModuleService _moduleService;
    private readonly IModuleInventoryCapabilityResolver _resolver;

    public ModuleInventoryController(IModuleService moduleService, IModuleInventoryCapabilityResolver resolver)
    {
        _moduleService = moduleService;
        _resolver = resolver;
    }

    [HttpGet("supplies")]
    public async Task<ActionResult<IReadOnlyList<ModuleSupplyItemDto>>> GetSupplies(
        string moduleKey,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return Ok(await gate.Handler!.GetSuppliesAsync(gate.UserId!.Value, search, category));
    }

    [HttpPost("supplies")]
    public async Task<ActionResult<ModuleSupplyItemDto>> CreateSupply(
        string moduleKey,
        CreateModuleSupplyItemRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var item = await gate.Handler!.CreateSupplyAsync(request, gate.UserId!.Value);
        return Ok(item);
    }

    [HttpPut("supplies/{supplyId:guid}")]
    public async Task<ActionResult<ModuleSupplyItemDto>> UpdateSupply(
        string moduleKey,
        Guid supplyId,
        UpdateModuleSupplyItemRequest request)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var item = await gate.Handler!.UpdateSupplyAsync(supplyId, request, gate.UserId!.Value);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpDelete("supplies/{supplyId:guid}")]
    public async Task<ActionResult> DeleteSupply(string moduleKey, Guid supplyId)
    {
        var gate = await ResolveGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return await gate.Handler!.DeleteSupplyAsync(supplyId, gate.UserId!.Value)
            ? NoContent()
            : NotFound();
    }

    private async Task<GateResult> ResolveGateAsync(string moduleKey)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return GateResult.Unauthorized();
        }

        if (!await _moduleService.IsActiveAsync(moduleKey, userId.Value))
        {
            return GateResult.Forbid();
        }

        var handler = _resolver.Resolve(moduleKey);
        if (handler == null)
        {
            return GateResult.NotImplemented($"Module inventory capability handler was not found for '{moduleKey}'.");
        }

        return GateResult.Allowed(userId.Value, handler);
    }

    private sealed class GateResult
    {
        public bool IsAllowed { get; init; }
        public Guid? UserId { get; init; }
        public Contracts.Services.ModuleCapabilities.IModuleInventoryCapabilityHandler? Handler { get; init; }
        public ActionResult? Result { get; init; }

        public static GateResult Allowed(Guid userId, Contracts.Services.ModuleCapabilities.IModuleInventoryCapabilityHandler handler)
            => new() { IsAllowed = true, UserId = userId, Handler = handler };

        public static GateResult Unauthorized() => new() { Result = new UnauthorizedResult() };

        public static GateResult Forbid() => new() { Result = new ForbidResult() };

        public static GateResult NotImplemented(string message)
            => new() { Result = new ObjectResult(message) { StatusCode = StatusCodes.Status501NotImplemented } };
    }
}
