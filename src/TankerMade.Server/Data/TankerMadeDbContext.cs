using Microsoft.EntityFrameworkCore;
using TankerMade.Core.Entities;
using TankerMade.Modules.Crafting;
using TankerMade.Modules.Crafting.Entities;

namespace TankerMade.Server.Data;

public class TankerMadeDbContext : DbContext
{
    public TankerMadeDbContext(DbContextOptions<TankerMadeDbContext> options)
        : base(options)
    {
    }

    // User-owned entities
    public DbSet<User> Users { get; set; }

    // Module host entities
    public DbSet<ModuleDefinition> ModuleDefinitions { get; set; }
    public DbSet<UserModuleActivation> UserModuleActivations { get; set; }

    // Reference crafting module entities
    public DbSet<CraftingProject> CraftingProjects { get; set; }
    public DbSet<CraftingPattern> CraftingPatterns { get; set; }

    // Reference entities (shared)
    public DbSet<Theme> Themes { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Source> Sources { get; set; }
    public DbSet<Brand> Brands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureModules(modelBuilder);
        ConfigureCraftingProject(modelBuilder);
        ConfigureCraftingPattern(modelBuilder);
        ConfigureReferenceEntities(modelBuilder);
        SeedReferenceData(modelBuilder);
    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);  // BCrypt hashes are ~60 chars

            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.Username)
                .IsUnique();

            entity.HasIndex(e => e.Email)
                .IsUnique();
        });
    }


    private void ConfigureModules(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ModuleDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ModuleKey).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Version).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.ModuleKey).IsUnique();
        });

        modelBuilder.Entity<UserModuleActivation>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<ModuleDefinition>()
                .WithMany()
                .HasForeignKey(e => e.ModuleDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.ModuleDefinitionId })
                .IsUnique();
        });
    }

    private void ConfigureCraftingProject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingProject>(entity =>
        {
            entity.ToTable("CraftingProjects");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(220);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Progress)
                .HasPrecision(5, 2);

            entity.Property(e => e.Difficulty)
                .IsRequired();

            // Relationships
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<CraftingPattern>()
                .WithMany()
                .HasForeignKey(e => e.PatternId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne<Theme>()
                .WithMany()
                .HasForeignKey(e => e.ThemeId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Slug);
        });
    }

    private void ConfigureCraftingPattern(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingPattern>(entity =>
        {
            entity.ToTable("CraftingPatterns");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(220);

            entity.Property(e => e.Type)
                .HasMaxLength(50);

            entity.Property(e => e.Form)
                .HasMaxLength(50);

            entity.Property(e => e.Difficulty)
                .HasMaxLength(50);

            // Relationships
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Theme>()
                .WithMany()
                .HasForeignKey(e => e.ThemeId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne<Source>()
                .WithMany()
                .HasForeignKey(e => e.SourceId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Slug);
        });
    }

    private void ConfigureReferenceEntities(ModelBuilder modelBuilder)
    {
        // Theme
        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(120);
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        // Color
        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(120);
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        // Source
        modelBuilder.Entity<Source>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(120);
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        // Brand
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(120);
            entity.HasIndex(e => e.Slug).IsUnique();
        });
    }

    private void SeedReferenceData(ModelBuilder modelBuilder)
    {
        var now = new DateTime(2025, 10, 18, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<ModuleDefinition>().HasData(
            new
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555551"),
                ModuleKey = CraftingModule.ModuleKey,
                Name = CraftingModule.Name,
                Description = CraftingModule.Description,
                Version = CraftingModule.Version,
                IsBundled = true,
                CreatedAt = now
            }
        );

        // Seed Themes
        modelBuilder.Entity<Theme>().HasData(
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Animals", Slug = "animals", CreatedAt = now },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111112"), Name = "Baby", Slug = "baby", CreatedAt = now },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111113"), Name = "Home Decor", Slug = "home-decor", CreatedAt = now },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111114"), Name = "Toys", Slug = "toys", CreatedAt = now },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111115"), Name = "Clothing", Slug = "clothing", CreatedAt = now }
        );

        // Seed Colors
        modelBuilder.Entity<Color>().HasData(
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222221"), Name = "Red", Slug = "red", CreatedAt = now },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Blue", Slug = "blue", CreatedAt = now },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222223"), Name = "Green", Slug = "green", CreatedAt = now },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222224"), Name = "Yellow", Slug = "yellow", CreatedAt = now },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222225"), Name = "Black", Slug = "black", CreatedAt = now },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222226"), Name = "White", Slug = "white", CreatedAt = now }
        );

        // Seed Sources
        modelBuilder.Entity<Source>().HasData(
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333331"), Name = "Website", Slug = "website", CreatedAt = now },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333332"), Name = "Marketplace", Slug = "marketplace", CreatedAt = now },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Video", Slug = "video", CreatedAt = now },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333334"), Name = "Book", Slug = "book", CreatedAt = now },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333335"), Name = "Custom", Slug = "custom", CreatedAt = now }
        );

    }
}
