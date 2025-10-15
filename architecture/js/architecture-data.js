// Core registry for diagrams and dictionary

window.TM = {
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

  services: {
    IProjectService: {
      creates: ["CreateProjectDto"],
      reads: ["ProjectDto"],
      updates: ["UpdateProjectDto"],
      deletes: ["ProjectDto"],
      business: ["GetBySlugAsync", "GetByUserIdAsync", "SearchAsync"]
    },
    IPatternService: {
      creates: ["CreatePatternDto"],
      reads: ["PatternDto"],
      updates: ["UpdatePatternDto"],
      deletes: ["PatternDto"],
      business: ["GetBySlugAsync", "GetByUserIdAsync", "GetByThemeIdAsync", "GetByDifficultyAsync", "SearchAsync"]
    },
    IUserService: {
      creates: ["CreateUserDto"],
      reads: ["UserDto"],
      updates: ["UpdateUserDto"],
      deletes: ["UserDto"],
      business: ["GetByUsernameAsync", "GetByEmailAsync", "UsernameExistsAsync", "EmailExistsAsync", "SearchAsync"]
    }
  },

  entities: ["Project", "Pattern", "User", "Theme", "Source"],

  // Data dictionary: add or edit as your model evolves
  fields: [
    {
      name: "Id",
      type: "Guid",
      control: "system",
      purpose: "Primary key for all persisted entities",
      usedIn: ["Project", "Pattern", "User"],
      validation: "Auto-generated; unique",
      security: "System managed"
    },
    {
      name: "Name",
      type: "string",
      control: "user",
      purpose: "Human-readable name",
      usedIn: ["ProjectDto", "CreateProjectDto", "UpdateProjectDto", "PatternDto", "CreatePatternDto", "UpdatePatternDto"],
      validation: "Required; trimmed; length limits",
      security: "User editable"
    },
    {
      name: "Slug",
      type: "string",
      control: "system",
      purpose: "URL-friendly identifier derived from Name",
      usedIn: ["ProjectDto", "PatternDto"],
      validation: "Unique; lowercase; hyphenated",
      security: "Read-only to users"
    },
    {
      name: "CreatedAt",
      type: "DateTime",
      control: "system",
      purpose: "Audit: when the record was created",
      usedIn: ["ProjectDto", "PatternDto", "UserDto"],
      validation: "Set by system clock or DB",
      security: "Read-only"
    },
    {
      name: "UpdatedAt",
      type: "DateTime",
      control: "system",
      purpose: "Audit: last modification time",
      usedIn: ["ProjectDto"],
      validation: "Set by system on update",
      security: "Read-only"
    },
    {
      name: "Username",
      type: "string",
      control: "user",
      purpose: "User’s public handle",
      usedIn: ["UserDto", "CreateUserDto", "UpdateUserDto"],
      validation: "Required; unique; format constraints",
      security: "User editable (self) or admin"
    },
    {
      name: "Email",
      type: "string",
      control: "user",
      purpose: "User’s primary email address",
      usedIn: ["UserDto", "CreateUserDto", "UpdateUserDto"],
      validation: "Required; unique; RFC-compliant",
      security: "User editable (self) or admin"
    },
    {
      name: "Password",
      type: "string",
      control: "user",
      purpose: "Plain text input to be hashed and stored",
      usedIn: ["CreateUserDto", "UpdateUserDto"],
      validation: "Required on create; complexity rules",
      security: "Never stored in plain; hash in service"
    },
    {
      name: "Role",
      type: "string",
      control: "admin",
      purpose: "Authorization role (e.g., Admin, User)",
      usedIn: ["UserDto", "CreateUserDto", "UpdateUserDto"],
      validation: "Enum/whitelist",
      security: "Admin-only to change"
    },
    {
      name: "ThemeId",
      type: "Guid?",
      control: "user",
      purpose: "Foreign key to Theme",
      usedIn: ["PatternDto", "CreatePatternDto", "UpdatePatternDto"],
      validation: "Optional; FK constraint",
      security: "User editable"
    },
    {
      name: "SourceId",
      type: "Guid?",
      control: "user",
      purpose: "Foreign key to Source/Reference",
      usedIn: ["PatternDto", "CreatePatternDto", "UpdatePatternDto"],
      validation: "Optional; FK constraint",
      security: "User editable"
    },
    {
      name: "Difficulty",
      type: "int|string",
      control: "user",
      purpose: "Pattern or project difficulty",
      usedIn: ["ProjectDto", "CreateProjectDto", "UpdateProjectDto", "PatternDto", "CreatePatternDto", "UpdatePatternDto"],
      validation: "Enum coercion; range",
      security: "User editable"
    },
    {
      name: "Progress",
      type: "decimal",
      control: "user",
      purpose: "Project completion percentage",
      usedIn: ["ProjectDto", "CreateProjectDto", "UpdateProjectDto"],
      validation: "0–100 range",
      security: "User editable"
    },
    {
      name: "UserId",
      type: "Guid",
      control: "system",
      purpose: "Ownership – derived from auth context",
      usedIn: ["CreateProjectDto", "CreatePatternDto"],
      validation: "Supplied by server from token",
      security: "Not user-controlled in service"
    }
  ]
};
