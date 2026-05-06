using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Application.Services.Interfaces;
using AuthService.Domain.Entities;

namespace AuthService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Check if user exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Exception("User already exists");

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = request.Password // TEMP (we'll hash later)
        };

        await _userRepository.AddAsync(user);

        return new AuthResponseDto
        {
            Token = "dummy-token",
            Email = user.Email
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || user.PasswordHash != request.Password)
            throw new Exception("Invalid credentials");

        return new AuthResponseDto
        {
            Token = "dummy-token",
            Email = user.Email
        };
    }
}