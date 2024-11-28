using FluentValidation;
using Nerd.Domain.DTOs;

namespace Nerd.Core.Validators;

public class CheckCardRequestValidator : AbstractValidator<CheckCardRequest>
{
    public CheckCardRequestValidator()
    {
        RuleFor(x => x.PayerCard)
            .NotEmpty().WithMessage("Card number is required.")
            .Length(16).WithMessage("Card number must be 16 digits.")
            .Matches(@"^\d{16}$").WithMessage("Card number must contain only digits.");

        RuleFor(x => x.PIN)
        .NotEmpty().WithMessage("PIN is required.")
        .Custom((pin, context) =>
        {
            if (pin.Length != 3)
            {
                context.AddFailure("PIN must be exactly 3 digits.");
            }
            if (pin.All(c => char.IsDigit(c)) is false)
            {
                context.AddFailure("PIN must contain only digits.");
            }
        });
    }
}
