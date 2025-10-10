namespace TankerMade.Contracts.DTOs.Projects
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public Guid? PatternId { get; set; }
        public string PatternName { get; set; }  // Flattened from Pattern entity
        public Guid? ThemeId { get; set; }
        public string ThemeName { get; set; }    // Flattened from Theme entity
        public int Difficulty { get; set; }
        public string DifficultyName { get; set; } // Enum display value
        public decimal Progress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }     // Flattened from User entity
    }
}