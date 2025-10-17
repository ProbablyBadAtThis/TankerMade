using TankerMade.Contracts.DTOs.Projects;

namespace TankerMade.Contracts.Services
{
    public interface IProjectService
    {
        // Create
        Task<ProjectDto> CreateAsync(CreateProjectDto createDto, Guid userId);

        // Read
        Task<ProjectDto?> GetByIdAsync(Guid id, Guid userId);
        Task<ProjectDto?> GetBySlugAsync(string slug, Guid userId);
        Task<IEnumerable<ProjectDto>> GetAllAsync(Guid userId);

        // Update  
        Task<ProjectDto?> UpdateAsync(UpdateProjectDto updateDto, Guid userId);

        // Delete
        Task<bool> DeleteAsync(Guid id, Guid userId);

        // Additional business operations
        Task<bool> ExistsAsync(Guid id, Guid userId);
        Task<IEnumerable<ProjectDto>> SearchAsync(string searchTerm, Guid userId);
    }
}