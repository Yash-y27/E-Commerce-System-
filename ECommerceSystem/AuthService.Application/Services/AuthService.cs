using AuthService.Application.Common;
using AuthService.Application.Common.Constants;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Application.Services.Interfaces;
using AuthService.Domain.Entities;


namespace AuthService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Check if user exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new BadRequestException(
                ErrorMessages.UserAlreadyExists);

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber
        };

        await _userRepository.AddAsync(user);

        return new AuthResponseDto
        {
            Token = _jwtTokenGenerator.GenerateToken(user),
            Email = user.Email
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new BadRequestException(
                ErrorMessages.InvalidCredentials);

        return new AuthResponseDto
        {
            Token = _jwtTokenGenerator.GenerateToken(user),
            Email = user.Email
        };
    }

    public AuthService(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
}