using TankerMade.Contracts.DTOs.Projects;

namespace TankerMade.Contracts.Services
{
    public interface IProjectService
    {
        // Create
        Task<ProjectDto> CreateAsync(CreateProjectDto createDto);

        // Read
        Task<ProjectDto?> GetByIdAsync(Guid id);
        Task<ProjectDto?> GetBySlugAsync(string slug);
        Task<IEnumerable<ProjectDto>> GetAllAsync();
        Task<IEnumerable<ProjectDto>> GetByUserIdAsync(Guid userId);

        // Update  
        Task<ProjectDto?> UpdateAsync(UpdateProjectDto updateDto);

        // Delete
        Task<bool> DeleteAsync(Guid id);

        // Additional business operations
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<ProjectDto>> SearchAsync(string searchTerm);
    }
}