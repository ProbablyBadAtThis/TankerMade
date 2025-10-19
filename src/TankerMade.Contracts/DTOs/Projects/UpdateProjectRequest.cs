using System.ComponentModel.DataAnnotations;

namespace TankerMade.Contracts.DTOs.Projects
{
    public class UpdateProjectRequest
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;
        public Guid? PatternId { get; set; }
        public Guid? ThemeId { get; set; }
        [Required(ErrorMessage = "Difficulty is required")]
        [Range(1, 5, ErrorMessage = "Difficulty must be between 1 and 5")]
        public int Difficulty { get; set; }
        [Required(ErrorMessage = "Progress is required")]
        [Range(0.00, 100.00, ErrorMessage = "Progress must be between 0.00 and 100.00")]
        public decimal Progress { get; set; }
    }
}