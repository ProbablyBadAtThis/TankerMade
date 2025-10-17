# TankerMade Project - Master Context Document
**Last Updated:** October 16, 2025  
**Current Phase:** Phase 1 - Foundation (In Progress)

---

## 🎯 Project Overview
TankerMade is a **craft project management application** supporting multiple craft types (crochet, knitting, sewing, quilting, embroidery).

### Architecture Stack
- **Backend:** ASP.NET Core 9.0 Web API (C#)
- **Frontend:** Blazor WebAssembly (C#)
- **Database:** SQL Server with Entity Framework Core
- **Authentication:** JWT tokens
- **Deployment:** Cloudflare Pages (documentation only)

### Operating Modes
1. **Offline Mode:** IndexedDB for local storage
2. **Server Mode:** API + SQL Server
3. **Hybrid:** Seamless switching between modes

---

## 📁 Solution Structure

    TankerMade/
    ├── .github/
    │   └── workflows/                    # GitHub Actions (future)
    ├── artifacts/
    │   ├── artifact.md                   # Project artifacts
    │   └── project-artifact.md           # Additional artifacts
    ├── docs/
    │   ├── index.html                    # Documentation hub landing
    │   ├── architecture-visualizer/      # Interactive architecture diagrams
    │   │   ├── index.html
    │   │   ├── assets/
    │   │   ├── js/                       # Diagram JavaScript modules
    │   │   │   ├── architecture-data.js
    │   │   │   ├── data-dictionary.js
    │   │   │   ├── data-flow-diagram.js
    │   │   │   ├── dto-flow-diagram.js
    │   │   │   └── service-flow-diagram.js
    │   │   └── styles/                   # Diagram CSS
    │   │       ├── diagram.css
    │   │       └── main.css
    │   ├── dev-tracker/                  # Static HTML progress tracker
    │   │   ├── index.html                # Dashboard
    │   │   ├── phase-1.html through phase-10.html
    │   │   ├── shared.css                # Shared styles
    │   │   └── shared.js                 # Shared JavaScript
    │   └── workbench/                    # Architecture documentation
    │       ├── index.html
    │       ├── architecture.html
    │       ├── deployment.html
    │       ├── development-guidelines.html
    │       ├── domain-model.html
    │       ├── implementation-phases.html
    │       ├── technical-specs.html
    │       └── ui-ux-specs.html
    ├── exclude/
    │   └── docs/                         # Context documents (not deployed)
    │       ├── TANKERMADE_PROJECT_CONTEXT.md
    │       ├── TANKERMADE_PATTERNS_EXAMPLES.md
    │       ├── THREAD_STARTER_TEMPLATE.md
    │       └── WEEKLY_REVIEW.md
    └── src/
        ├── TankerMade.Core/              # Domain entities, enums
        │   ├── Entities/
        │   │   ├── Brand.cs
        │   │   ├── Color.cs
        │   │   ├── Pattern.cs
        │   │   ├── Project.cs
        │   │   ├── Source.cs
        │   │   ├── Theme.cs
        │   │   └── User.cs
        │   ├── Enums/
        │   │   ├── Difficulty.cs
        │   │   ├── PatternForm.cs
        │   │   ├── PatternType.cs
        │   │   └── UserRole.cs
        │   └── TankerMade.Core.csproj
        ├── TankerMade.Contracts/         # DTOs, service interfaces
        │   ├── DTOs/
        │   │   ├── Patterns/
        │   │   │   ├── PatternDto.cs
        │   │   │   ├── CreatePatternDto.cs
        │   │   │   └── UpdatePatternDto.cs
        │   │   ├── Projects/
        │   │   │   ├── ProjectDto.cs
        │   │   │   ├── CreateProjectDto.cs
        │   │   │   └── UpdateProjectDto.cs
        │   │   └── Users/
        │   │       ├── UserDto.cs
        │   │       ├── CreateUserDto.cs
        │   │       └── UpdateUserDto.cs
        │   ├── Services/
        │   │   ├── IPatternService.cs
        │   │   ├── IProjectService.cs
        │   │   └── IUserService.cs
        │   └── TankerMade.Contracts.csproj
        ├── TankerMade.Application/       # Service implementations
        │   ├── Common/
        │   │   └── Exceptions/
        │   │       └── UnauthorizedException.cs
        │   ├── Services/
        │   │   └── UserService.cs
        │   └── TankerMade.Application.csproj
        ├── TankerMade.Client/            # Blazor WASM UI
        │   ├── Layout/
        │   │   ├── MainLayout.razor
        │   │   └── NavMenu.razor
        │   ├── Pages/
        │   │   ├── Home.razor
        │   │   ├── Counter.razor
        │   │   └── Weather.razor
        │   ├── wwwroot/
        │   │   ├── css/
        │   │   ├── lib/bootstrap/
        │   │   └── index.html
        │   ├── App.razor
        │   ├── Program.cs
        │   └── TankerMade.Client.csproj
        └── TankerMade.Server/            # ASP.NET Web API
            ├── Program.cs
            ├── appsettings.json
            ├── appsettings.Development.json
            └── TankerMade.Server.csproj


---

## 🏗️ Architecture Patterns

### Clean Architecture Layers
1. **Core Layer** (TankerMade.Core)
   - Domain entities
   - Enums for system constants
   - No external dependencies
   
2. **Contracts Layer** (TankerMade.Contracts)
   - DTOs (Data Transfer Objects)
   - Service interfaces (IProjectService, etc.)
   - Shared between Client and Server
   
3. **Application Layer** (TankerMade.Application)
   - Service implementations
   - Business logic
   - References: Core + Contracts
   
4. **Infrastructure** (Implicit in Server project)
   - Entity Framework DbContext
   - Database migrations
   - External service integrations

### Dependency Rules
- Core → Nothing (pure domain)
- Contracts → Core only
- Application → Core + Contracts
- Client → Contracts only (API calls)
- Server → Core + Contracts + Application

---

## 📐 Design Patterns

### Entity Pattern
All entities follow this structure:

    public class Entity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Protected constructor for EF Core
        protected Entity() { }
        
        // Public constructor with validation
        public Entity(
            Guid id,
            string name,
            Guid userId)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        // Business methods update UpdatedAt
        public void UpdateName(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            UpdatedAt = DateTime.UtcNow;
        }
    }

Key principles:
- Protected parameterless constructor for EF Core
- Public constructor validates required fields
- Use `?? throw` pattern for validation
- Business methods (not setters) for updates
- Business methods update `UpdatedAt`

### DTO Pattern

**Display DTOs:** Full data with flattened relationships

    public class EntityDisplayDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid PatternId { get; set; }
        public string PatternName { get; set; }  // Flattened
        public DateTime CreatedAt { get; set; }
    }

