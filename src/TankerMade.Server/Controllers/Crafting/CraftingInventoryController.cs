using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.Services;
using TankerMade.Modules.Crafting;
using TankerMade.Modules.Crafting.DTOs.Inventory;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Controllers;

namespace TankerMade.Server.Controllers.Crafting;

[ApiController]
[Authorize]
[Route("api/modules/crafting/inventory")]
public class CraftingInventoryController : ControllerBase
{
    private readonly ICraftingInventoryService _inventoryService;
    private readonly IModuleService _moduleService;

    public CraftingInventoryController(ICraftingInventoryService inventoryService, IModuleService moduleService)
    {
        _inventoryService = inventoryService;
        _moduleService = moduleService;
    }

    [HttpGet("yarns")]
    public async Task<ActionResult<IReadOnlyList<CraftingYarnInventoryItemDto>>> GetYarns(
        [FromQuery] CraftingYarnInventoryFilterDto filter)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _inventoryService.GetYarnsAsync(userId.Value, filter));
    }

    [HttpGet("yarns/{id:guid}")]
    public async Task<ActionResult<CraftingYarnInventoryItemDto>> GetYarnById(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var yarn = await _inventoryService.GetYarnByIdAsync(id, userId.Value);
        return yarn == null ? NotFound() : Ok(yarn);
    }

    [HttpPost("yarns")]
    public async Task<ActionResult<CraftingYarnInventoryItemDto>> CreateYarn(
        CreateCraftingYarnInventoryItemDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var yarn = await _inventoryService.CreateOrMergeYarnAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetYarnById), new { id = yarn.Id }, yarn);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("tools")]
    public async Task<ActionResult<IReadOnlyList<CraftingToolInventoryItemDto>>> GetTools(
        [FromQuery] CraftingToolInventoryFilterDto filter)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _inventoryService.GetToolsAsync(userId.Value, filter));
    }

    [HttpGet("tools/{id:guid}")]
    public async Task<ActionResult<CraftingToolInventoryItemDto>> GetToolById(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var tool = await _inventoryService.GetToolByIdAsync(id, userId.Value);
        return tool == null ? NotFound() : Ok(tool);
    }

    [HttpPost("tools")]
    public async Task<ActionResult<CraftingToolInventoryItemDto>> CreateTool(
        CreateCraftingToolInventoryItemDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var tool = await _inventoryService.CreateOrMergeToolAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetToolById), new { id = tool.Id }, tool);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("notions")]
    public async Task<ActionResult<IReadOnlyList<CraftingNotionInventoryItemDto>>> GetNotions(
        [FromQuery] CraftingNotionInventoryFilterDto filter)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _inventoryService.GetNotionsAsync(userId.Value, filter));
    }

    [HttpGet("notions/{id:guid}")]
    public async Task<ActionResult<CraftingNotionInventoryItemDto>> GetNotionById(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var notion = await _inventoryService.GetNotionByIdAsync(id, userId.Value);
        return notion == null ? NotFound() : Ok(notion);
    }

    [HttpPost("notions")]
    public async Task<ActionResult<CraftingNotionInventoryItemDto>> CreateNotion(
        CreateCraftingNotionInventoryItemDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var notion = await _inventoryService.CreateOrMergeNotionAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetNotionById), new { id = notion.Id }, notion);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("reference/{category}")]
    public async Task<ActionResult<IReadOnlyList<CraftingInventoryReferenceItemDto>>> GetReferenceItems(
        string category)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _inventoryService.GetReferenceItemsAsync(category));
    }

    private async Task<Guid?> GetActiveModuleUserIdAsync()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return null;
        }

        return await _moduleService.IsActiveAsync(CraftingModule.ModuleKey, userId.Value)
            ? userId
            : null;
    }
}
