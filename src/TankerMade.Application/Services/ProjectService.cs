using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TankerMade.Application.Common.Exceptions;
using TankerMade.Contracts.DTOs.Projects;
using TankerMade.Contracts.Services;

namespace TankerMade.Application.Services
{
    public class ProjectService : IProjectService
    {
        // TODO: Add constructor with repository dependencies when ready
        // private readonly IProjectRepository _projectRepository;
        // public ProjectService(IProjectRepository projectRepository) => _projectRepository = projectRepository;

        public async Task<ProjectDto> CreateAsync(CreateProjectDto createDto, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<ProjectDto> GetByIdAsync(Guid id, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<ProjectDto> GetBySlugAsync(string slug, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<IEnumerable<ProjectDto>> GetAllAsync(Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<ProjectDto> UpdateAsync(UpdateProjectDto updateDto, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<bool> ExistsAsync(Guid id, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<IEnumerable<ProjectDto>> SearchAsync(string search, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }

    }
}
