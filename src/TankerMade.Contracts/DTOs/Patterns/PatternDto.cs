using System;
using System.Linq;

namespace TankerMade.Contracts.DTOs.Patterns
{
    public class PatternDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public Guid? ThemeId { get; set; }
        public Guid? SourceId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ThemeName { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string DifficultyName { get; set; } = string.Empty;
        public string PatternTypeName { get; set; } = string.Empty;
        public string FormName { get; set; } = string.Empty;
    }
}