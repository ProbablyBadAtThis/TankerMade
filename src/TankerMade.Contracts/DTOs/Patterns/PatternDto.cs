using System;
using System.Linq;

namespace TankerMade.Contracts.DTOs.Patterns
{
    public class PatternDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Type { get; set; }
        public string Form { get; set; }
        public string Difficulty { get; set; }
        public Guid? ThemeId { get; set; }
        public Guid? SourceId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ThemeName { get; set; }
        public string SourceName { get; set; }
        public string Username { get; set; }
        public string DifficultyName { get; set; }
        public string PatternTypeName { get; set; }
        public string FormName { get; set; }
    }
}