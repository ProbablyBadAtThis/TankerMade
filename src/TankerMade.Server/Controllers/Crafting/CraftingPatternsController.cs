using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.Services;
using TankerMade.Modules.Crafting;
using TankerMade.Modules.Crafting.DTOs.Patterns;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Controllers;

namespace TankerMade.Server.Controllers.Crafting;

[ApiController]
[Authorize]
[Route("api/modules/crafting/patterns")]
public class CraftingPatternsController : ControllerBase
{
    private readonly ICraftingPatternService _patternService;
    private readonly IModuleService _moduleService;

    public CraftingPatternsController(ICraftingPatternService patternService, IModuleService moduleService)
    {
        _patternService = patternService;
        _moduleService = moduleService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CraftingPatternDto>>> GetAll()
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _patternService.GetAllAsync(userId.Value));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CraftingPatternDto>> GetById(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var pattern = await _patternService.GetByIdAsync(id, userId.Value);
        return pattern == null ? NotFound() : Ok(pattern);
    }

    [HttpPost]
    public async Task<ActionResult<CraftingPatternDto>> Create(CreateCraftingPatternDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var pattern = await _patternService.CreateAsync(request, userId.Value);
        return CreatedAtAction(nameof(GetById), new { id = pattern.Id }, pattern);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CraftingPatternDto>> Update(Guid id, UpdateCraftingPatternDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        request.Id = id;
        var pattern = await _patternService.UpdateAsync(request, userId.Value);
        return pattern == null ? NotFound() : Ok(pattern);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var deleted = await _patternService.DeleteAsync(id, userId.Value);
        return deleted ? NoContent() : NotFound();
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
