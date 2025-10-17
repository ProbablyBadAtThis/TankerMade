# TankerMade Code Pattern Examples

## Entity Example (Project.cs)

    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        protected Project() { }
        
        public Project(Guid id, string name, Guid userId)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Slug = GenerateSlug(name);
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void UpdateName(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Slug = GenerateSlug(name);
            UpdatedAt = DateTime.UtcNow;
        }
        
        private string GenerateSlug(string name) => 
            name.ToLowerInvariant().Replace(" ", "-");
    }

## DTO Examples

    // Display DTO
    public class ProjectDisplayDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PatternName { get; set; }  // Flattened
        public DateTime CreatedAt { get; set; }
    }
    
    // Create DTO
    public class CreateProjectDto
    {
        public string Name { get; set; }
        public Guid PatternId { get; set; }
    }
    
    // Update DTO
    public class UpdateProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

## Service Example

    public interface IProjectService
    {
        Task<ProjectDisplayDto> GetByIdAsync(Guid id, Guid userId);
        Task<ProjectDisplayDto> CreateAsync(CreateProjectDto dto, Guid userId);
    }
    
    public class ProjectService : IProjectService
    {
        public async Task<ProjectDisplayDto> GetByIdAsync(Guid id, Guid userId)
        {
            var project = await _context.Projects.FindAsync(id);
            
            if (project == null)
                throw new NotFoundException("Project not found");
                
            if (project.UserId != userId)
                throw new UnauthorizedException("Not authorized");
                
            return MapToDto(project);
        }
    }
