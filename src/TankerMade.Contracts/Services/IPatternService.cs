using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TankerMade.Contracts.DTOs.Patterns;

namespace TankerMade.Contracts.Services
{
    public interface IPatternService
    {
        // Create
        Task<PatternDto> CreateAsync(CreatePatternDto createDto, Guid userId);

        // Read
        Task<PatternDto?> GetByIdAsync(Guid id, Guid userId);
        Task<PatternDto?> GetBySlugAsync(string slug, Guid userId);
        Task<IEnumerable<PatternDto>> GetAllAsync(Guid userId);
        Task<IEnumerable<PatternDto>> GetByThemeIdAsync(Guid themeId, Guid userId);
        Task<IEnumerable<PatternDto>> GetByDifficultyAsync(int difficulty, Guid userId);

        // Update  
        Task<PatternDto?> UpdateAsync(UpdatePatternDto updateDto, Guid userId);

        // Delete
        Task<bool> DeleteAsync(Guid id, Guid userId);

        // Additional business operations
        Task<bool> ExistsAsync(Guid id, Guid userId);
        Task<IEnumerable<PatternDto>> SearchAsync(string searchTerm, Guid userId);
    }
}
