using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankerMade.Contracts.Services;
using TankerMade.Modules.Crafting;
using TankerMade.Modules.Crafting.DTOs.Projects;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Controllers;

namespace TankerMade.Server.Controllers.Crafting;

[ApiController]
[Authorize]
[Route("api/modules/crafting/projects")]
public class CraftingProjectsController : ControllerBase
{
    private readonly ICraftingProjectService _projectService;
    private readonly IModuleService _moduleService;

    public CraftingProjectsController(ICraftingProjectService projectService, IModuleService moduleService)
    {
        _projectService = projectService;
        _moduleService = moduleService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CraftingProjectDto>>> GetAll([FromQuery] bool includeArchived = false)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        return Ok(await _projectService.GetAllAsync(userId.Value, includeArchived));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CraftingProjectDto>> GetById(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var project = await _projectService.GetByIdAsync(id, userId.Value);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<CraftingProjectDto>> Create(CreateCraftingProjectDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.CreateAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CraftingProjectDto>> Update(Guid id, UpdateCraftingProjectDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            request.Id = id;
            var project = await _projectService.UpdateAsync(request, userId.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/archive")]
    public async Task<ActionResult<CraftingProjectDto>> Archive(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var project = await _projectService.ArchiveAsync(id, userId.Value);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPut("{id:guid}/reopen")]
    public async Task<ActionResult<CraftingProjectDto>> Reopen(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var project = await _projectService.ReopenAsync(id, userId.Value);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/progress")]
    public async Task<ActionResult<CraftingProjectDto>> SetStepProgress(
        Guid id,
        Guid stepId,
        UpdateCraftingProjectStepProgressDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.SetStepProgressAsync(id, stepId, request, userId.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/timer/start")]
    public async Task<ActionResult<CraftingProjectDto>> StartTimer(Guid id, Guid stepId, UpdateCraftingProjectTimerDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.StartTimerAsync(id, stepId, request, userId.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/timer/pause")]
    public async Task<ActionResult<CraftingProjectDto>> PauseTimer(Guid id, Guid stepId, UpdateCraftingProjectTimerDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.PauseTimerAsync(id, stepId, request, userId.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}/steps/{stepId:guid}/timer")]
    public async Task<ActionResult<CraftingProjectDto>> SetTimer(Guid id, Guid stepId, UpdateCraftingProjectTimerDto request)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.SetTimerAsync(id, stepId, request, userId.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}/steps/{stepId:guid}/timer")]
    public async Task<ActionResult<CraftingProjectDto>> ResetTimer(Guid id, Guid stepId)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        try
        {
            var project = await _projectService.ResetTimerAsync(id, stepId, userId.Value);
            return project == null ? NotFound() : Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var userId = await GetActiveModuleUserIdAsync();
        if (userId == null)
        {
            return Forbid();
        }

        var deleted = await _projectService.DeleteAsync(id, userId.Value);
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
