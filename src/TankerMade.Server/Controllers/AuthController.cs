using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TankerMade.Contracts.DTOs.Auth;
using TankerMade.Core.Entities;
using TankerMade.Server.Data;
using TankerMade.Server.Services;

namespace TankerMade.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TankerMadeDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly JwtSettingsOptions _jwtSettings;

    public AuthController(
        TankerMadeDbContext context,
        IPasswordService passwordService,
        IJwtService jwtService,
        JwtSettingsOptions jwtSettings)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Validate model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if username already exists
        var existingUsername = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (existingUsername != null)
        {
            return BadRequest(new { error = "Username already exists" });
        }

        // Check if email already exists
        var existingEmail = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingEmail != null)
        {
            return BadRequest(new { error = "Email already exists" });
        }

        // Hash password
        var passwordHash = _passwordService.HashPassword(request.Password);

        // Create new user
        var user = new User(
            Guid.NewGuid(),
            request.Username,
            request.Email,
            passwordHash,
            request.Role
        );

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);

        // Return auth response
        return Ok(new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = _jwtSettings.GetExpiresAtUtc(DateTime.UtcNow)
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Validate model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Find user by username
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            return Unauthorized(new { error = "Invalid username or password" });
        }

        // Verify password
        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { error = "Invalid username or password" });
        }

        // Update last login
        user.UpdateLastLogin();
        await _context.SaveChangesAsync();

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);

        // Return auth response
        return Ok(new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = _jwtSettings.GetExpiresAtUtc(DateTime.UtcNow)
        });
    }
}
