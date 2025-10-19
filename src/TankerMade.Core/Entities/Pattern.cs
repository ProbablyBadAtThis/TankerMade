using System;

namespace TankerMade.Core.Entities
{
    public class Pattern
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public Guid? PatternId { get; set; }
        public Guid? ThemeId { get; set; }
        public Guid? SourceId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Protected constructor for Entity Framework
        protected Pattern() { }

        // Main constructor for creating new Patterns
        public Pattern(Guid id, string name, Guid userId)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            // Generate slug from name
            Slug = GenerateSlug(name);

            // Other properties can be set later via methods or properties
        }

        private string GenerateSlug(string name)
        {
            return name?.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("'", "")
                ?? string.Empty;
        }
    }
}
