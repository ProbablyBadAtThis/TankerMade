using System;

namespace TankerMade.Core.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public Guid? PatternId { get; set; }
        public Guid? ThemeId { get; set; }
        public int Difficulty { get; set; }
        public decimal Progress { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Protected constructor for Entity Framework
        protected Project() { }

        // Main constructor for creating new projects
        public Project(Guid id, string name, Guid userId)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Progress = 0;

            // Generate slug from name
            Slug = GenerateSlug(name);
        }
        private string GenerateSlug(string name)
        {
            return name?.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("'", "")
                ?? string.Empty;
        }

        public void UpdateProgress(decimal newProgress)
        {
            if (newProgress < 0 || newProgress > 100)
                throw new ArgumentException("Progress must be between 0 and 100");

            Progress = newProgress;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}