**Create DTOs:** Only user-provided fields

    public class CreateEntityDto
    {
        // NO Id - generated by system
        // NO timestamps - set by system
        // NO UserId - from authenticated context
        public string Name { get; set; }
        public Guid PatternId { get; set; }
    }

**Update DTOs:** Id + updatable fields only

    public class UpdateEntityDto
    {
        public Guid Id { get; set; }  // To identify record
        public string Name { get; set; }
        // NO timestamps - updated by system
        // NO UserId - cannot be changed
    }

### Service Pattern

    public interface IEntityService
    {
        Task<EntityDisplayDto> GetByIdAsync(Guid id, Guid userId);
        Task<IEnumerable<EntityDisplayDto>> GetAllAsync(Guid userId);
        Task<EntityDisplayDto> CreateAsync(CreateEntityDto dto, Guid userId);
        Task<EntityDisplayDto> UpdateAsync(UpdateEntityDto dto, Guid userId);
        Task DeleteAsync(Guid id, Guid userId);
    }

Key principles:
- All methods async (Task<T>)
- UserId parameter for ownership validation
- Return Display DTOs
- Accept Create/Update DTOs as input

---

## 🔐 Security Principles

1. **User Ownership Validation**
   - Every service method validates user owns the resource
   - Throw UnauthorizedException if not
   
2. **Role-Based Authorization**
   - Admin can access all records
   - User can only access their own
   - Moderator has limited admin access
   
3. **DTO Security**
   - Never expose passwords in Display DTOs
   - UserId set from authenticated context, not user input
   - System fields (timestamps, slugs) not in input DTOs
   
4. **Constructor Validation**
   - Required fields validated with `?? throw`
   - Business rules enforced in entity constructors
   - Invalid state prevented at construction time

---

## 🗄️ Core Entities (Phase 1 - Completed)

### Project Entity
Properties:
- Id (Guid)
- Name (string)
- Slug (string) - auto-generated from Name
- Description (string, nullable)
- PatternId (Guid, nullable)
- ThemeId (Guid, nullable)
- Difficulty (Difficulty enum)
- Progress (decimal, 0-100)
- UserId (Guid)
- CreatedAt, UpdatedAt (DateTime)

Methods:
- UpdateProgress(decimal progress)
- UpdateDetails(name, description, difficulty)

