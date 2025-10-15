// Central data registry for TankerMade Architecture Visualizer
window.TM = {
    // DTO structure mapping
    dtos: {
        projects: {
            display: "ProjectDto",
            create: "CreateProjectDto",
            update: "UpdateProjectDto"
        },
        patterns: {
            display: "PatternDto",
            create: "CreatePatternDto",
            update: "UpdatePatternDto"
        },
        users: {
            display: "UserDto",
            create: "CreateUserDto",
            update: "UpdateUserDto"
        }
    },

    // Service interfaces and their operations
    services: {
        IProjectService: {
            creates: ["CreateProjectDto"],
            reads: ["ProjectDto"],
            updates: ["UpdateProjectDto"],
            deletes: ["ProjectDto"],
            business: ["GetBySlugAsync", "GetByUserIdAsync", "SearchAsync", "ExistsAsync"]
        },
        IPatternService: {
            creates: ["CreatePatternDto"],
            reads: ["PatternDto"],
            updates: ["UpdatePatternDto"],
            deletes: ["PatternDto"],
            business: ["GetBySlugAsync", "GetByUserIdAsync", "GetByThemeIdAsync", "GetByDifficultyAsync", "SearchAsync", "ExistsAsync"]
        },
        IUserService: {
            creates: ["CreateUserDto"],
            reads: ["UserDto"],
            updates: ["UpdateUserDto"],
            deletes: ["UserDto"],
            business: ["GetByUsernameAsync", "GetByEmailAsync", "UsernameExistsAsync", "EmailExistsAsync", "SearchAsync", "ExistsAsync"]
        }
    },

    // Domain entities
    entities: ["Project", "Pattern", "User", "Theme", "Source"],

    // Complete field definitions with current TankerMade schema
    fields: [
        {
            name: "Id",
            type: "Guid",
            control: "system",
            purpose: "Primary key for all persisted entities",
            usedIn: ["ProjectDto", "PatternDto", "UserDto"],
            validation: "Auto-generated; unique",
            security: "System managed"
        },
        {
            name: "Name",
            type: "string",
            control: "user",
            purpose: "Human-readable name for projects, patterns, and display",
            usedIn: ["ProjectDto", "CreateProjectDto", "UpdateProjectDto", "PatternDto", "CreatePatternDto", "UpdatePatternDto"],
            validation: "Required; trimmed; length 1-200 characters",
            security: "User editable; owner or admin only"
        },
        {
            name: "Slug",
            type: "string",
            control: "system",
            purpose: "URL-friendly identifier auto-generated from Name field",
            usedIn: ["ProjectDto", "PatternDto"],
            validation: "Unique; lowercase; hyphenated; auto-generated",
            security: "Read-only to users; system manages uniqueness"
        },
        {
            name: "Description",
            type: "string",
            control: "user",
            purpose: "Detailed text description for projects and patterns",
            usedIn: ["ProjectDto", "CreateProjectDto", "UpdateProjectDto", "PatternDto", "CreatePatternDto", "UpdatePatternDto"],
            validation: "Optional; max 2000 characters",
            security: "User editable; owner or admin only"
        },
        {
            name: "CreatedAt",
            type: "DateTime",
            control: "system",
            purpose: "Audit timestamp: when the record was created",
            usedIn: ["ProjectDto", "PatternDto", "UserDto"],
            validation: "Set by system clock on entity creation",
            security: "Read-only; audit trail"
        },
        {
            name: "UpdatedAt",
            type: "DateTime",
            control: "system",
            purpose: "Audit timestamp: last modification time",
            usedIn: ["ProjectDto"],
            validation: "Set by system on any update operation",
            security: "Read-only; audit trail"
        },
        {
            name: "Username",
            type: "string",
            control: "user",
            purpose: "User's unique public identifier for login",
            usedIn: ["UserDto", "CreateUserDto", "UpdateUserDto"],
            validation: "Required; unique; 3-50 chars; alphanumeric + underscore",
            security: "User editable (self) or admin; must remain unique"
        },
        {
            name: "Email",
            type: "string",
            control: "user",
            purpose: "User's primary email address for auth and communication",
            usedIn: ["UserDto", "CreateUserDto", "UpdateUserDto"],
            validation: "Required; unique; valid email format",
            security: "User editable (self) or admin; must remain unique"
        },
        {
            name: "Password",
            type: "string",
            control: "user",
            purpose: "Plain text password input (hashed before storage)",
            usedIn: ["CreateUserDto", "UpdateUserDto"],
            validation: "Required on create; min 8 chars; complexity rules",
            security: "Never stored plain; hashed with salt; user or admin"
        },
        {
            name: "Role",
            type: "string",
            control: "admin",
            purpose: "Authorization role (Admin, User) for permission control",
            usedIn: ["UserDto", "CreateUserDto", "UpdateUserDto"],
            validation: "Enum validation; default 'User'",
            security: "Admin-only modification; affects system permissions"
        },
        {
            name: "LastLoginAt",
            type: "DateTime?",
            control: "system",
            purpose: "Audit timestamp: user's most recent login",
            usedIn: ["UserDto"],
            validation: "Nullable; set on successful authentication",
            security: "System managed; privacy-sensitive data"
        },
        {
            name: "UserId",
            type: "Guid",
            control: "system",
            purpose: "Foreign key establishing ownership relationship",
            usedIn: ["CreateProjectDto", "CreatePatternDto", "ProjectDto", "PatternDto"],
            validation: "Required FK; must reference valid User.Id",
            security: "Derived from authentication; not user-controlled"
        },
        {
            name: "ThemeId",
            type: "Guid?",
            control: "user",
            purpose: "Optional foreign key to Theme for categorization",
            usedIn: ["PatternDto", "CreatePatternDto", "UpdatePatternDto"],
            validation: "Optional FK; must reference valid Theme.Id if provided",
            security: "User editable; owner or admin only"
        },
        {
            name: "SourceId",
            type: "Guid?",
            control: "user",
            purpose: "Optional foreign key to Source for pattern attribution",
            usedIn: ["PatternDto", "CreatePatternDto", "UpdatePatternDto"],
            validation: "Optional FK; must reference valid Source.Id if provided",
            security: "User editable; owner or admin only"
        },
        {
            name: "Type",
            type: "string",
            control: "user",
            purpose: "Pattern type classification (Crochet, Knitting, etc.)",
            usedIn: ["PatternDto", "CreatePatternDto", "UpdatePatternDto"],
            validation: "Required; enum validation against PatternType",
            security: "User editable; owner or admin only"
        },
        {
            name: "Form",
            type: "string",
            control: "user",
            purpose: "Pattern form classification (2D, 3D, Wearable, etc.)",
            usedIn: ["PatternDto", "CreatePatternDto", "UpdatePatternDto"],
            validation: "Required; enum validation against PatternForm",
            security: "User editable; owner or admin only"
        },
        {
            name: "Difficulty",
            type: "string",
            control: "user",
            purpose: "Skill level required (Beginner, Intermediate, Advanced, Expert)",
            usedIn: ["ProjectDto", "CreateProjectDto", "UpdateProjectDto", "PatternDto", "CreatePatternDto", "UpdatePatternDto"],
            validation: "Required; enum validation against DifficultyLevel",
            security: "User editable; owner or admin only"
        },
        {
            name: "Progress",
            type: "decimal",
            control: "user",
            purpose: "Project completion percentage (0-100)",
            usedIn: ["ProjectDto", "CreateProjectDto", "UpdateProjectDto"],
            validation: "Range 0.00-100.00; decimal precision 2",
            security: "User editable; owner or admin only"
        },
        {
            name: "PatternId",
            type: "Guid?",
            control: "user",
            purpose: "Optional foreign key linking Project to Pattern",
            usedIn: ["ProjectDto", "CreateProjectDto", "UpdateProjectDto"],
            validation: "Optional FK; must reference valid Pattern.Id if provided",
            security: "User editable; owner or admin only"
        }
    ],

    // Implementation status tracking
    implementation: {
        dtos: {
            complete: ["ProjectDto", "CreateProjectDto", "UpdateProjectDto", "PatternDto", "CreatePatternDto", "UpdatePatternDto", "UserDto", "CreateUserDto", "UpdateUserDto"],
            pending: []
        },
        services: {
            interfaces: ["IProjectService", "IPatternService", "IUserService"],
            implementations: {
                complete: ["UserService"],
                inProgress: [],
                pending: ["ProjectService", "PatternService"]
            }
        }
    }
};
