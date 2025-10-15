namespace TankerMade.Contracts.DTOs.Patterns
{
    public class UpdatePatternDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Form { get; set; }
        public string Difficulty { get; set; }
        public Guid? ThemeId { get; set; }
        public Guid? SourceId { get; set; }
    }
}
