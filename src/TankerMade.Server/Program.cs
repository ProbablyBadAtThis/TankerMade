using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using TankerMade.Contracts.Services;
using TankerMade.Contracts.Services.ModuleCapabilities;
using TankerMade.Modules.Knitting.Services;
using TankerMade.Modules.Printing3D.Services;
using TankerMade.Server.Data;
using TankerMade.Server.Modules;
using TankerMade.Server.Services;
using TankerMade.Server.Services.Assets;
using TankerMade.Server.Services.Knitting;
using TankerMade.Server.Services.ModuleCapabilities;
using TankerMade.Server.Services.Printing3D;

var builder = WebApplication.CreateBuilder(args);

BundledModuleCatalog.Validate();

var moduleDiscoveryOptions = builder.Configuration
    .GetSection("ModuleDiscovery")
    .Get<ModuleDiscoveryOptions>() ?? new ModuleDiscoveryOptions();
var assetStorageOptions = builder.Configuration
    .GetSection("AssetStorage")
    .Get<AssetStorageOptions>() ?? new AssetStorageOptions();
var jwtSettings = builder.Configuration
    .GetSection(JwtSettingsOptions.SectionName)
    .Get<JwtSettingsOptions>() ?? new JwtSettingsOptions();

var configuredAssetRoot = string.IsNullOrWhiteSpace(assetStorageOptions.RootDirectory)
    ? "App_Data/assets"
    : assetStorageOptions.RootDirectory.Trim();
var assetRootPath = Path.IsPathRooted(configuredAssetRoot)
    ? configuredAssetRoot
    : Path.Combine(builder.Environment.ContentRootPath, configuredAssetRoot);
Directory.CreateDirectory(assetRootPath);

// Add DbContext
var dataDir = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDir);
var dataProtectionKeyDir = Path.Combine(dataDir, "DataProtectionKeys");
Directory.CreateDirectory(dataProtectionKeyDir);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var connectionBuilder = new SqliteConnectionStringBuilder(connectionString);
if (!Path.IsPathRooted(connectionBuilder.DataSource))
{
    connectionBuilder.DataSource = Path.Combine(builder.Environment.ContentRootPath, connectionBuilder.DataSource);
    connectionString = connectionBuilder.ConnectionString;
}

builder.Services.AddDbContext<TankerMadeDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeyDir))
    .SetApplicationName("TankerMade");

ValidateJwtSettings(jwtSettings);
builder.Services.AddSingleton(jwtSettings);
var secretKey = jwtSettings.SecretKey;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddMemoryCache();

// Configure CORS for Blazor client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7001",
                "http://localhost:5001",
                "https://localhost:7051",
                "http://localhost:5017")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<IModuleRegistrationService, ModuleRegistrationService>();
builder.Services.AddScoped<IModuleDiscoveryProvider, BundledModuleDiscoveryProvider>();
builder.Services.AddSingleton(moduleDiscoveryOptions);
builder.Services.AddSingleton(assetStorageOptions);
builder.Services.AddScoped<IModuleDiscoveryProvider, ExternalManifestModuleDiscoveryProvider>();
builder.Services.AddSingleton<IAssetStorageService, LocalDiskAssetStorageService>();
builder.Services.AddScoped<IAssetThumbnailService, AssetThumbnailService>();
builder.Services.AddScoped<IKnittingPatternService, KnittingPatternService>();
builder.Services.AddScoped<IKnittingProjectService, KnittingProjectService>();
builder.Services.AddScoped<IKnittingInventoryService, KnittingInventoryService>();
builder.Services.AddScoped<IKnittingKitService, KnittingKitService>();
builder.Services.AddScoped<IKnittingSettingsService, KnittingSettingsService>();
builder.Services.AddScoped<IPrintingInventoryService, PrintingInventoryService>();
builder.Services.AddScoped<IModulePatternCapabilityResolver, ModulePatternCapabilityResolver>();
builder.Services.AddScoped<IModuleProjectCapabilityResolver, ModuleProjectCapabilityResolver>();
builder.Services.AddScoped<IModuleInventoryCapabilityResolver, ModuleInventoryCapabilityResolver>();
builder.Services.AddScoped<IModuleKitCapabilityResolver, ModuleKitCapabilityResolver>();
builder.Services.AddScoped<IModuleSettingsCapabilityResolver, ModuleSettingsCapabilityResolver>();
builder.Services.AddScoped<IModuleRecentWorkSummaryResolver, ModuleRecentWorkSummaryResolver>();
builder.Services.AddScoped<IModuleDashboardContributionResolver, ModuleDashboardContributionResolver>();
builder.Services.AddScoped<IRecentWorkService, RecentWorkService>();
builder.Services.AddScoped<IDashboardOverviewService, DashboardOverviewService>();
builder.Services.AddScoped<IModulePatternCapabilityHandler, KnittingPatternCapabilityHandler>();
builder.Services.AddScoped<IModuleProjectCapabilityHandler, KnittingProjectCapabilityHandler>();
builder.Services.AddScoped<IModuleInventoryCapabilityHandler, KnittingInventoryCapabilityHandler>();
builder.Services.AddScoped<IModuleInventoryCapabilityHandler, PrintingInventoryCapabilityHandler>();
builder.Services.AddScoped<IModuleKitCapabilityHandler, KnittingKitCapabilityHandler>();
builder.Services.AddScoped<IModuleSettingsCapabilityHandler, KnittingSettingsCapabilityHandler>();
builder.Services.AddScoped<IModuleRecentWorkSummaryProvider, KnittingRecentWorkSummaryProvider>();
builder.Services.AddScoped<IModuleDashboardContributionProvider, KnittingDashboardContributionProvider>();

// Add controllers and OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Paste the JWT from /api/Auth/login. Scalar adds the 'Bearer' prefix."
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
        });

        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Ensure App_Data directory exists and apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TankerMadeDbContext>();
    db.Database.Migrate();

    var moduleRegistration = scope.ServiceProvider.GetRequiredService<IModuleRegistrationService>();
    await moduleRegistration.SyncDiscoveredModulesAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "TankerMade API";
        options.Theme = ScalarTheme.Mars;
    });
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static void ValidateJwtSettings(JwtSettingsOptions settings)
{
    if (string.IsNullOrWhiteSpace(settings.SecretKey))
    {
        throw new InvalidOperationException(
            $"Missing {JwtSettingsOptions.SectionName}:SecretKey. Configure it via user-secrets or an environment variable.");
    }

    if (settings.SecretKey.Length < JwtSettingsOptions.MinimumSecretLength)
    {
        throw new InvalidOperationException(
            $"{JwtSettingsOptions.SectionName}:SecretKey must be at least {JwtSettingsOptions.MinimumSecretLength} characters.");
    }

    if (string.IsNullOrWhiteSpace(settings.Issuer))
    {
        throw new InvalidOperationException($"Missing {JwtSettingsOptions.SectionName}:Issuer.");
    }

    if (string.IsNullOrWhiteSpace(settings.Audience))
    {
        throw new InvalidOperationException($"Missing {JwtSettingsOptions.SectionName}:Audience.");
    }

    if (settings.ExpirationMinutes <= 0)
    {
        throw new InvalidOperationException($"{JwtSettingsOptions.SectionName}:ExpirationMinutes must be greater than 0.");
    }
}