### Pattern Entity
Properties:
- Id, Name, Slug
- Type (PatternType enum)
- Form (PatternForm enum)
- Difficulty (Difficulty enum)
- ThemeId, SourceId (Guid, nullable)
- UserId (Guid)
- CreatedAt, UpdatedAt

Methods:
- UpdateDetails(type, form, difficulty)

### User Entity
Properties:
- Id, Username, Email
- Role (UserRole enum)
- CreatedAt, UpdatedAt, LastLoginAt (DateTime)

Methods:
- UpdateLastLogin()
- UpdateRole(string role)

Security: Password NOT in entity (handled by authentication layer)

---

## 📦 Reference Entities (Completed)

Simple lookup tables with: Id, Name, Slug, CreatedAt
- Theme
- Color
- Source
- Brand

No user ownership - shared across all users

---

## 🎨 Enums (System Constants)

### Difficulty
- Beginner = 1
- Easy = 2
- Intermediate = 3
- Advanced = 4
- Expert = 5

### PatternType
- Crochet = 1
- Knitting = 2
- Sewing = 3
- Embroidery = 4
- Quilting = 5

### PatternForm
- Written = 1
- Charted = 2
- Video = 3
- Diagram = 4
- Photo = 5

### UserRole
- User = 1
- Admin = 2
- Moderator = 3

---

## ✅ Phase 1 Progress Checklist

### Solution Structure (COMPLETE)
- ✅ Create solution
- ✅ Create TankerMade.Core project
- ✅ Create TankerMade.Contracts project
- ✅ Create TankerMade.Client project (Blazor WASM)
- ✅ Create TankerMade.Server project (ASP.NET Web API)
- ✅ Add projects to solution
- ✅ Set project references

### Domain Models (COMPLETE)
- ✅ Create Project entity
- ✅ Create Pattern entity
- ✅ Create User entity
- ✅ Create Difficulty enum
- ✅ Create PatternType enum
- ✅ Create PatternForm enum
- ✅ Create UserRole enum
- ✅ Create Theme, Color, Source, Brand reference entities

### DTOs (COMPLETE)
- ✅ ProjectDto, CreateProjectDto, UpdateProjectDto
- ✅ PatternDto, CreatePatternDto, UpdatePatternDto
- ✅ UserDto, CreateUserDto, UpdateUserDto

### Service Interfaces (COMPLETE)
- ✅ IProjectService interface
- ✅ IPatternService interface
- ✅ IUserService interface

### Service Implementations (IN PROGRESS)
- ✅ UserService with role-based security
- ✅ PatternService 
- ✅ ProjectService 

### Client Application (NOT STARTED)
- ❌ Setup dependencies
- ❌ Create layout & navigation
- ❌ Setup routing
- ❌ Create home page
- ❌ IndexedDB service setup
- ❌ Offline service implementations
- ❌ Data seeding
- ❌ CRUD pages (list, detail, create/edit)
- ❌ Client validation
- ❌ Mode configuration (offline/server)
- ❌ Settings page
- ❌ Auth pages (login/register)

### Server Foundation (NOT STARTED)
- ❌ Install EF Core packages
- ❌ Create DbContext
- ❌ Configure entity relationships
- ❌ Create initial migration
- ❌ Seed reference data
- ❌ Install JWT packages
- ❌ Configure JWT authentication
- ❌ Create JWT service
- ❌ Create auth endpoints
- ❌ Implement password hashing
- ❌ Setup Minimal APIs
- ❌ Create API endpoints (projects, patterns, users)
- ❌ Add authorization filters
- ❌ Implement server validation
- ❌ Add error handling middleware
- ❌ Register services in DI container
- ❌ Setup AutoMapper
- ❌ Test auth flow
- ❌ Test CRUD operations

---

## 🌐 Documentation Sites

### Dev Tracker
- **URL:** https://tankermade.pages.dev/dev-tracker/
- **Purpose:** Track development progress through 10 phases
- **Tech:** Static HTML + CSS + JS
- **Features:**
  - Modular architecture (shared.css, shared.js)
  - Dynamic header/nav injection
  - FOUC prevention (preload + fade-in)
  - Progress tracking via HTML `checked` attributes (Git) + localStorage (browser)
  - Export/import functionality
  - Per-phase progress visualization

### Documentation Hub
- **URL:** https://tankermade.pages.dev/
- **Purpose:** Central landing page for all documentation
- **Links to:**
  - Dev Tracker
  - Workbench (architecture decisions - planned)
  - Architecture Visualizer (diagrams - planned)

---

## 🚀 Development Workflow

