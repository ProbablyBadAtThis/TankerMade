using Microsoft.EntityFrameworkCore;
using TankerMade.Core.Entities;
using TankerMade.Modules.Crafting.Entities;
using TankerMade.Modules.Printing3D.Entities;
using TankerMade.Server.Modules;

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
    public DbSet<CraftingProjectStepProgress> CraftingProjectStepProgress { get; set; }
    public DbSet<CraftingProjectTimer> CraftingProjectTimers { get; set; }
    public DbSet<CraftingPattern> CraftingPatterns { get; set; }
    public DbSet<CraftingPatternPiece> CraftingPatternPieces { get; set; }
    public DbSet<CraftingPatternStep> CraftingPatternSteps { get; set; }
    public DbSet<CraftingKit> CraftingKits { get; set; }
    public DbSet<CraftingKitPiece> CraftingKitPieces { get; set; }
    public DbSet<CraftingKitSupply> CraftingKitSupplies { get; set; }
    public DbSet<CraftingProjectInventoryLink> CraftingProjectInventoryLinks { get; set; }
    public DbSet<CraftingYarnInventoryItem> CraftingYarnInventoryItems { get; set; }
    public DbSet<CraftingYarnLot> CraftingYarnLots { get; set; }
    public DbSet<CraftingInventoryPurchase> CraftingInventoryPurchases { get; set; }
    public DbSet<CraftingToolInventoryItem> CraftingToolInventoryItems { get; set; }
    public DbSet<CraftingToolPurchase> CraftingToolPurchases { get; set; }
    public DbSet<CraftingNotionInventoryItem> CraftingNotionInventoryItems { get; set; }
    public DbSet<CraftingNotionPurchase> CraftingNotionPurchases { get; set; }
    public DbSet<CraftingInventoryReferenceItem> CraftingInventoryReferenceItems { get; set; }

    // Reference 3D printing module entities
    public DbSet<PrintingMaterialInventoryItem> PrintingMaterialInventoryItems { get; set; }
    public DbSet<PrintingSpool> PrintingSpools { get; set; }
    public DbSet<PrintingInventoryPurchase> PrintingInventoryPurchases { get; set; }
    public DbSet<PrintingInventoryReferenceItem> PrintingInventoryReferenceItems { get; set; }

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
        ConfigureCraftingProjectStepProgress(modelBuilder);
        ConfigureCraftingProjectTimer(modelBuilder);
        ConfigureCraftingPattern(modelBuilder);
        ConfigureCraftingPatternPiece(modelBuilder);
        ConfigureCraftingPatternStep(modelBuilder);
        ConfigureCraftingKit(modelBuilder);
        ConfigureCraftingKitPiece(modelBuilder);
        ConfigureCraftingKitSupply(modelBuilder);
        ConfigureCraftingProjectInventoryLink(modelBuilder);
        ConfigureCraftingYarnInventoryItem(modelBuilder);
        ConfigureCraftingYarnLot(modelBuilder);
        ConfigureCraftingInventoryPurchase(modelBuilder);
        ConfigureCraftingToolInventoryItem(modelBuilder);
        ConfigureCraftingToolPurchase(modelBuilder);
        ConfigureCraftingNotionInventoryItem(modelBuilder);
        ConfigureCraftingNotionPurchase(modelBuilder);
        ConfigureCraftingInventoryReferenceItem(modelBuilder);
        ConfigurePrintingMaterialInventoryItem(modelBuilder);
        ConfigurePrintingSpool(modelBuilder);
        ConfigurePrintingInventoryPurchase(modelBuilder);
        ConfigurePrintingInventoryReferenceItem(modelBuilder);
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

            entity.Property(e => e.Difficulty)
                .IsRequired();

            entity.Property(e => e.IsArchived)
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

            entity.HasOne<CraftingKit>()
                .WithMany()
                .HasForeignKey(e => e.KitId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne<CraftingKitPiece>()
                .WithMany()
                .HasForeignKey(e => e.KitPieceId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne<Theme>()
                .WithMany()
                .HasForeignKey(e => e.ThemeId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsArchived);
            entity.HasIndex(e => e.Slug);
            entity.HasIndex(e => e.KitId);
            entity.HasIndex(e => e.KitPieceId)
                .IsUnique();
        });
    }

    private void ConfigureCraftingProjectStepProgress(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingProjectStepProgress>(entity =>
        {
            entity.ToTable("CraftingProjectStepProgress");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.IsComplete)
                .IsRequired();

            entity.HasOne<CraftingProject>()
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<CraftingPatternStep>()
                .WithMany()
                .HasForeignKey(e => e.PatternStepId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.PatternStepId);
            entity.HasIndex(e => new { e.ProjectId, e.PatternStepId })
                .IsUnique();
        });
    }

    private void ConfigureCraftingProjectTimer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingProjectTimer>(entity =>
        {
            entity.ToTable("CraftingProjectTimers");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ElapsedSeconds)
                .IsRequired();

            entity.Property(e => e.IsRunning)
                .IsRequired();

            entity.HasOne<CraftingProject>()
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<CraftingPatternStep>()
                .WithMany()
                .HasForeignKey(e => e.PatternStepId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.PatternStepId);
            entity.HasIndex(e => new { e.ProjectId, e.PatternStepId })
                .IsUnique();
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

    private void ConfigureCraftingPatternPiece(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingPatternPiece>(entity =>
        {
            entity.ToTable("CraftingPatternPieces");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne<CraftingPattern>()
                .WithMany()
                .HasForeignKey(e => e.PatternId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PatternId);
            entity.HasIndex(e => new { e.PatternId, e.SortOrder });
        });
    }

    private void ConfigureCraftingPatternStep(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingPatternStep>(entity =>
        {
            entity.ToTable("CraftingPatternSteps");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Label)
                .HasMaxLength(100);

            entity.Property(e => e.Instructions)
                .HasMaxLength(4000);

            entity.HasOne<CraftingPatternPiece>()
                .WithMany()
                .HasForeignKey(e => e.PatternPieceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PatternPieceId);
            entity.HasIndex(e => new { e.PatternPieceId, e.SortOrder });
        });
    }

    private void ConfigureCraftingKit(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingKit>(entity =>
        {
            entity.ToTable("CraftingKits");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(220);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Difficulty).IsRequired();
            entity.Property(e => e.IsArchived).IsRequired();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Theme>()
                .WithMany()
                .HasForeignKey(e => e.ThemeId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsArchived);
            entity.HasIndex(e => e.Slug);
            entity.HasIndex(e => e.ThemeId);
        });
    }

    private void ConfigureCraftingKitPiece(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingKitPiece>(entity =>
        {
            entity.ToTable("CraftingKitPieces");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne<CraftingKit>()
                .WithMany()
                .HasForeignKey(e => e.KitId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<CraftingPattern>()
                .WithMany()
                .HasForeignKey(e => e.PatternId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.KitId);
            entity.HasIndex(e => e.PatternId);
            entity.HasIndex(e => new { e.KitId, e.SortOrder });
        });
    }

    private void ConfigureCraftingKitSupply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingKitSupply>(entity =>
        {
            entity.ToTable("CraftingKitSupplies");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SupplyType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne<CraftingKit>()
                .WithMany()
                .HasForeignKey(e => e.KitId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.KitId);
            entity.HasIndex(e => new { e.KitId, e.SortOrder });
        });
    }

    private void ConfigureCraftingProjectInventoryLink(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingProjectInventoryLink>(entity =>
        {
            entity.ToTable("CraftingProjectInventoryLinks");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.InventoryItemType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne<CraftingProject>()
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => new { e.ProjectId, e.InventoryItemType, e.InventoryItemId })
                .IsUnique();
        });
    }

    private void ConfigureCraftingYarnInventoryItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingYarnInventoryItem>(entity =>
        {
            entity.ToTable("CraftingYarnInventoryItems");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.BrandName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.ColorName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.NormalizedBrandName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.NormalizedColorName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.MainColor).HasMaxLength(100);
            entity.Property(e => e.WeightName).HasMaxLength(100);
            entity.Property(e => e.FiberContent).HasMaxLength(300);
            entity.Property(e => e.FiberTag).HasMaxLength(50);
            entity.Property(e => e.LengthUnit).HasMaxLength(20);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.NormalizedBrandName, e.NormalizedColorName })
                .IsUnique();
        });
    }

    private void ConfigureCraftingYarnLot(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingYarnLot>(entity =>
        {
            entity.ToTable("CraftingYarnLots");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.LotNumber).HasMaxLength(100);

            entity.HasOne<CraftingYarnInventoryItem>()
                .WithMany()
                .HasForeignKey(e => e.YarnInventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.YarnInventoryItemId);
            entity.HasIndex(e => new { e.YarnInventoryItemId, e.LotNumber })
                .IsUnique();
        });
    }

    private void ConfigureCraftingInventoryPurchase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingInventoryPurchase>(entity =>
        {
            entity.ToTable("CraftingInventoryPurchases");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SourceName).HasMaxLength(150);

            entity.HasOne<CraftingYarnInventoryItem>()
                .WithMany()
                .HasForeignKey(e => e.YarnInventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.YarnInventoryItemId);
        });
    }

    private void ConfigureCraftingToolInventoryItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingToolInventoryItem>(entity =>
        {
            entity.ToTable("CraftingToolInventoryItems");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.BrandName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.TypeName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.NormalizedBrandName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.NormalizedTypeName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Size).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.NormalizedBrandName, e.NormalizedTypeName })
                .IsUnique();
        });
    }

    private void ConfigureCraftingToolPurchase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingToolPurchase>(entity =>
        {
            entity.ToTable("CraftingToolPurchases");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SourceName).HasMaxLength(150);

            entity.HasOne<CraftingToolInventoryItem>()
                .WithMany()
                .HasForeignKey(e => e.ToolInventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ToolInventoryItemId);
        });
    }

    private void ConfigureCraftingNotionInventoryItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingNotionInventoryItem>(entity =>
        {
            entity.ToTable("CraftingNotionInventoryItems");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.BrandName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.TypeName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.NormalizedBrandName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.NormalizedTypeName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Size).HasMaxLength(100);
            entity.Property(e => e.ColorName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.NormalizedBrandName, e.NormalizedTypeName })
                .IsUnique();
        });
    }

    private void ConfigureCraftingNotionPurchase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingNotionPurchase>(entity =>
        {
            entity.ToTable("CraftingNotionPurchases");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SourceName).HasMaxLength(150);

            entity.HasOne<CraftingNotionInventoryItem>()
                .WithMany()
                .HasForeignKey(e => e.NotionInventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.NotionInventoryItemId);
        });
    }

    private void ConfigureCraftingInventoryReferenceItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CraftingInventoryReferenceItem>(entity =>
        {
            entity.ToTable("CraftingInventoryReferenceItems");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(170);

            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => new { e.Category, e.Slug })
                .IsUnique();
        });
    }

    private void ConfigurePrintingMaterialInventoryItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrintingMaterialInventoryItem>(entity =>
        {
            entity.ToTable("PrintingMaterialInventoryItems");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.MaterialType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BrandName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.ColorName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.NormalizedMaterialType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NormalizedBrandName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.NormalizedColorName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Diameter).HasMaxLength(50);
            entity.Property(e => e.StorageLocation).HasMaxLength(200);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.NormalizedMaterialType, e.NormalizedBrandName, e.NormalizedColorName })
                .IsUnique();
        });
    }

    private void ConfigurePrintingSpool(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrintingSpool>(entity =>
        {
            entity.ToTable("PrintingSpools");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SpoolCode).HasMaxLength(100);
            entity.Property(e => e.PrinterCompatibility).HasMaxLength(300);

            entity.HasOne<PrintingMaterialInventoryItem>()
                .WithMany()
                .HasForeignKey(e => e.MaterialInventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.MaterialInventoryItemId);
            entity.HasIndex(e => new { e.MaterialInventoryItemId, e.SpoolCode })
                .IsUnique();
        });
    }

    private void ConfigurePrintingInventoryPurchase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrintingInventoryPurchase>(entity =>
        {
            entity.ToTable("PrintingInventoryPurchases");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.SourceName).HasMaxLength(150);

            entity.HasOne<PrintingMaterialInventoryItem>()
                .WithMany()
                .HasForeignKey(e => e.MaterialInventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.MaterialInventoryItemId);
        });
    }

    private void ConfigurePrintingInventoryReferenceItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrintingInventoryReferenceItem>(entity =>
        {
            entity.ToTable("PrintingInventoryReferenceItems");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(170);

            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => new { e.Category, e.Slug })
                .IsUnique();
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

        var bundledModules = BundledModuleCatalog.Registrations
            .Select(registration => new
            {
                Id = registration.Id,
                ModuleKey = registration.Module.ModuleKey,
                Name = registration.Module.Name,
                Description = registration.Module.Description,
                Version = registration.Module.Version,
                IsBundled = registration.Module.IsBundled,
                CreatedAt = now
            })
            .ToArray();

        modelBuilder.Entity<ModuleDefinition>().HasData(bundledModules);

        modelBuilder.Entity<CraftingInventoryReferenceItem>().HasData(
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666601"), Category = "yarn-weight", Name = "Lace", Slug = "lace", SortOrder = 1, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666602"), Category = "yarn-weight", Name = "Fingering", Slug = "fingering", SortOrder = 2, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666603"), Category = "yarn-weight", Name = "DK", Slug = "dk", SortOrder = 3, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666604"), Category = "yarn-weight", Name = "Worsted", Slug = "worsted", SortOrder = 4, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666605"), Category = "yarn-weight", Name = "Bulky", Slug = "bulky", SortOrder = 5, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666606"), Category = "fiber-tag", Name = "Synthetic", Slug = "synthetic", SortOrder = 1, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666607"), Category = "fiber-tag", Name = "Natural", Slug = "natural", SortOrder = 2, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666608"), Category = "fiber-tag", Name = "Blended", Slug = "blended", SortOrder = 3, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666609"), Category = "tool-type", Name = "Hook", Slug = "hook", SortOrder = 1, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-66666666660a"), Category = "tool-type", Name = "Needle", Slug = "needle", SortOrder = 2, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-66666666660b"), Category = "tool-type", Name = "Gauge Ruler", Slug = "gauge-ruler", SortOrder = 3, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-66666666660c"), Category = "tool-type", Name = "Stitch Holder", Slug = "stitch-holder", SortOrder = 4, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-66666666660d"), Category = "notion-type", Name = "Button", Slug = "button", SortOrder = 1, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-66666666660e"), Category = "notion-type", Name = "Stitch Marker", Slug = "stitch-marker", SortOrder = 2, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-66666666660f"), Category = "notion-type", Name = "Tapestry Needle", Slug = "tapestry-needle", SortOrder = 3, CreatedAt = now },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666610"), Category = "notion-type", Name = "Zipper", Slug = "zipper", SortOrder = 4, CreatedAt = now }
        );

        modelBuilder.Entity<PrintingInventoryReferenceItem>().HasData(
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777701"), Category = "material-type", Name = "PLA", Slug = "pla", SortOrder = 1, CreatedAt = now },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777702"), Category = "material-type", Name = "PETG", Slug = "petg", SortOrder = 2, CreatedAt = now },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777703"), Category = "material-type", Name = "ABS", Slug = "abs", SortOrder = 3, CreatedAt = now },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777704"), Category = "material-type", Name = "TPU", Slug = "tpu", SortOrder = 4, CreatedAt = now },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777705"), Category = "diameter", Name = "1.75mm", Slug = "1-75mm", SortOrder = 1, CreatedAt = now },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777706"), Category = "diameter", Name = "2.85mm", Slug = "2-85mm", SortOrder = 2, CreatedAt = now },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777707"), Category = "printer-tooling", Name = "Nozzle", Slug = "nozzle", SortOrder = 1, CreatedAt = now },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777708"), Category = "printer-tooling", Name = "Build Plate", Slug = "build-plate", SortOrder = 2, CreatedAt = now },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777709"), Category = "printer-tooling", Name = "Filament Dryer", Slug = "filament-dryer", SortOrder = 3, CreatedAt = now },
            new { Id = Guid.Parse("77777777-7777-7777-7777-77777777770a"), Category = "printer-tooling", Name = "Scraper", Slug = "scraper", SortOrder = 4, CreatedAt = now }
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
