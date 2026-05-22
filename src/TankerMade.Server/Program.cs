using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using TankerMade.Contracts.Services;
using TankerMade.Modules.Crafting.Services;
using TankerMade.Server.Data;
using TankerMade.Server.Services;
using TankerMade.Server.Services.Crafting;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
var dataDir = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDir);

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

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT Secret Key not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Configure CORS for Blazor client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7001", "http://localhost:5001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<ICraftingProjectService, CraftingProjectService>();
builder.Services.AddScoped<ICraftingPatternService, CraftingPatternService>();

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

// HTTPS redirect disabled for local dev — run with --launch-profile https to enable
// app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
