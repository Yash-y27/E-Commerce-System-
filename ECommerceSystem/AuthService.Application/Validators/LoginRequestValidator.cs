using FluentValidation;
using AuthService.Application.DTOs;

namespace AuthService.Application.Validators;

public class LoginRequestValidator
    : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6)
            .MaximumLength(100);
    }
}