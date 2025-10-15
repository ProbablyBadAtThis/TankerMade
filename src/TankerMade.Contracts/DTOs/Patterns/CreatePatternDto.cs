namespace TankerMade.Contracts.DTOs.Patterns
{
    public class CreatePatternDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Form { get; set; }
        public string Difficulty { get; set; }
        public Guid UserId { get; set; }
        public Guid? ThemeId { get; set; }
        public Guid? SourceId { get; set; }
    }
}
