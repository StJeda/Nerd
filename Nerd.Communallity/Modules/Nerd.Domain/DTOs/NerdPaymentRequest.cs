using Nerd.Domain.Enums;

namespace Nerd.Domain.DTOs;

public record NerdPaymentRequest
{
    public required string PayerCard { get; init; }
    public required DateTime ExpDate { get; init; }
    public required string PIN { get; init; }
    public required decimal Amount { get; set; }
    public required Currency Currency { get; init; }
    public required string DebtSeria { get; init; }
}