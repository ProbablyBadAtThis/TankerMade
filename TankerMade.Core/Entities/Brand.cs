using System;

namespace TankerMade.Core.Entities
{
    public class Brand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; }

        // Protected constructor for Entity Framework
        protected Brand() { }

        // Main constructor
        public Brand(Guid id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CreatedAt = DateTime.UtcNow;
            Slug = GenerateSlug(name);
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
