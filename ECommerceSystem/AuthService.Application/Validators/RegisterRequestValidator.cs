using FluentValidation;
using AuthService.Application.DTOs;
using System.Text.RegularExpressions;

namespace AuthService.Application.Validators;

public class RegisterRequestValidator
    : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 50).WithMessage("First name must be 2–50 characters")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Length(2, 50).WithMessage("Last name must be 2–50 characters")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Last name can only contain letters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(100)
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
            .Matches(@"[\W]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x)
            .Must(x => !x.Email.Contains(x.FirstName, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Email cannot contain first name for security reasons");
    }
}