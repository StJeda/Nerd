using Nerd.Domain.Enums;
using System.Collections.Frozen;

namespace Nerd.Domain.Extensions;

public static class ErrorExtensions
{
    public static FrozenDictionary<Errors, string> OperationErrors = new Dictionary<Errors, string>()
    {
        [Errors.Success] = "Ok",
        [Errors.FailedValidationCard] = "Validation occured error with Card",
        [Errors.FailedValidationAmount] = "Validation occured error with Amount",
        [Errors.FailedValidationGuid] = "Validation occured error with Guid",
        [Errors.FailedValidationMerchant] = "Validation occured error with Merchant",
        [Errors.CardIsNotExist] = "Card is not exist in system",
        [Errors.CardIsBlocked] = "Card is unactivated or blocked",
        [Errors.UncorrectedPin] = "It is uncorrected pin for your card",
        [Errors.ServerError] = "Internal Server Error"
    }.ToFrozenDictionary();
}