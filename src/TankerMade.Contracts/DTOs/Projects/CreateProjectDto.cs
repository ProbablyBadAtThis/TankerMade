namespace TankerMade.Contracts.DTOs.Projects
{
    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? PatternId { get; set; }
        public Guid? ThemeId { get; set; }
        public int Difficulty { get; set; }
        public decimal Progress { get; set; }
        public Guid UserId { get; set; }
    }
}
