using Nerd.Domain.DTOs;

namespace Nerd.Domain.Abstractions;

public interface ICardIsService
{
    Task<CheckCardResponse> CheckCardAsync(CheckCardRequest request);
    Task<PayMoneyResponse> FillCardByCardAsync(TransferRequest request);
    Task<PayMoneyResponse> WithdrawByCardAsync(PayMoneyRequest request);
}