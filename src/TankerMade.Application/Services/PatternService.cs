using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TankerMade.Application.Common.Exceptions;
using TankerMade.Contracts.DTOs.Patterns;
using TankerMade.Contracts.Services;

namespace TankerMade.Application.Services
{
    public class PatternService : IPatternService
    {
        // TODO: Add constructor with repository dependencies when ready
        // private readonly IPatternRepository _patternRepository;
        // public PatternService(IPatternRepository patternRepository) => _patternRepository = patternRepository;

        public async Task<PatternDto> CreateAsync(CreatePatternDto createDto, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<PatternDto> GetByIdAsync(Guid id, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<PatternDto> GetBySlugAsync(string slug, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<IEnumerable<PatternDto>> GetAllAsync(Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<IEnumerable<PatternDto>> GetByThemeIdAsync(Guid themeId, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<IEnumerable<PatternDto>> GetByDifficultyAsync(int difficulty, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
        public async Task<PatternDto> UpdateAsync(UpdatePatternDto updateDto, Guid userId)
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

        public async Task<IEnumerable<PatternDto>> SearchAsync(string searchTerm, Guid userId)
        {
            // TODO: ???
            throw new NotImplementedException("Operation not implemented yet");
        }
    }
}
