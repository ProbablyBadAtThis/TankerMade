using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.Services;
using TankerMade.Modules.Printing3D;
using TankerMade.Modules.Printing3D.DTOs.Inventory;
using TankerMade.Modules.Printing3D.Services;
using TankerMade.Server.Controllers;

namespace TankerMade.Server.Controllers.Printing3D;

[ApiController]
[Authorize]
[Route("api/modules/printing-3d/inventory")]
public class PrintingInventoryController : ControllerBase
{
    private readonly IPrintingInventoryService _inventoryService;
    private readonly IModuleService _moduleService;

    public PrintingInventoryController(IPrintingInventoryService inventoryService, IModuleService moduleService)
    {
        _inventoryService = inventoryService;
        _moduleService = moduleService;
    }

    [HttpGet("materials")]
    public async Task<ActionResult<IReadOnlyList<PrintingMaterialInventoryItemDto>>> GetMaterials(
        [FromQuery] PrintingMaterialInventoryFilterDto filter)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _inventoryService.GetMaterialsAsync(userId.Value, filter));
    }

    [HttpGet("materials/{id:guid}")]
    public async Task<ActionResult<PrintingMaterialInventoryItemDto>> GetMaterialById(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var material = await _inventoryService.GetMaterialByIdAsync(id, userId.Value);
        return material == null ? NotFound() : Ok(material);
    }

    [HttpPost("materials")]
    public async Task<ActionResult<PrintingMaterialInventoryItemDto>> CreateMaterial(
        CreatePrintingMaterialInventoryItemDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var material = await _inventoryService.CreateOrMergeMaterialAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetMaterialById), new { id = material.Id }, material);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("reference/{category}")]
    public async Task<ActionResult<IReadOnlyList<PrintingInventoryReferenceItemDto>>> GetReferenceItems(
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

        return await _moduleService.IsActiveAsync(Printing3DModule.ModuleKey, userId.Value)
            ? userId
            : null;
    }
}