### Creating a New Entity
1. Add to `TankerMade.Core/Entities/`
2. Include: Id, CreatedAt, UpdatedAt
3. Add protected parameterless constructor
4. Add public constructor with required parameters
5. Validate required parameters with `?? throw`
6. Add business methods (not property setters)
7. Business methods update `UpdatedAt`

### Creating DTOs
1. Add to `TankerMade.Contracts/DTOs/{EntityName}/`
2. Create `{Entity}DisplayDto` with all properties
3. Flatten navigation properties (e.g., PatternName instead of Pattern)
4. Create `Create{Entity}Dto` with user input only
5. Create `Update{Entity}Dto` with Id + updatable fields

### Creating Services
1. Add interface to `TankerMade.Contracts/Services/`
2. Define async CRUD methods
3. Add userId parameter to all methods
4. Add implementation to `TankerMade.Application/Services/`
5. Validate user ownership
6. Check roles for admin operations
7. Return Display DTOs

### Updating Dev Tracker
1. Open relevant phase HTML file
2. Add `checked` attribute to completed checkboxes
3. Commit with message: `chore: update Dev Tracker - [task description]`
4. Push to GitHub
5. Cloudflare auto-deploys in ~30-60 seconds

---

## 📝 Coding Standards

### Naming Conventions
- **Entities:** Singular (Project, not Projects)
- **DTOs:** `{Entity}DisplayDto`, `Create{Entity}Dto`, `Update{Entity}Dto`
- **Services:** `I{Entity}Service` (interface), `{Entity}Service` (implementation)
- **Methods:** PascalCase, descriptive verbs (GetByIdAsync, not Get)
- **Parameters:** camelCase
- **Private fields:** _camelCase with underscore prefix

### File Organization
- One class per file
- File name matches class name
- Organize by feature, not by type

### Comments
- XML documentation comments on all public interfaces/methods
- Inline comments only for non-obvious logic
- No obvious comments ("// Set the name to the parameter")

### Error Handling
- Custom exceptions for domain rules
- `ArgumentNullException` for null parameters
- `UnauthorizedException` for security violations
- `InvalidOperationException` for invalid state
- Never swallow exceptions

---

## 🎯 Next Steps (When Resuming Development)

### Immediate Tasks
1. Complete IPatternService interface definition
2. Implement PatternService class
3. Create IProjectService interface
4. Implement ProjectService class
5. Review all service implementations for consistency

### After Service Layer Complete
1. Install Entity Framework Core packages in Server project
2. Create TankerMadeDbContext
3. Configure entity relationships and constraints
4. Create initial migration
5. Seed reference data (Themes, Sources, Brands)

---

## 🔧 Tools & Resources

### Development Tools
- Visual Studio 2022 (or later)
- SQL Server (LocalDB or Express)
- Git for version control
- GitHub for repository hosting
- Cloudflare Pages for documentation hosting

### Documentation URLs
- **Production Site:** https://tankermade.pages.dev/
- **Dev Tracker:** https://tankermade.pages.dev/dev-tracker/
- **GitHub Repo:** (private)

### Key Commands
    # Create new migration
    dotnet ef migrations add MigrationName --project TankerMade.Server
    
    # Update database
    dotnet ef database update --project TankerMade.Server
    
    # Run server
    dotnet run --project TankerMade.Server
    
    # Run client
    dotnet run --project TankerMade.Client

---

## 📌 Important Notes

### What This Project IS
- C# / Blazor WASM / ASP.NET Core
- Entity Framework Core for data access
- JWT authentication
- Clean architecture with clear layer separation
- Offline-first capability with server sync

### What This Project IS NOT
- ❌ NOT Node.js / Next.js / React
- ❌ NOT Tauri desktop app
- ❌ NOT using Rust
- ❌ NOT using SQLite
- ❌ NOT a previous YarnProject iteration

### Reference Confusion Alert
The Space contains old YarnProject files from a DIFFERENT project. Those used Next.js/Tauri/SQLite. IGNORE THOSE. This is TankerMade with C#/Blazor/SQL Server.

---

## 🧠 Memory & Context Guidelines

### For New Threads
Always start with:
1. Attach this context document
2. State current phase and specific task
3. Reference Dev Tracker URL for current progress
4. Remind about architecture stack (C#/Blazor/ASP.NET)

### For Code Reviews
Ask to validate against:
- Entity pattern (protected + public constructor)
- DTO pattern (Display vs Input separation)
- Service pattern (ownership validation)
- Security principles (role-based, no password exposure)

### For Consistency
Before implementing anything:
- Check existing similar implementations
- Follow established patterns
- Ask for review before committing

---

**END OF CONTEXT DOCUMENT**
