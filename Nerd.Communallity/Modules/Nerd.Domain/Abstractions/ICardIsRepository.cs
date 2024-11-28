using Nerd.Domain.Models;

namespace Nerd.Domain.Abstractions;

public interface ICardIsRepository
{
    Task<Card?> GetCardAsync(string numbers);
    Task<int> TransferFundsAsync(string sourcePayerCard, string destinationPayerCard, decimal amount);
    Task<int> WithdrawFromCardAsync(string payerCard, decimal amount);
}