using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TankerMade.Contracts.DTOs.Patterns;

namespace TankerMade.Contracts.Services
{
    public interface IPatternService
    {
        // Create
        Task<PatternDto> CreateAsync(CreatePatternDto createDto);

        // Read
        Task<PatternDto?> GetByIdAsync(Guid id);
        Task<PatternDto?> GetBySlugAsync(string slug);
        Task<IEnumerable<PatternDto>> GetAllAsync();
        Task<IEnumerable<PatternDto>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<PatternDto>> GetByThemeIdAsync(Guid themeId);
        Task<IEnumerable<PatternDto>> GetByDifficultyAsync(int difficulty);

        // Update  
        Task<PatternDto?> UpdateAsync(UpdatePatternDto updateDto);

        // Delete
        Task<bool> DeleteAsync(Guid id);

        // Additional business operations
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<PatternDto>> SearchAsync(string searchTerm);
    }
}
