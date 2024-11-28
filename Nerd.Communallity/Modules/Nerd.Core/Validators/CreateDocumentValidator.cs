using FluentValidation;
using Nerd.Domain.DTOs;
using Nerd.Domain.Enums;
using static Nerd.Core.Extensions.ErrorExtensions;

namespace Lotus.API.Validators;

public class CreateDocumentValidator : AbstractValidator<CreateDocumentRequest>
{
    private static readonly decimal AmountLimit = 30000.0m;
    
    public CreateDocumentValidator()
    {
        RuleFor(m => m.PayInfo.PayerCard)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .CreditCard()
            .WithErrorCode(Errors.FailedValidationCard.ToString("D"))
            .WithMessage(OperationErrors[Errors.FailedValidationCard]);

        RuleFor(m => m.Amount)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(x => x > decimal.Zero && x <= AmountLimit)
            .WithErrorCode(Errors.FailedValidationAmount.ToString("D"))
            .WithMessage(OperationErrors[Errors.FailedValidationAmount]);

        RuleFor(m => m.DocumentId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithErrorCode(Errors.FailedValidationGuid.ToString("D"))
            .WithMessage(OperationErrors[Errors.FailedValidationGuid]);

        RuleFor(m => m.MerchantName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
            .WithErrorCode(Errors.FailedValidationMerchant.ToString("D"))
            .WithMessage(OperationErrors[Errors.FailedValidationMerchant]);

    }
}
