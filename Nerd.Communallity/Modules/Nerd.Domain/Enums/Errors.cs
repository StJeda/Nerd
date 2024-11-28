namespace Nerd.Domain.Enums;

public enum Errors
{
    Success = 0,
    FailedValidationCard = 1,
    FailedValidationAmount = 2,
    FailedValidationGuid = 3,
    FailedValidationMerchant = 4,
    CardIsNotExist = 5,
    CardIsBlocked = 6,
    UncorrectedPin = 7,
    RequestIsBad = 400,
    ServerError = 500
}
