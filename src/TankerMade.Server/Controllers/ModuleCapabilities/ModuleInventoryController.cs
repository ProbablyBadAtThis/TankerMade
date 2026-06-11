using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.DTOs.ModuleInventory;
using TankerMade.Contracts.Services;
using TankerMade.Contracts.Services.ModuleCapabilities;
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

    [HttpGet("yarns")]
    public async Task<ActionResult<IReadOnlyList<ModuleYarnInventoryItemDto>>> GetYarns(
        string moduleKey,
        [FromQuery] ModuleYarnInventoryFilterRequest? filter = null)
    {
        var gate = await ResolveCraftGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return Ok(await gate.Handler!.GetYarnsAsync(gate.UserId!.Value, filter));
    }

    [HttpPost("yarns")]
    public async Task<ActionResult<ModuleYarnInventoryItemDto>> CreateYarn(
        string moduleKey,
        CreateModuleYarnInventoryItemDto request)
    {
        var gate = await ResolveCraftGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return Ok(await gate.Handler!.CreateOrMergeYarnAsync(request, gate.UserId!.Value));
    }

    [HttpGet("yarns/{id:guid}")]
    public async Task<ActionResult<ModuleYarnInventoryItemDto>> GetYarn(string moduleKey, Guid id)
    {
        var gate = await ResolveCraftGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var yarn = await gate.Handler!.GetYarnByIdAsync(id, gate.UserId!.Value);
        return yarn == null ? NotFound() : Ok(yarn);
    }

    [HttpGet("tools")]
    public async Task<ActionResult<IReadOnlyList<ModuleToolInventoryItemDto>>> GetTools(
        string moduleKey,
        [FromQuery] ModuleToolInventoryFilterRequest? filter = null)
    {
        var gate = await ResolveCraftGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return Ok(await gate.Handler!.GetToolsAsync(gate.UserId!.Value, filter));
    }

    [HttpPost("tools")]
    public async Task<ActionResult<ModuleToolInventoryItemDto>> CreateTool(
        string moduleKey,
        CreateModuleToolInventoryItemDto request)
    {
        var gate = await ResolveCraftGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return Ok(await gate.Handler!.CreateOrMergeToolAsync(request, gate.UserId!.Value));
    }

    [HttpGet("tools/{id:guid}")]
    public async Task<ActionResult<ModuleToolInventoryItemDto>> GetTool(string moduleKey, Guid id)
    {
        var gate = await ResolveCraftGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var tool = await gate.Handler!.GetToolByIdAsync(id, gate.UserId!.Value);
        return tool == null ? NotFound() : Ok(tool);
    }

    [HttpGet("notions")]
    public async Task<ActionResult<IReadOnlyList<ModuleNotionInventoryItemDto>>> GetNotions(
        string moduleKey,
        [FromQuery] ModuleNotionInventoryFilterRequest? filter = null)
    {
        var gate = await ResolveCraftGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return Ok(await gate.Handler!.GetNotionsAsync(gate.UserId!.Value, filter));
    }

    [HttpPost("notions")]
    public async Task<ActionResult<ModuleNotionInventoryItemDto>> CreateNotion(
        string moduleKey,
        CreateModuleNotionInventoryItemDto request)
    {
        var gate = await ResolveCraftGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return Ok(await gate.Handler!.CreateOrMergeNotionAsync(request, gate.UserId!.Value));
    }

    [HttpGet("notions/{id:guid}")]
    public async Task<ActionResult<ModuleNotionInventoryItemDto>> GetNotion(string moduleKey, Guid id)
    {
        var gate = await ResolveCraftGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        var notion = await gate.Handler!.GetNotionByIdAsync(id, gate.UserId!.Value);
        return notion == null ? NotFound() : Ok(notion);
    }

    [HttpGet("reference/{category}")]
    public async Task<ActionResult<IReadOnlyList<ModuleInventoryReferenceItemDto>>> GetReferenceItems(
        string moduleKey,
        string category)
    {
        var gate = await ResolveCraftGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return Ok(await gate.Handler!.GetReferenceItemsAsync(category));
    }

    [HttpPost("reference/{category}")]
    public async Task<ActionResult<ModuleInventoryReferenceItemDto>> CreateReferenceItem(
        string moduleKey,
        string category,
        CreateModuleInventoryReferenceItemDto request)
    {
        var gate = await ResolveCraftGateAsync(moduleKey);
        if (!gate.IsAllowed)
        {
            return gate.Result!;
        }

        return Ok(await gate.Handler!.CreateReferenceItemAsync(category, request));
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

    private async Task<CraftGateResult> ResolveCraftGateAsync(string moduleKey)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return CraftGateResult.Unauthorized();
        }

        if (!await _moduleService.IsActiveAsync(moduleKey, userId.Value))
        {
            return CraftGateResult.Forbid();
        }

        var handler = _resolver.Resolve(moduleKey) as IModuleCraftInventoryCapabilityHandler;
        if (handler == null)
        {
            return CraftGateResult.NotImplemented($"Module craft inventory capability handler was not found for '{moduleKey}'.");
        }

        return CraftGateResult.Allowed(userId.Value, handler);
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

    private sealed class CraftGateResult
    {
        public bool IsAllowed { get; init; }
        public Guid? UserId { get; init; }
        public IModuleCraftInventoryCapabilityHandler? Handler { get; init; }
        public ActionResult? Result { get; init; }

        public static CraftGateResult Allowed(Guid userId, IModuleCraftInventoryCapabilityHandler handler)
            => new() { IsAllowed = true, UserId = userId, Handler = handler };

        public static CraftGateResult Unauthorized() => new() { Result = new UnauthorizedResult() };

        public static CraftGateResult Forbid() => new() { Result = new ForbidResult() };

        public static CraftGateResult NotImplemented(string message)
            => new() { Result = new ObjectResult(message) { StatusCode = StatusCodes.Status501NotImplemented } };
    }
}
