using System;

namespace TankerMade.Contracts.DTOs.Projects
{
    public class UpdateProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? PatternId { get; set; }
        public Guid? ThemeId { get; set; }
        public int Difficulty { get; set; }
        public decimal Progress { get; set; }
    }
}
