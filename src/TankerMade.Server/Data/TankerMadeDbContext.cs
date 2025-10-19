using Microsoft.EntityFrameworkCore;
using TankerMade.Core.Entities;

namespace TankerMade.Server.Data;

public class TankerMadeDbContext : DbContext
{
    public TankerMadeDbContext(DbContextOptions<TankerMadeDbContext> options)
        : base(options)
    {
    }

    // User-owned entities
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Pattern> Patterns { get; set; }

    // Reference entities (shared)
    public DbSet<Theme> Themes { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Source> Sources { get; set; }
    public DbSet<Brand> Brands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureProject(modelBuilder);
        ConfigurePattern(modelBuilder);
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

            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.Username)
                .IsUnique();

            entity.HasIndex(e => e.Email)
                .IsUnique();
        });
    }

    private void ConfigureProject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
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

            entity.HasOne<Pattern>()
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

    private void ConfigurePattern(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pattern>(entity =>
        {
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
                .OnDelete(DeleteBehavior.Restrict);  // Changed from Cascade to Restrict

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
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333331"), Name = "Ravelry", Slug = "ravelry", CreatedAt = now },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333332"), Name = "Etsy", Slug = "etsy", CreatedAt = now },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "YouTube", Slug = "youtube", CreatedAt = now },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333334"), Name = "Book", Slug = "book", CreatedAt = now },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333335"), Name = "Custom", Slug = "custom", CreatedAt = now }
        );

        // Seed Brands
        modelBuilder.Entity<Brand>().HasData(
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444441"), Name = "Red Heart", Slug = "red-heart", CreatedAt = now },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444442"), Name = "Lion Brand", Slug = "lion-brand", CreatedAt = now },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444443"), Name = "Bernat", Slug = "bernat", CreatedAt = now },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Caron", Slug = "caron", CreatedAt = now }
        );
    }
}
