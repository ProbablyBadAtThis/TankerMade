namespace TankerMade.Contracts.DTOs.Patterns
{
    public class CreatePatternDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid? ThemeId { get; set; }
        public Guid? SourceId { get; set; }
    }
}
